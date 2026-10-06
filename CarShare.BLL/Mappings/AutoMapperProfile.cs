using AutoMapper;
using CarShare.BLL.DTOs.Car;
using CarShare.BLL.DTOs.Rental;
using CarShare.BLL.DTOs.User;
using CarShare.DAL.Enums;
using CarShare.DAL.Models;

namespace CarShare.BLL.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // User Mappings
            CreateMap<UserCreateDTO, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordSalt, opt => opt.Ignore());

            CreateMap<User, UserResponseDTO>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

            // Car Mappings
            CreateMap<CarCreateDTO, Car>()
                .ForMember(dest => dest.CarImages, opt => opt.Ignore())
                .ForMember(dest => dest.RentalStatus, opt => opt.MapFrom(_ => RentalStatus.Available))
                .ForMember(dest => dest.IsApproved, opt => opt.MapFrom(_ => false));

            CreateMap<Car, CarResponseDTO>()
                .ForMember(dest => dest.ImageUrls,
                    opt => opt.MapFrom(src => src.CarImages != null ? src.CarImages.Select(img => img.ImageUrl).ToList() : new List<string>()))
                .ForMember(dest => dest.Transmission,
                    opt => opt.MapFrom(src => src.Transmission.ToString()))
                .ForMember(dest => dest.RentalStatus,
                    opt => opt.MapFrom(src => src.RentalStatus.ToString()))
                .ForMember(dest => dest.Year,
                    opt => opt.MapFrom(src => src.Year))
                .ForMember(dest => dest.OwnerName,
                    opt => opt.MapFrom(src => src.Owner != null ? $"{src.Owner.FirstName} {src.Owner.LastName}".Trim() : null));

            // Rental Mappings
            CreateMap<RentalProposalDTO, RentalProposal>()
                .ForMember(dest => dest.Rental, opt => opt.Ignore())
                .ForMember(dest => dest.Car, opt => opt.Ignore())
                .ForMember(dest => dest.Renter, opt => opt.Ignore());

            CreateMap<RentalProposal, RentalResponseDTO>()
                .ForMember(dest => dest.CarTitle,
                    opt => opt.MapFrom(src => src.Car != null ? src.Car.Title : null))
                .ForMember(dest => dest.RenterName,
                    opt => opt.MapFrom(src => src.Renter != null ? $"{src.Renter.FirstName} {src.Renter.LastName}".Trim() : null))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.LicenseVerificationUrl,
                    opt => opt.MapFrom(src => src.LicenseVerificationUrl))
                .ForMember(dest => dest.AdditionalDocumentsUrl,
                    opt => opt.MapFrom(src => src.AdditionalDocumentsUrl))
                .ForMember(dest => dest.Message,
                    opt => opt.MapFrom(src => src.Message));
        }
    }
}