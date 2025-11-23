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
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.RentalLocationId, opt => opt.MapFrom(src => src.RentalLocationId))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdateAt, opt => opt.MapFrom(src => src.UpdatedAt));
            CreateMap<CreateStaffUserDTO, User>();

            // Payment Mappings
            CreateMap<Payment, PaymentDTO>()
                .ForMember(dest => dest.PaymentId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PaymentDate, opt => opt.MapFrom(src => src.PaymentDate ?? DateTime.MinValue))
                .ForMember(dest => dest.PaymentType, opt => opt.MapFrom(src => src.PaymentType.ToString()))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod ?? string.Empty))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.HasValue ? src.UserId.Value.ToString() : string.Empty))
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.RentalOrderId.HasValue ? src.RentalOrderId.Value.ToString() : string.Empty))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.RentalOrder != null ? src.RentalOrder.OrderDate : DateTime.MinValue));

            // Payment to PaymentDetailDTO mapping (includes MoMo fields)
            CreateMap<Payment, PaymentDetailDTO>()
                .ForMember(dest => dest.PaymentId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PaymentType, opt => opt.MapFrom(src => src.PaymentType))
                .ForMember(dest => dest.PaymentDate, opt => opt.MapFrom(src => src.PaymentDate))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
                .ForMember(dest => dest.BillingImageUrl, opt => opt.MapFrom(src => src.BillingImageUrl))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.RentalOrderId, opt => opt.MapFrom(src => src.RentalOrderId))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User != null ? new UserInfoDTO
                {
                    UserId = src.User.Id,
                    Email = src.User.Email,
                    FullName = src.User.FullName,
                    Role = src.User.Role
                } : null))
                .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.RentalOrder != null ? new OrderInfoDTO
                {
                    OrderId = src.RentalOrder.Id,
                    OrderDate = src.RentalOrder.OrderDate,
                    PickupTime = src.RentalOrder.PickupTime,
                    ExpectedReturnTime = src.RentalOrder.ExpectedReturnTime,
                    ActualReturnTime = src.RentalOrder.ActualReturnTime,
                    Total = src.RentalOrder.Total
                } : null))
                // ===== Map MoMo fields =====
                .ForMember(dest => dest.MomoOrderId, opt => opt.MapFrom(src => src.MomoOrderId))
                .ForMember(dest => dest.MomoRequestId, opt => opt.MapFrom(src => src.MomoRequestId))
                .ForMember(dest => dest.MomoPartnerCode, opt => opt.MapFrom(src => src.MomoPartnerCode))
                .ForMember(dest => dest.MomoTransId, opt => opt.MapFrom(src => src.MomoTransId))
                .ForMember(dest => dest.MomoResultCode, opt => opt.MapFrom(src => src.MomoResultCode))
                .ForMember(dest => dest.MomoPayType, opt => opt.MapFrom(src => src.MomoPayType))
                .ForMember(dest => dest.MomoMessage, opt => opt.MapFrom(src => src.MomoMessage))
                .ForMember(dest => dest.MomoSignature, opt => opt.MapFrom(src => src.MomoSignature))
// ===== Map PayOS fields =====
// Bổ sung ignore các trường PayOS
.ForMember(dest => dest.PayOSOrderCode, opt => opt.Ignore())
.ForMember(dest => dest.PayOSTransactionId, opt => opt.Ignore())
.ForMember(dest => dest.PayOSAccountNumber, opt => opt.Ignore())
.ForMember(dest => dest.PayOSChecksum, opt => opt.Ignore())
.ForMember(dest => dest.PayOSCheckoutUrl, opt => opt.Ignore())
.ForMember(dest => dest.PayOSQrCode, opt => opt.Ignore());


            // Mapping từ CreatePaymentDTO → Payment
            CreateMap<CreatePaymentDTO, Payment>()
                .ForMember(dest => dest.PaymentDate, opt => opt.MapFrom(src => src.PaymentDate ?? DateTime.Now))
                .ForMember(dest => dest.PaymentType, opt => opt.MapFrom(src => src.PaymentType))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
                .ForMember(dest => dest.BillingImageUrl, opt => opt.MapFrom(src => src.BillingImageUrl))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.RentalOrderId, opt => opt.MapFrom(src => src.RentalOrderId))
                // Ignore các trường MoMo và các trường khác không có trong CreatePaymentDTO
                .ForMember(dest => dest.MomoOrderId, opt => opt.Ignore())
                .ForMember(dest => dest.MomoRequestId, opt => opt.Ignore())
                .ForMember(dest => dest.MomoPartnerCode, opt => opt.Ignore())
                .ForMember(dest => dest.MomoTransId, opt => opt.Ignore())
                .ForMember(dest => dest.MomoResultCode, opt => opt.Ignore())
                .ForMember(dest => dest.MomoPayType, opt => opt.Ignore())
                .ForMember(dest => dest.MomoMessage, opt => opt.Ignore())
                .ForMember(dest => dest.MomoSignature, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.RentalOrder, opt => opt.Ignore());


            //RentalLocation Mappings
            CreateMap<RentalLocation, RentalLocationDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));
            CreateMap<CreateRentalLocationDTO, RentalLocation>();

            //CarDeliveryHistory Mappings
            CreateMap<CarDeliveryHistory, CarDeliveryHistoryDTO>().ReverseMap();
            CreateMap<CarDeliveryHistoryCreateDTO, CarDeliveryHistory>();
            CreateMap<CarDeliveryHistoryUpdateDTO, CarDeliveryHistory>();

            //CarRentalLocation Mappings
            CreateMap<CarRentalLocation, CarRentalLocationDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.CarId, opt => opt.MapFrom(src => src.CarId))
                .ForMember(dest => dest.LocationId, opt => opt.MapFrom(src => src.LocationId));
            //CarReturnHistory Mappings
            CreateMap<CreateCarRentalLocationDTO, CarRentalLocation>();
            CreateMap<CarReturnHistory, CarReturnHistoryDTO>().ReverseMap();
            CreateMap<CarReturnHistory, CarReturnHistoryCreateDTO>().ReverseMap();

            //CitizenId Mappings
            CreateMap<CitizenId, CitizenIdDTO>().ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.CitizenIdNumber, opt => opt.MapFrom(src => src.CitizenIdNumber))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.ImageUrl2, opt => opt.MapFrom(src => src.ImageUrl2))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.RentalOrderId, opt => opt.MapFrom(src => src.RentalOrderId));
            CreateMap<CreateCitizenIdDTO, CitizenId>();

            //DriverLicense Mappings
            CreateMap<DriverLicense, DriverLicenseDTO>().ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.LicenseNumber, opt => opt.MapFrom(src => src.LicenseNumber))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.ImageUrl2, opt => opt.MapFrom(src => src.ImageUrl2))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.RentalOrderId, opt => opt.MapFrom(src => src.RentalOrderId));
            CreateMap<CreateDriverLicenseDTO, DriverLicense>();

            //RentalOrder Mappings
            CreateMap<RentalOrder, RentalOrderDTO>().ReverseMap()
                .ForMember(dest => dest.Id, otp => otp.MapFrom(src => src))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
                .ForMember(dest => dest.PickupTime, opt => opt.MapFrom(src => src.PickupTime))
                .ForMember(dest => dest.ExpectedReturnTime, opt => opt.MapFrom(src => src.ExpectedReturnTime))
                .ForMember(dest => dest.ActualReturnTime, opt => opt.MapFrom(src => src.ActualReturnTime))
                .ForMember(dest => dest.SubTotal, opt => opt.MapFrom(src => src.SubTotal))
                .ForMember(dest => dest.Deposit, opt => opt.MapFrom(src => src.Deposit))
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total))
                .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount))
                .ForMember(dest => dest.ExtraFee, opt => opt.MapFrom(src => src.ExtraFee))
                .ForMember(dest => dest.DamageFee, opt => opt.MapFrom(src => src.DamageFee))
                .ForMember(dest => dest.DamageNotes, opt => opt.MapFrom(src => src.DamageNotes))
                .ForMember(dest => dest.WithDriver, opt => opt.MapFrom(src => src.WithDriver))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.CarId, opt => opt.MapFrom(src => src.CarId))
                .ForMember(dest => dest.RentalLocationId, opt => opt.MapFrom(src => src.RentalLocationId))
                .ForMember(dest => dest.CitizenId, opt => opt.MapFrom(src => src.CitizenId))
                .ForMember(dest => dest.DriverLicenseId, opt => opt.MapFrom(src => src.DriverLicenseId))
                .ForMember(dest => dest.RentalContactId, opt => opt.MapFrom(src => src.RentalContactId));
            CreateMap<RentalOrder, RentalOrderWithDetailsDTO>().ReverseMap()
    .ForMember(dest => dest.Id, otp => otp.MapFrom(src => src))
    .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
    .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
    .ForMember(dest => dest.PickupTime, opt => opt.MapFrom(src => src.PickupTime))
    .ForMember(dest => dest.ExpectedReturnTime, opt => opt.MapFrom(src => src.ExpectedReturnTime))
    .ForMember(dest => dest.ActualReturnTime, opt => opt.MapFrom(src => src.ActualReturnTime))
    .ForMember(dest => dest.SubTotal, opt => opt.MapFrom(src => src.SubTotal))
    .ForMember(dest => dest.Deposit, opt => opt.MapFrom(src => src.Deposit))
    .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total))
    .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount))
    .ForMember(dest => dest.ExtraFee, opt => opt.MapFrom(src => src.ExtraFee))
    .ForMember(dest => dest.DamageFee, opt => opt.MapFrom(src => src.DamageFee))
    .ForMember(dest => dest.DamageNotes, opt => opt.MapFrom(src => src.DamageNotes))
    .ForMember(dest => dest.WithDriver, opt => opt.MapFrom(src => src.WithDriver))
    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
    .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
    .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
    .ForMember(dest => dest.CarId, opt => opt.MapFrom(src => src.CarId))
    .ForMember(dest => dest.RentalLocationId, opt => opt.MapFrom(src => src.RentalLocationId))
    .ForMember(dest => dest.CitizenId, opt => opt.MapFrom(src => src.CitizenId))
    .ForMember(dest => dest.DriverLicenseId, opt => opt.MapFrom(src => src.DriverLicenseId))
    .ForMember(dest => dest.RentalContactId, opt => opt.MapFrom(src => src.RentalContactId))
    .ForMember(dest => dest.Payments, opt => opt.MapFrom(src => src.Payments));
            CreateMap<CreateRentalOrderDTO, RentalOrder>();
            //feedback mappings
            CreateMap<Feedback, FeedbackDTO>()
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User.FullName));

            // DTO -> Entity
            CreateMap<CreateFeedbackDTO, Feedback>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

            CreateMap<UpdateFeedbackDTO, Feedback>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.Now));

            //Rental Contact Mappings
            CreateMap<RentalContact, RentalContactDTO>()
                    .ForMember(dest => dest.RentalOrderId, opt => opt.MapFrom(src => src.RentalOrderId))
                    .ForMember(dest => dest.RentalDate, opt => opt.MapFrom(src => src.RentalDate))
                    .ForMember(dest => dest.RentalPeriod, opt => opt.MapFrom(src => src.RentalPeriod))
                    .ForMember(dest => dest.ReturnDate, opt => opt.MapFrom(src => src.ReturnDate))
                    .ForMember(dest => dest.TerminationClause, opt => opt.MapFrom(src => src.TerminationClause))
                    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                    .ForMember(dest => dest.LesseeId, opt => opt.MapFrom(src => src.LesseeId))
                    .ForMember(dest => dest.LessorId, opt => opt.MapFrom(src => src.LessorId))
                    .ReverseMap();
            CreateMap<RentalContactCreateDTO, RentalContact>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // tránh map ID thủ công
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));
            CreateMap<RentalContactUpdateDTO, RentalContact>()

                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));


        }
    }
}
