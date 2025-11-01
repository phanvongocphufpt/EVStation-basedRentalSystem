using AutoMapper;
using Repository.Entities;
using Repository.IRepositories;
using Service.DTOs;
using Service.IServices;

namespace Service.Services
{
    public class CarDeliveryHistoryService : ICarDeliveryHistoryService
    {
        private readonly ICarDeliveryHistoryRepository _repo;
        private readonly ICarRentalLocationRepository _carRentalLocationRepo;
        private readonly IMapper _mapper;

        public CarDeliveryHistoryService(
            ICarDeliveryHistoryRepository repo,
            ICarRentalLocationRepository carRentalLocationRepo,
            IMapper mapper)
        {
            _repo = repo;
            _carRentalLocationRepo = carRentalLocationRepo;
            _mapper = mapper;
        }

        public async Task<(IEnumerable<CarDeliveryHistoryDTO> data, int total)> GetAllAsync(int pageIndex, int pageSize)
        {
            var list = await _repo.GetAllAsync(pageIndex, pageSize);
            var total = await _repo.CountAsync();
            return (_mapper.Map<IEnumerable<CarDeliveryHistoryDTO>>(list), total);
        }

        public async Task<CarDeliveryHistoryDTO?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<CarDeliveryHistoryDTO>(entity);
        }

        public async Task AddAsync(CarDeliveryHistoryCreateDTO dto)
        {
            // 🔹 Kiểm tra xem xe có tồn tại tại chi nhánh này không
            var carRentalLocation = await _carRentalLocationRepo
                .GetByCarAndRentalLocationIdAsync(dto.CarId, dto.LocationId);

            if (carRentalLocation == null)
                throw new Exception("Không tìm thấy xe tại chi nhánh này.");

            if (carRentalLocation.Quantity <= 0)
                throw new Exception("Chi nhánh này không còn xe khả dụng để giao.");

            using var transaction = await _carRentalLocationRepo.BeginTransactionAsync();
            try
            {
                // 🔹 Giảm số lượng xe ở chi nhánh khi giao
                carRentalLocation.Quantity -= 1;
                await _carRentalLocationRepo.UpdateAsync(carRentalLocation);

                // 🔹 Lưu lịch sử giao xe
                var history = new CarDeliveryHistory
                {
                    DeliveryDate = dto.DeliveryDate,
                    OdometerStart = dto.OdometerStart,
                    BatteryLevelStart = dto.BatteryLevelStart,
                    VehicleConditionStart = dto.VehicleConditionStart,
                    OrderId = dto.OrderId,
                    CustomerId = dto.CustomerId,
                    StaffId = dto.StaffId,
                    CarId = dto.CarId,
                    LocationId = dto.LocationId, 
                };


                await _repo.AddAsync(history);
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateAsync(int id, CarDeliveryHistoryCreateDTO dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Không tìm thấy lịch sử giao xe.");

            _mapper.Map(dto, entity);
            await _repo.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
        }
    }
}
