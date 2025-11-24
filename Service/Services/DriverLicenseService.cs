using AutoMapper;
using Repository.Entities;
using Repository.Entities.Enum;
using Repository.IRepositories;
using Repository.Repositories;
using Service.Common;
using Service.DTOs;
using Service.EmailConfirmation;
using Service.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class DriverLicenseService : IDriverLicenseService
    {
        private readonly IDriverLicenseRepository _driverLicenseRepository;
        private readonly IRentalOrderRepository _rentalOrderRepository;
        private readonly EmailService _emailService;
        private readonly IMapper _mapper;
        public DriverLicenseService(IDriverLicenseRepository driverLicenseRepository, IMapper mapper, IRentalOrderRepository rentalOrderRepository, EmailService emailService)
        {
            _driverLicenseRepository = driverLicenseRepository;
            _mapper = mapper;
            _rentalOrderRepository = rentalOrderRepository;
            _emailService = emailService;
        }
        public async Task<Result<IEnumerable<DriverLicenseDTO>>> GetAllAsync()
        {
            var driverLicenses = await _driverLicenseRepository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<DriverLicenseDTO>>(driverLicenses);
            return Result<IEnumerable<DriverLicenseDTO>>.Success(dtos);
        }
        public async Task<Result<DriverLicenseDTO>> GetByIdAsync(int id)
        {
            var driverLicense = await _driverLicenseRepository.GetByIdAsync(id);
            if (driverLicense == null)
            {
                return Result<DriverLicenseDTO>.Failure("Giấy phép lái xe không tồn tại! Kiểm tra lại Id.");
            }
            var dto = _mapper.Map<DriverLicenseDTO>(driverLicense);
            return Result<DriverLicenseDTO>.Success(dto);
        }
        public async Task<Result<DriverLicenseDTO>> GetByOrderIdAsync(int id)
        {
            var driverLicense = await _driverLicenseRepository.GetByOrderIdAsync(id);
            if (driverLicense == null)
            {
                return Result<DriverLicenseDTO>.Failure("Giấy phép lái xe không tồn tại cho Order này! Kiểm tra lại OrderId.");
            }
            var dto = _mapper.Map<DriverLicenseDTO>(driverLicense);
            return Result<DriverLicenseDTO>.Success(dto);
        }
        public async Task<Result<CreateDriverLicenseDTO>> CreateAsync(CreateDriverLicenseDTO createDriverLicenseDTO)
        {
            var dto = _mapper.Map<DriverLicense>(createDriverLicenseDTO);
            var order = await _rentalOrderRepository.GetByIdAsync(dto.RentalOrderId);
            if (order == null)
            {
                return Result<CreateDriverLicenseDTO>.Failure("Order không tồn tại! Kiểm tra lại Id của Order.");
            }
            if (order.WithDriver == true)
            {
                return Result<CreateDriverLicenseDTO>.Failure("Order có tài xế, không cần giấy phép lái xe!");
            }
            if (order.DriverLicense != null)
            {
                return Result<CreateDriverLicenseDTO>.Failure("Order đã có giấy phép lái xe rồi!");
            }
            var driverLicense = new DriverLicense
            {
                Name = dto.Name,
                LicenseNumber = dto.LicenseNumber,
                ImageUrl = dto.ImageUrl,
                ImageUrl2 = dto.ImageUrl2,
                Status = DocumentStatus.Pending,
                CreatedAt = DateTime.Now,
                RentalOrderId = order.Id,
                RentalOrder = order
            };
            await _driverLicenseRepository.AddAsync(driverLicense);
            if (order.CitizenId.HasValue)
            {
                order.Status = RentalOrderStatus.Pending;
                await _rentalOrderRepository.UpdateAsync(order);
            }
            return Result<CreateDriverLicenseDTO>.Success(createDriverLicenseDTO, "Tạo giấy phép lái xe thành công.");
        }
        public async Task<Result<UpdateDriverLicenseStatusDTO>> UpdateStatusAsync(UpdateDriverLicenseStatusDTO driverLicenseDTO)
        {
            var existingDriverLicense = await _driverLicenseRepository.GetByIdAsync(driverLicenseDTO.DriverLicenseId);
            if (existingDriverLicense == null)
            {
                return Result<UpdateDriverLicenseStatusDTO>.Failure("Giấy phép lái xe không tồn tại! Kiểm tra lại Id.");
            }
            var order = await _rentalOrderRepository.GetByIdAsync(existingDriverLicense.RentalOrderId);
            if (order == null)
            {
                return Result<UpdateDriverLicenseStatusDTO>.Failure("Order không tồn tại! Kiểm tra lại Id của order.");
            }
            existingDriverLicense.Status = driverLicenseDTO.Status;
            existingDriverLicense.UpdatedAt = DateTime.Now;
            await _driverLicenseRepository.UpdateAsync(existingDriverLicense);
            
            // Reload order với navigation properties để kiểm tra trạng thái cả 2 giấy tờ
            order = await _rentalOrderRepository.GetByIdAsync(existingDriverLicense.RentalOrderId);
            
            // Kiểm tra nếu cả 2 giấy tờ đều được duyệt và order đang ở trạng thái Pending
            if (order.CitizenId.HasValue && 
                order.CitizenIdNavigation != null &&
                order.DriverLicense != null &&
                order.CitizenIdNavigation.Status == DocumentStatus.Approved && 
                order.DriverLicense.Status == DocumentStatus.Approved && 
                order.Status == RentalOrderStatus.Pending)
            {
                // Tự động chuyển trạng thái order sang Pending (chờ thanh toán deposit)
                order.Status = RentalOrderStatus.Pending;
                order.UpdatedAt = DateTime.Now;
                await _rentalOrderRepository.UpdateAsync(order);
                
                // Gửi email thông báo
                await _emailService.SendRemindEmail(order.User.Email, order);
            }
            return Result<UpdateDriverLicenseStatusDTO>.Success(driverLicenseDTO, "Cập nhật trạng thái giấy phép lái xe thành công.");
        }
        public async Task<Result<UpdateDriverLicenseInfoDTO>> UpdateInfoAsync(UpdateDriverLicenseInfoDTO driverLicenseDTO)
        {
            var existingDriverLicense = await _driverLicenseRepository.GetByIdAsync(driverLicenseDTO.Id);
            if (existingDriverLicense == null)
            {
                return Result<UpdateDriverLicenseInfoDTO>.Failure("Giấy phép lái xe không tồn tại! Kiểm tra lại Id.");
            }
            existingDriverLicense.Name = driverLicenseDTO.Name;
            existingDriverLicense.LicenseNumber = driverLicenseDTO.LicenseNumber;
            existingDriverLicense.ImageUrl = driverLicenseDTO.ImageUrl;
            existingDriverLicense.ImageUrl2 = driverLicenseDTO.ImageUrl2;
            existingDriverLicense.Status = DocumentStatus.Pending;
            existingDriverLicense.UpdatedAt = DateTime.Now;
            await _driverLicenseRepository.UpdateAsync(existingDriverLicense);
            return Result<UpdateDriverLicenseInfoDTO>.Success(driverLicenseDTO, "Cập nhật thông tin giấy phép lái xe thành công.");
        }
    }
}
