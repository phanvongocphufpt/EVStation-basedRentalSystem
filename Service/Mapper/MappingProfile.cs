using AutoMapper;
using Repository.Entities;
using Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //User Mappings
            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.DriverLicenseId, opt => opt.MapFrom(src => src.DriverLicenseId))
                .ForMember(dest => dest.CitizenId, opt => opt.MapFrom(src => src.CitizenId));
            CreateMap<CreateStaffUserDTO, User>();

            //Payment Mappings
            CreateMap<Payment, PaymentDTO>()
                .ForMember(dest => dest.PaymentId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PaymentDate, opt => opt.MapFrom(src => src.PaymentDate))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.UserFullname, opt => opt.MapFrom(src => src.RentalOrder.User.FullName))
                .ForMember(dest => dest.Car, opt => opt.MapFrom(src => src.RentalOrder.Car.Name))
                .ForMember(dest => dest.OrderDate, otp => otp.MapFrom(src => src.RentalOrder.OrderDate));
            CreateMap<CreatePaymentDTO, Payment>();

            //RentalLocation Mappings
            CreateMap<RentalLocation, RentalLocationDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));
            CreateMap<CreateRentalLocationDTO, RentalLocation>();

<<<<<<< HEAD
            //CarDeliveryHistory Mappings
            CreateMap<CarDeliveryHistory, CarDeliveryHistoryDTO>().ReverseMap();
            CreateMap<CarDeliveryHistoryCreateDTO, CarDeliveryHistory>();
            CreateMap<CarDeliveryHistoryUpdateDTO, CarDeliveryHistory>();
=======
            //CarRentalLocation Mappings
            CreateMap<CarRentalLocation, CarRentalLocationDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.CarId, opt => opt.MapFrom(src => src.CarId))
                .ForMember(dest => dest.LocationId, opt => opt.MapFrom(src => src.LocationId));
            CreateMap<CreateCarRentalLocationDTO, CarRentalLocation>();
>>>>>>> main
        }
    }
}
