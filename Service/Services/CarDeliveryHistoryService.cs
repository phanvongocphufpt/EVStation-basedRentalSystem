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
        private readonly IRentalLocationRepository _carRentalLocationRepo;
        private readonly IMapper _mapper;

        public CarDeliveryHistoryService(
            ICarDeliveryHistoryRepository repo,
            IRentalLocationRepository carRentalLocationRepo,
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
            //// 🔹 Kiểm tra tồn tại xe tại địa điểm
            //// Sau khi thêm CarDeliveryHistory thành công
            //var carRentalLocation = await _carRentalLocationRepo
            //    .GetAllAsync();

            //if (carRentalLocation == null)
            //    throw new Exception("Không tìm thấy mối quan hệ xe và chi nhánh");

            //if (carRentalLocation.Quantity <= 0)
            //    throw new Exception("Chi nhánh này không còn xe khả dụng");

            //carRentalLocation.Quantity -= 1;
            //await _carRentalLocationRepository.UpdateAsync(carRentalLocation);

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
