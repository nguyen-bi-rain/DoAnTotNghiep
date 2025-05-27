using AuthJWT.Domain.Contracts;
using AuthJWT.Domain.DTOs;
using AuthJWT.Domain.Entities.Common;
using AuthJWT.Exceptions;
using AuthJWT.Services.Interfaces;
using AuthJWT.UnitOfWorks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AuthJWT.Services.Implements;

public class RatingService : IRatingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RatingService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<RatingCreateDto> CreateRatingAsync(RatingCreateDto rating)
    {
        var model = _mapper.Map<Rating>(rating);
        await _unitOfWork.RatingRepository.AddAsync(model);
        await EnsureChangesSavedAsync("Failed to create rating.");
        return rating;
    }

    public async Task<bool> DeleteRatingAsync(Guid id)
    {
        var rating = await GetRatingByIdAsyncInternal(id);
        _unitOfWork.RatingRepository.Delete(rating);
        return await _unitOfWork.SaveChangesAsync() > 0;
    }

    public async Task<IEnumerable<RatingDto>> GetAllRatingsAsync()
    {
        var ratings = await _unitOfWork.RatingRepository.GetQuery().ToListAsync();
        return _mapper.Map<IEnumerable<RatingDto>>(ratings);
    }

    public async Task<RatingDto> GetRatingByIdAsync(Guid id)
    {
        var rating = await GetRatingByIdAsyncInternal(id);
        return _mapper.Map<RatingDto>(rating);
    }

    public async Task<PaginateList<RatingResponse>> GetRatingsByHotelIdAsync(Guid hotelId, int pageIndex = 1, int pageSize = 10)
    {
        var ratings = await _unitOfWork.RatingRepository
            .GetQuery(x => x.HotelId == hotelId).Include(x => x.User).ToListAsync();
        var ratingResponses = ratings.Select(r => new RatingResponse
        {
            RatingValue = r.RatingValue,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt,
            UserName = $"{r.User.FirstName} {r.User.LastName}"
        }).ToList();
    
        return PaginateList<RatingResponse>.Create(ratingResponses, pageIndex, pageSize);
    }

    public async Task<RatingUpdateDto> UpdateRatingAsync(RatingUpdateDto rating)
    {
        await GetRatingByIdAsyncInternal(rating.Id); // Ensure rating exists
        var model = _mapper.Map<Rating>(rating);
        _unitOfWork.RatingRepository.Update(model);
        await EnsureChangesSavedAsync("Failed to update rating.");
        return rating;
    }

    private async Task<Rating> GetRatingByIdAsyncInternal(Guid id)
    {
        return await _unitOfWork.RatingRepository
            .GetQueryById(id)
            .FirstOrDefaultAsync() 
            ?? throw new ResourceNotFoundException($"Rating with ID {id} not found.");
    }

    private async Task EnsureChangesSavedAsync(string errorMessage)
    {
        if (await _unitOfWork.SaveChangesAsync() <= 0)
        {
            throw new DatabaseBadRequestException(errorMessage);
        }
    }
}