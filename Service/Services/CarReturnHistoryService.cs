using AutoMapper;
using Repository.Entities;
using Repository.IRepositories;
using Service.Common;
using Service.DTOs;
using Service.IServices;
using System.Collections.Generic;
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

        public async Task<Result<IEnumerable<CarReturnHistoryDTO>>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            var mapped = _mapper.Map<IEnumerable<CarReturnHistoryDTO>>(list);
            return Result<IEnumerable<CarReturnHistoryDTO>>.Success(mapped, "Lấy danh sách lịch sử trả xe thành công.");
        }

        public async Task<Result<CarReturnHistoryDTO?>> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                return Result<CarReturnHistoryDTO?>.Failure("Không tìm thấy lịch sử trả xe.");

            return Result<CarReturnHistoryDTO?>.Success(_mapper.Map<CarReturnHistoryDTO>(entity), "Lấy thông tin lịch sử trả xe thành công.");
        }

        public async Task<Result<string>> AddAsync(CarReturnHistoryCreateDTO dto)
        {
            var entity = _mapper.Map<CarReturnHistory>(dto);
            entity.ReturnDate = dto.ReturnDate;

            await _repo.AddAsync(entity);
            return Result<string>.Success("OK", "Thêm lịch sử trả xe thành công.");
        }

        public async Task<Result<string>> UpdateAsync(int id, CarReturnHistoryCreateDTO dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return Result<string>.Failure("Không tìm thấy lịch sử trả xe.");

            _mapper.Map(dto, existing);
            await _repo.UpdateAsync(existing);
            return Result<string>.Success("OK", "Cập nhật lịch sử trả xe thành công.");
        }

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
