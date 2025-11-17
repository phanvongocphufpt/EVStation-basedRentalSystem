using AutoMapper;
using Repository.Entities;
using Repository.Entities.Enum;
using Repository.IRepositories;
using Service.Common;
using Service.DTOs;
using Service.IServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Services
{
    public class RentalOrderService : IRentalOrderService
    {
        private readonly IRentalOrderRepository _rentalOrderRepository;
        private readonly IMapper _mapper;

        public RentalOrderService(IRentalOrderRepository rentalOrderRepository, IMapper mapper)
        {
            _rentalOrderRepository = rentalOrderRepository;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<RentalOrderDTO>>> GetAllAsync()
        {
            var orders = await _rentalOrderRepository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<RentalOrderDTO>>(orders);
            return Result<IEnumerable<RentalOrderDTO>>.Success(dtos);
        }

        public async Task<Result<RentalOrderDTO>> GetByIdAsync(int id)
        {
            var order = await _rentalOrderRepository.GetByIdAsync(id);
            if (order == null)
                return Result<RentalOrderDTO>.Failure("Đơn đặt thuê không tồn tại!");
            var dto = _mapper.Map<RentalOrderDTO>(order);
            return Result<RentalOrderDTO>.Success(dto);
        }

        public async Task<Result<IEnumerable<RentalOrderDTO>>> GetByUserIdAsync(int userId)
        {
            var orders = await _rentalOrderRepository.GetByUserIdAsync(userId);
            var dtos = _mapper.Map<IEnumerable<RentalOrderDTO>>(orders);
            return Result<IEnumerable<RentalOrderDTO>>.Success(dtos);
        }

        public async Task<Result<CreateRentalOrderDTO>> CreateAsync(CreateRentalOrderDTO dto)
        {
            var order = _mapper.Map<RentalOrder>(dto);
            order.CreatedAt = DateTime.Now;
            order.Status = RentalOrderStatus.Pending;

            await _rentalOrderRepository.AddAsync(order);
            await _rentalOrderRepository.SaveChangesAsync();

            return Result<CreateRentalOrderDTO>.Success(dto, "Tạo order thành công!");
        }

        public async Task<Result<UpdateRentalOrderTotalDTO>> UpdateTotalAsync(UpdateRentalOrderTotalDTO dto)
        {
            // Giữ logic cũ nếu bạn đã có
            return Result<UpdateRentalOrderTotalDTO>.Success(dto);
        }

        public async Task<Result<bool>> ConfirmPaymentAsync(int orderId)
        {
            // Giữ logic cũ
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> ConfirmTotalAsync(int orderId)
        {
            // Giữ logic cũ
            return Result<bool>.Success(true);
        }

        // ------------------- MỚI -------------------
        public async Task<Result<UpdateRentalOrderStatusDTO>> UpdateStatusAsync(UpdateRentalOrderStatusDTO dto)
        {
            var order = await _rentalOrderRepository.GetByIdAsync(dto.OrderId);
            if (order == null)
                return Result<UpdateRentalOrderStatusDTO>.Failure("Đơn đặt thuê không tồn tại!");

            if (!Enum.IsDefined(typeof(RentalOrderStatus), dto.Status))
                return Result<UpdateRentalOrderStatusDTO>.Failure("Trạng thái không hợp lệ.");

            order.Status = dto.Status;
            order.UpdatedAt = DateTime.Now;

            await _rentalOrderRepository.UpdateAsync(order);
            await _rentalOrderRepository.SaveChangesAsync();

            return Result<UpdateRentalOrderStatusDTO>.Success(dto, "Cập nhật trạng thái thành công!");
        }
    }
}
