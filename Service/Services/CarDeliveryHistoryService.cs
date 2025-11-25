using AutoMapper;
using Repository.Entities;
using Repository.Entities.Enum;
using Repository.IRepositories;
using Service.DTOs;
using Service.IServices;
using Service.Common;

namespace Service.Services
{
    public class CarDeliveryHistoryService : ICarDeliveryHistoryService
    {
        private readonly ICarDeliveryHistoryRepository _repo;
        private readonly IRentalOrderRepository _rentalOrderRepo;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;

        public CarDeliveryHistoryService(
            ICarDeliveryHistoryRepository repo,
            IRentalOrderRepository rentalOrderRepo,
            IMapper mapper,
            IUserRepository userRepo)
        {
            _repo = repo;
            _rentalOrderRepo = rentalOrderRepo;
            _mapper = mapper;
            _userRepo = userRepo;
        }

        public async Task<Result<(IEnumerable<CarDeliveryHistoryDTO> Data, int Total)>> GetAllAsync(int pageIndex, int pageSize)
        {
            try
            {
                var list = await _repo.GetAllAsync(pageIndex, pageSize);
                var total = await _repo.CountAsync();

                var mapped = _mapper.Map<IEnumerable<CarDeliveryHistoryDTO>>(list);
                return Result<(IEnumerable<CarDeliveryHistoryDTO>, int)>.Success((mapped, total), "Lấy danh sách lịch sử giao xe thành công.");
            }
            catch (Exception ex)
            {
                return Result<(IEnumerable<CarDeliveryHistoryDTO>, int)>.Failure($"Lỗi khi lấy danh sách: {ex.Message}");
            }
        }

        // 🔹 Lấy 1 bản ghi theo ID
        public async Task<Result<CarDeliveryHistoryDTO?>> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _repo.GetByIdAsync(id);
                if (entity == null)
                    return Result<CarDeliveryHistoryDTO?>.Failure("Không tìm thấy lịch sử giao xe.");

                return Result<CarDeliveryHistoryDTO?>.Success(_mapper.Map<CarDeliveryHistoryDTO>(entity), "Lấy thông tin thành công.");
            }
            catch (Exception ex)
            {
                return Result<CarDeliveryHistoryDTO?>.Failure($"Lỗi khi lấy dữ liệu: {ex.Message}");
            }
        }

        public async Task<Result<string>> AddAsync(CarDeliveryHistoryCreateDTO dto)
        {
            try
            {
                var order = await _rentalOrderRepo.GetByIdAsync(dto.OrderId);
                if (order == null)
                    return Result<string>.Failure("Không tìm thấy đơn thuê.");
                var user = await _userRepo.GetByIdAsync(order.UserId);
                if (user.DriverLicense == null || user.DriverLicense.Status != DocumentStatus.Approved)
                {
                    return Result<string>.Failure("Người dùng chưa có hoặc chưa xác thực thông tin giấy phép lái xe.");
                }
                if (order.Status != RentalOrderStatus.OrderDepositConfirmed)
                    return Result<string>.Failure("Chỉ có thể giao xe cho các đơn thuê ở trạng thái 'OrderDepositConfirmed'.");

                var history = new CarDeliveryHistory
                {
                    DeliveryDate = DateTime.Now,
                    OdometerStart = dto.OdometerStart,
                    BatteryLevelStart = dto.BatteryLevelStart,
                    VehicleConditionStart = dto.VehicleConditionStart,
                    ImageUrl = dto.ImageUrl,
                    ImageUrl2 = dto.ImageUrl2,
                    ImageUrl3 = dto.ImageUrl3,
                    ImageUrl4 = dto.ImageUrl4,
                    ImageUrl5 = dto.ImageUrl5,
                    ImageUrl6 = dto.ImageUrl6,
                    OrderId = order.Id,
                    CarId = order.CarId,
                };

                await _repo.AddAsync(history);

                order.Status = RentalOrderStatus.CheckedIn;
                await _rentalOrderRepo.UpdateAsync(order);

                return Result<string>.Success("Giao xe thành công. Trạng thái đơn đã chuyển sang 'CheckedIn'.", "Giao xe thành công. Trạng thái đơn đã chuyển sang 'CheckedIn'.");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"Giao xe thất bại: {ex.Message}");
            }
        }

        public async Task<Result<string>> UpdateAsync(int id, CarDeliveryHistoryCreateDTO dto)
        {
            try
            {
                var entity = await _repo.GetByIdAsync(id);
                if (entity == null)
                    return Result<string>.Failure("Không tìm thấy lịch sử giao xe cần cập nhật.");

                _mapper.Map(dto, entity);
                await _repo.UpdateAsync(entity);

                return Result<string>.Success("Cập nhật lịch sử giao xe thành công.");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"Lỗi khi cập nhật: {ex.Message}");
            }
        }

        public async Task<Result<string>> DeleteAsync(int id)
        {
            try
            {
                var entity = await _repo.GetByIdAsync(id);
                if (entity == null)
                    return Result<string>.Failure("Không tìm thấy lịch sử giao xe cần xóa.");

                await _repo.DeleteAsync(id);
                return Result<string>.Success("Xóa lịch sử giao xe thành công.");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"Lỗi khi xóa: {ex.Message}");
            }
        }
    }
}
