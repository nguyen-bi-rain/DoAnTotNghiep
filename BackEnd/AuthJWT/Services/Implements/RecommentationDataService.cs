using AuthJWT.Domain.Contracts;
using AuthJWT.Domain.Entities.Common;
using AuthJWT.Services.Interfaces;
using AuthJWT.UnitOfWorks;
using Microsoft.EntityFrameworkCore;

namespace AuthJWT.Services.Implements
{
    internal static class RoomDataCache
    {
        private static readonly object _cacheLock = new();
        private static List<RecommenVectorData>? _cachedRoomData;
        private static DateTime _cacheExpiry = DateTime.MinValue;
        private static readonly TimeSpan CacheTimeout = TimeSpan.FromMinutes(1);
        private static int _dataVersion = 0; // Track data version

        public static (List<RecommenVectorData>? data, int version) GetCachedData()
        {
            lock (_cacheLock)
            {
                if (_cachedRoomData != null && DateTime.UtcNow < _cacheExpiry)
                {
                    return (_cachedRoomData, _dataVersion);
                }
                return (null, _dataVersion);
            }
        }

        public static bool SetCachedData(List<RecommenVectorData> data, int currentVersion = -1)
        {
            lock (_cacheLock)
            {
                // If version is provided and matches current version, data hasn't changed
                if (currentVersion >= 0 && currentVersion == _dataVersion)
                {
                    // Just update expiry time
                    _cacheExpiry = DateTime.UtcNow.Add(CacheTimeout);
                    return false; // No change to data
                }

                // Explicitly remove previous cache data
                _cachedRoomData = null;
                // Set new cache data
                _cachedRoomData = data;
                _cacheExpiry = DateTime.UtcNow.Add(CacheTimeout);
                _dataVersion++; // Increment version
                return true; // Data was changed
            }
        }
    }

    public class RecommendationService(IUnitOfWork unitOfWork, VectorProcess vectorProcess, IS3Service s3Service) : IRecommendationService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly VectorProcess _vectorProcess = vectorProcess;
        private readonly IS3Service _s3Service = s3Service;

        public async Task<List<RecommenVectorData>> CollectAllRoomData(DateTime fromDate = default, DateTime endDate = default, int numberGuest = 1, string location = null)
        {
            // Use default dates if not specified
            if (fromDate == default)
                fromDate = DateTime.UtcNow;
            if (endDate == default)
                endDate = DateTime.UtcNow.AddDays(1);

            var (cachedData, version) = RoomDataCache.GetCachedData();
            if (cachedData != null)
            {
                return cachedData;
            }

            var query = _unitOfWork.RoomRepository.GetQuery()
                .Include(r => r.Hotel)
                    .ThenInclude(h => h.Location)
                .Include(r => r.Hotel.Ratings)
                .Include(r => r.Conveniences)
                    .ThenInclude(rc => rc.Convenience)
                .Include(r => r.Bookings)
                    .ThenInclude(b => b.Booking)
                .Where(r => r.Hotel.IsApproved && r.Capacity >= numberGuest);

            // Add location filter if provided
            if (!string.IsNullOrEmpty(location))
            {
                query = query.Where(r => r.Hotel.Location.City == location);
            }

            var rooms = await query
                .AsSplitQuery()
                .AsNoTracking()
                .ToArrayAsync();

            // Filter out rooms with conflicting bookings
            var availableRooms = rooms
                .Where(r => !r.Bookings.Any(br =>
                    (br.Booking.Status == "confirmed" || br.Booking.Status == "Pending") &&
                    fromDate < br.Booking.CheckOutDate &&
                    endDate > br.Booking.CheckInDate
                ))
                .ToArray();

            var result = ProcessRoomData(availableRooms);
            bool dataChanged = RoomDataCache.SetCachedData(result, version);
            // Could log or take action based on dataChanged flag
            return result;
        }

        private static List<RecommenVectorData> ProcessRoomData(Room[] rooms)
        {
            var result = new List<RecommenVectorData>(rooms.Length);

            for (int i = 0; i < rooms.Length; i++)
            {
                var room = rooms[i];
                var avgRating = room.Hotel.Ratings?.Count > 0 ?
                    room.Hotel.Ratings.Average(r => r.RatingValue) : 0;

                result.Add(new RecommenVectorData
                {
                    HotelName = room.Hotel.Name,
                    RoomId = room.Id,
                    RoomPrice = (double)room.PricePerNight,
                    RoomNumber = room.NumberOfRooms,
                    RoomTypeId = room.RoomTypeId,
                    Capacity = room.Capacity,
                    BedCount = room.NumberOfBeds,
                    ViewType = room.ViewType,
                    Convenviences = room.Conveniences?.Select(c => c.Convenience?.Name)
                        .Where(name => !string.IsNullOrEmpty(name))
                        .Cast<string>().ToList() ?? [],
                    RatingValue = avgRating,
                    BookingCount = room.Bookings?.Count ?? 0
                });
            }

            return result;
        }

        public async Task<List<RecommenVectorData>> GetUserBookingHistory(string userId)
        {
            var bookings = await _unitOfWork.BookingRepository.GetQuery(b => b.UserId == userId)
                .Include(b => b.BookingRooms)
                    .ThenInclude(br => br.Room)
                        .ThenInclude(r => r.Hotel)
                            .ThenInclude(h => h.Ratings)
                .Include(b => b.BookingRooms)
                    .ThenInclude(br => br.Room)
                        .ThenInclude(r => r.Conveniences)
                            .ThenInclude(rc => rc.Convenience)
                .AsNoTracking()
                .ToArrayAsync();

            return ProcessBookingData(bookings);
        }

        private static List<RecommenVectorData> ProcessBookingData(Booking[] bookings)
        {
            var result = new List<RecommenVectorData>();

            for (int i = 0; i < bookings.Length; i++)
            {
                var booking = bookings[i];
                if (booking.BookingRooms != null)
                {
                    foreach (var bookingRoom in booking.BookingRooms)
                    {
                        ProcessBookingRoom(bookingRoom, result);
                    }
                }
            }

            return result;
        }

        private static void ProcessBookingRoom(BookingRoom bookingRoom, List<RecommenVectorData> result)
        {
            var room = bookingRoom.Room;
            var hotelAvgRating = room.Hotel.Ratings?.Count > 0 ?
                room.Hotel.Ratings.Average(r => r.RatingValue) : 0;

            result.Add(new RecommenVectorData
            {
                HotelName = room.Hotel.Name,
                RoomId = room.Id,
                RoomPrice = (double)room.PricePerNight,
                RoomNumber = room.NumberOfRooms,
                RoomTypeId = room.RoomTypeId,
                Capacity = room.Capacity,
                BedCount = room.NumberOfBeds,
                ViewType = room.ViewType,
                Convenviences = room.Conveniences?.Select(c => c.Convenience?.Name)
                    .Where(name => !string.IsNullOrEmpty(name))
                    .Cast<string>().ToList() ?? [],
                RatingValue = hotelAvgRating,
                BookingCount = 1
            });
        }

        public async Task<PaginateList<HotelResponse>> GetRecommendedRooms(
            string userId,
            DateTime fromDate = default,
            DateTime endDate = default,
            int numberGuest = 1,
            string location = null,
            int page = 1,
            int pageSize = 10, int sortBy = 0)
        {
            var allRooms = await CollectAllRoomData(fromDate, endDate, numberGuest, location);
            var userHistory = await GetUserBookingHistory(userId);

            List<HotelResponse> recommendedRooms;
            if (userHistory.Count == 0)
            {
                recommendedRooms = await GetPopularRooms(allRooms);
            }
            else
            {
                recommendedRooms = await ProcessRecommendations(allRooms, userHistory);
            }

            if (sortBy != 0)
            {
                switch (sortBy)
                {
                    case 1: // Sort by price ascending
                        recommendedRooms = recommendedRooms.OrderBy(r => r.Price).ToList();
                        break;
                    case 2: // Sort by price descending
                        recommendedRooms = recommendedRooms.OrderByDescending(r => r.Price).ToList();
                        break;
                    case 3: // Sort by rating ascending
                        recommendedRooms = recommendedRooms.OrderBy(r => double.Parse(r.RatingValue)).ToList();
                        break;
                    case 4: // Sort by rating descending
                        recommendedRooms = recommendedRooms.OrderByDescending(r => double.Parse(r.RatingValue)).ToList();
                        break;
                    default:
                        break;
                }
            }

            // Apply pagination
            return PaginateList<HotelResponse>.Create(
                recommendedRooms,
                page,
                pageSize);
        }
        private async Task<List<HotelResponse>> GetPopularRooms(List<RecommenVectorData> allRooms)
        {
            var popularRoomIds = allRooms
                .OrderByDescending(r => r.BookingCount)
                .ThenByDescending(r => r.RatingValue)
                .Select(r => r.RoomId)
                .ToArray();

            return await GetRoomResponses(popularRoomIds);
        }

        private async Task<List<HotelResponse>> ProcessRecommendations(
            List<RecommenVectorData> allRooms,
            List<RecommenVectorData> userHistory)
        {
            var weights = _vectorProcess.GetFeatureWeights();
            var allRoomVectors = CreateRoomVectors(allRooms, weights);
            var userRoomIds = new HashSet<Guid>(userHistory.Select(h => h.RoomId));

            var userVector = CreateUserVector(userHistory, weights);
            var similarities = CalculateSimilarities(allRoomVectors, userVector, userRoomIds);

            var recommendedRoomIds = similarities
                .OrderByDescending(kv => kv.Value)
                .Select(kv => kv.Key)
                .ToArray();

            return await GetRoomResponses(recommendedRoomIds);
        }

        private Dictionary<Guid, double[]> CreateRoomVectors(List<RecommenVectorData> allRooms, double[] weights)
        {
            var allRoomVectors = new Dictionary<Guid, double[]>(allRooms.Count);

            foreach (var room in allRooms)
            {
                var roomVector = ConvertToVector(room);
                if (roomVector.Length != weights.Length)
                {
                    Array.Resize(ref roomVector, weights.Length);
                }
                allRoomVectors[room.RoomId] = _vectorProcess.ApplyWeights(roomVector, weights);
            }

            return allRoomVectors;
        }

        private double[] CreateUserVector(List<RecommenVectorData> userHistory, double[] weights)
        {
            var userVector = new double[weights.Length];

            foreach (var history in userHistory)
            {
                var historyVector = ConvertToVector(history);
                if (historyVector.Length != weights.Length)
                {
                    Array.Resize(ref historyVector, weights.Length);
                }
                var weightedHistoryVector = _vectorProcess.ApplyWeights(historyVector, weights);

                for (int i = 0; i < userVector.Length; i++)
                {
                    userVector[i] += weightedHistoryVector[i];
                }
            }

            var historyCount = userHistory.Count;
            for (int i = 0; i < userVector.Length; i++)
            {
                userVector[i] /= historyCount;
            }

            return userVector;
        }

        private Dictionary<Guid, double> CalculateSimilarities(
            Dictionary<Guid, double[]> allRoomVectors,
            double[] userVector,
            HashSet<Guid> userRoomIds)
        {
            var similarities = new Dictionary<Guid, double>(allRoomVectors.Count);

            foreach (var pair in allRoomVectors)
            {
                // Tính toán điểm tương đồng cho tất cả phòng (không loại bỏ phòng đã đặt)
                similarities[pair.Key] = _vectorProcess.CalculateCosineSimilarity(userVector, pair.Value);

                // Tùy chọn: Có thể tăng điểm cho phòng đã đặt nếu muốn ưu tiên chúng
                if (userRoomIds.Contains(pair.Key))
                {
                    // Tăng điểm tương đồng cho phòng đã đặt (có thể điều chỉnh hệ số này)
                    // Điều này khiến phòng đã đặt có thể xuất hiện cao hơn trong kết quả
                    similarities[pair.Key] *= 1.1; // Tăng 10% điểm tương đồng
                }
            }

            return similarities;
        }

        private async Task<List<HotelResponse>> GetRoomResponses(Guid[] roomIds)
        {
            var rooms = await _unitOfWork.RoomRepository.GetQuery(r => roomIds.Contains(r.Id))
                .Include(r => r.Hotel)
                    .ThenInclude(h => h.Ratings)
                .Include(r => r.Hotel)
                    .ThenInclude(h => h.HotelImages)
                .Include(r => r.Hotel)
                    .ThenInclude(h => h.Location)
                .AsNoTracking()
                .ToArrayAsync();

            // Create dictionary to map roomId to its position in the roomIds array
            var rankMap = new Dictionary<Guid, int>();
            for (int i = 0; i < roomIds.Length; i++)
            {
                rankMap[roomIds[i]] = i;
            }

            // Group rooms by hotel and store best rank for each hotel
            var hotelGroupsWithRank = rooms
                .GroupBy(r => r.HotelId)
                .Select(group => new
                {
                    HotelGroup = group,
                    // Find best (lowest) rank among rooms in this hotel
                    BestRank = group.Min(r => rankMap.GetValueOrDefault(r.Id, int.MaxValue))
                })
                // Order by the best rank to preserve recommendation order
                .OrderByDescending(x => x.BestRank)
                .ToArray();

            var result = new List<HotelResponse>(hotelGroupsWithRank.Length);

            foreach (var item in hotelGroupsWithRank)
            {
                var hotelResponse = await CreateHotelResponse(item.HotelGroup);
                result.Add(hotelResponse);
            }

            return result;
        }

        private async Task<HotelResponse> CreateHotelResponse(IGrouping<Guid, Room> hotelGroup)
        {
            var hotel = hotelGroup.First().Hotel;
            var avgRating = hotel.Ratings?.Count > 0 ? hotel.Ratings.Average(r => r.RatingValue) : 0;
            var minPrice = hotelGroup.Min(r => r.PricePerNight);
            var thumbnail = hotel.HotelImages?.FirstOrDefault(x => x.ImageType == "Thumbnail")?.ImageUrl.ToString();

            return new HotelResponse
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Thumnail = !string.IsNullOrEmpty(thumbnail) ? await _s3Service.GetFileUrlAsync(thumbnail) : string.Empty,
                RatingValue = avgRating.ToString("F1"),
                RatingCount = hotel.Ratings?.Count ?? 0,
                Location = $"{hotel.Location?.City}",
                Price = minPrice
            };
        }

        private static double[] ConvertToVector(RecommenVectorData data)
        {
            return
            [
                data.RoomPrice,
                data.RoomNumber,
                data.RoomTypeId.GetHashCode(),
                data.Capacity,
                data.BedCount,
                data.ViewType?.GetHashCode() ?? 0,
                data.Convenviences.Count,
                data.RatingValue,
                data.BookingCount
            ];
        }
    }
}