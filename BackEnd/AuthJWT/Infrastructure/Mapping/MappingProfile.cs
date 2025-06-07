using AuthJWT.Domain.Contracts;
using AuthJWT.Domain.DTOs;
using AuthJWT.Domain.DTOs.HotelDto;
using AuthJWT.Domain.Entities.Common;
using AuthJWT.Domain.Entities.Security;
using AutoMapper;

namespace AuthJWT.Infrastructure.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, UserResponse>().ReverseMap();
            CreateMap<ApplicationUser, CurrentUserResponse>().ReverseMap();
            CreateMap<UserRegisterRequest, ApplicationUser>().ReverseMap();
            CreateMap<ApplicationUser, UserProfileResponse>().ReverseMap();
            CreateMap<UpdateUserRequest, ApplicationUser>().ReverseMap();
            CreateMap<Hotel, HotelDto>().ReverseMap();
            CreateMap<Hotel, HotelCreateDto>().ReverseMap();
            CreateMap<Hotel, HotelUpdateDto>().ReverseMap();
            CreateMap<Hotel, HotelResponse>().ReverseMap();

            CreateMap<Rating, RatingDto>().ReverseMap();
            CreateMap<Rating, RatingCreateDto>().ReverseMap();
            CreateMap<Rating, RatingUpdateDto>().ReverseMap();

            CreateMap<Location, LocationDto>().ReverseMap();

            CreateMap<Room, RoomDto>()
                .ForMember(dest => dest.RoomImages, opt => opt.MapFrom(src => src.RoomImages))
                .ForMember(dest => dest.Conveniences, opt => opt.MapFrom(src => src.Conveniences.Select(c => new ConvenienceDto
                {
                    Id = c.ConvenienceId,
                    Name = c.Convenience.Name
                })))

            .ReverseMap();
            CreateMap<Room, RoomCreateDto>().ReverseMap();
            CreateMap<Room, RoomUpdateDto>().ReverseMap();
            CreateMap<Room, RoomResponse>().ReverseMap();
            CreateMap<Room, RoomHotelReponse>()
                .ForMember(dest => dest.Conveniences, opt => opt.MapFrom(src => src.Conveniences.Select(c => new ConvenienceDto
                {
                    Id = c.ConvenienceId,
                    Name = c.Convenience.Name
                })))
                .ReverseMap();

            CreateMap<RoomConvenience, RoomConvenienceDto>().ReverseMap();
            CreateMap<RoomConvenience, RoomConvenienceCreateDto>().ReverseMap();
            CreateMap<RoomImage, RoomImageDto>().ReverseMap();
            CreateMap<RoomImage, RoomImageCreateDto>().ReverseMap();
            CreateMap<RoomImage, RoomImageUpdateDto>().ReverseMap();
            CreateMap<HotelImage, HotelImageDto>().ReverseMap();
            CreateMap<HotelImage, HotelImageCreateDto>().ReverseMap();
            CreateMap<Convenience, ConvenienceDto>().ReverseMap();
            CreateMap<Convenience, ConvenienceCreateDto>().ReverseMap();
            CreateMap<Convenience, ConvenienceUpdateDto>().ReverseMap();
            CreateMap<HotelConvenience, HotelConvenienceDto>().ReverseMap();
            CreateMap<RoomType, RoomTypeDto>().ReverseMap();
            CreateMap<RoomType, RoomTypeCreateDto>().ReverseMap();
            CreateMap<RoomType, RoomTypeUpdateDto>().ReverseMap();
            CreateMap<Policy, PolicyDto>().ReverseMap();
            CreateMap<BookingRoom, BookingRoomDto>().ReverseMap();
            CreateMap<Policy, PolicyDto>().ReverseMap();
            CreateMap<Booking, BookingRoomDto>().ReverseMap();
            CreateMap<Booking, BookingCreateDto>().ReverseMap();
            CreateMap<BookingRoom, BookingRoomCreateDto>().ReverseMap();
            CreateMap<BookingRoom, BookingRoomResponse>()
                .ForMember(dest => dest.Room, opt => opt.MapFrom(src => src.Room))
                .ReverseMap();
            CreateMap<Invoice, InvoiceCreateDto>().ReverseMap();
            CreateMap<Invoice, InvoiceDto>().ReverseMap();

        }
    }
}
