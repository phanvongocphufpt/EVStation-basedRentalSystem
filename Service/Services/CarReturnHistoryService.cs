using AutoMapper;
using Repository.Entities;
using Repository.IRepositories;
using Service.DTOs;
using Service.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class CarReturnHistoryService : ICarReturnHistoryService
    {
        private readonly ICarReturnHistoryRepository _repo;
        private readonly IMapper _mapper;

        public CarReturnHistoryService(ICarReturnHistoryRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CarReturnHistoryDTO>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<CarReturnHistoryDTO>>(list);
        }

        public async Task<CarReturnHistoryDTO?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<CarReturnHistoryDTO>(entity);
        }

        public async Task AddAsync(CarReturnHistoryCreateDTO dto)
        {
            var entity = _mapper.Map<CarReturnHistory>(dto);
            entity.ReturnDate = dto.ReturnDate;
            await _repo.AddAsync(entity);
        }

        public async Task UpdateAsync(int id, CarReturnHistoryCreateDTO dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                throw new Exception("Không tìm thấy lịch sử trả xe.");

            _mapper.Map(dto, existing);
            await _repo.UpdateAsync(existing);
        }

        public async Task DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
        }
    }
}
