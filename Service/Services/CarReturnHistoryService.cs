using AutoMapper;
using Repository.Entities;
using Repository.Entities.Enum;
using Repository.IRepositories;
using Service.Common;
using Service.DTOs;
using Service.IServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Service.Services
{
    public class CarReturnHistoryService : ICarReturnHistoryService
    {
        private readonly ICarReturnHistoryRepository _repo;
        private readonly IRentalOrderRepository _rentalOrderRepo;
        private readonly IMapper _mapper;

        public CarReturnHistoryService(
            ICarReturnHistoryRepository repo,
            IRentalOrderRepository rentalOrderRepo,
            IMapper mapper)
        {
            _repo = repo;
            _rentalOrderRepo = rentalOrderRepo;
            _mapper = mapper;
        }

        // 🔹 Lấy tất cả
        public async Task<Result<IEnumerable<CarReturnHistoryDTO>>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            var mapped = _mapper.Map<IEnumerable<CarReturnHistoryDTO>>(list);
            return Result<IEnumerable<CarReturnHistoryDTO>>.Success(mapped, "Lấy danh sách lịch sử trả xe thành công.");
        }

        // 🔹 Lấy 1 bản ghi theo Id
        public async Task<Result<CarReturnHistoryDTO?>> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                return Result<CarReturnHistoryDTO?>.Failure("Không tìm thấy lịch sử trả xe.");

            return Result<CarReturnHistoryDTO?>.Success(_mapper.Map<CarReturnHistoryDTO>(entity));
        }

        // 🔹 Lấy thông tin đầy đủ theo OrderId
        public async Task<Result<CarReturnHistoryDTO?>> GetByOrderIdAsync(int orderId)
        {
            try
            {
                var entity = await _repo.GetByOrderIdAsync(orderId);
                if (entity == null)
                    return Result<CarReturnHistoryDTO?>.Failure("Không tìm thấy lịch sử trả xe với OrderId này.");

                return Result<CarReturnHistoryDTO?>.Success(_mapper.Map<CarReturnHistoryDTO>(entity), "Lấy thông tin lịch sử trả xe theo OrderId thành công.");
            }
            catch (Exception ex)
            {
                return Result<CarReturnHistoryDTO?>.Failure($"Lỗi khi lấy dữ liệu: {ex.Message}");
            }
        }

        public async Task<Result<string>> AddAsync(CarReturnHistoryCreateDTO dto)
        {

            try
            {
                var order = await _rentalOrderRepo.GetByIdAsync(dto.OrderId);
                if (order == null)
                    return Result<string>.Failure("Không tìm thấy đơn hàng để trả xe.");
                if (order.Status != RentalOrderStatus.Renting)
                    return Result<string>.Failure("Đơn hàng không ở trạng thái 'Renting', không thể trả xe.");

                order.ActualReturnTime = DateTime.Now;
                await _rentalOrderRepo.UpdateAsync(order);

                var entity = new CarReturnHistory
                {
                    ReturnDate = DateTime.Now,
                    OdometerEnd = dto.OdometerEnd,
                    BatteryLevelEnd = dto.BatteryLevelEnd,
                    VehicleConditionEnd = dto.VehicleConditionEnd,
                    ImageUrl = dto.ImageUrl,
                    ImageUrl2 = dto.ImageUrl2,
                    ImageUrl3 = dto.ImageUrl3,
                    ImageUrl4 = dto.ImageUrl4,
                    ImageUrl5 = dto.ImageUrl5,
                    ImageUrl6 = dto.ImageUrl6,
                    OrderId = dto.OrderId,
                    CarId = order.CarId       
                };
                await _repo.AddAsync(entity);
                order.Status = RentalOrderStatus.Returned;
                await _rentalOrderRepo.UpdateAsync(order);
                return Result<string>.Success("OK", "Trả xe thành công, đơn hàng đã cập nhật sang trạng thái 'Returned'.");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"Trả xe thất bại: {ex.Message}");
            }
        }

        // 🔹 Cập nhật (bổ sung, không xóa dữ liệu cũ)
        public async Task<Result<string>> UpdateAsync(int id, CarReturnHistoryCreateDTO dto)
        {
            try
            {
                var existing = await _repo.GetByIdAsync(id);
                if (existing == null)
                    return Result<string>.Failure("Không tìm thấy lịch sử trả xe.");

                // Chỉ cập nhật các trường được cung cấp, không xóa dữ liệu cũ
                if (dto.OdometerEnd > 0)
                    existing.OdometerEnd = dto.OdometerEnd;
                
                if (dto.BatteryLevelEnd >= 0)
                    existing.BatteryLevelEnd = dto.BatteryLevelEnd;
                
                if (!string.IsNullOrWhiteSpace(dto.VehicleConditionEnd))
                    existing.VehicleConditionEnd = dto.VehicleConditionEnd;

                // Bổ sung ImageUrl nếu có, không ghi đè nếu đã có giá trị
                if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
                    existing.ImageUrl = dto.ImageUrl;
                
                if (!string.IsNullOrWhiteSpace(dto.ImageUrl2))
                    existing.ImageUrl2 = dto.ImageUrl2;
                
                if (!string.IsNullOrWhiteSpace(dto.ImageUrl3))
                    existing.ImageUrl3 = dto.ImageUrl3;
                
                if (!string.IsNullOrWhiteSpace(dto.ImageUrl4))
                    existing.ImageUrl4 = dto.ImageUrl4;
                
                if (!string.IsNullOrWhiteSpace(dto.ImageUrl5))
                    existing.ImageUrl5 = dto.ImageUrl5;
                
                if (!string.IsNullOrWhiteSpace(dto.ImageUrl6))
                    existing.ImageUrl6 = dto.ImageUrl6;

                existing.UpdateAt = DateTime.Now;
                await _repo.UpdateAsync(existing);
                return Result<string>.Success("OK", "Cập nhật lịch sử trả xe thành công.");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"Lỗi khi cập nhật: {ex.Message}");
            }
        }

        // 🔹 Xóa
        public async Task<Result<string>> DeleteAsync(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return Result<string>.Failure("Không tìm thấy lịch sử trả xe để xóa.");

            await _repo.DeleteAsync(id);
            return Result<string>.Success("OK", "Xóa lịch sử trả xe thành công.");
        }
    }
}
