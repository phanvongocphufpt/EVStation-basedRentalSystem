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
        private readonly IMapper _mapper;

        public CarDeliveryHistoryService(ICarDeliveryHistoryRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CarDeliveryHistoryDTO>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<CarDeliveryHistoryDTO>>(list);
        }

        public async Task<CarDeliveryHistoryDTO?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return _mapper.Map<CarDeliveryHistoryDTO>(entity);
        }

      

        public async Task AddAsync(CarDeliveryHistoryCreateDTO dto)
        {
            var entity = _mapper.Map<CarDeliveryHistory>(dto);
            await _repo.AddAsync(entity);
        }

        public async Task UpdateAsync(int id, CarDeliveryHistoryCreateDTO dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) throw new Exception("Not found");

            _mapper.Map(dto, existing);
            await _repo.UpdateAsync(existing);
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) throw new Exception("Not found");

            await _repo.DeleteAsync(existing);
        }
    }
}
