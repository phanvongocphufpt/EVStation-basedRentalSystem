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
    public class RentalContactService : IRentalContactService
    {
        private readonly IRentalContactRepository _repo;
        private readonly IMapper _mapper;

        public RentalContactService(IRentalContactRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        // 🔹 Lấy tất cả hợp đồng thuê
        public async Task<Result<IEnumerable<RentalContactDTO>>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            var mapped = _mapper.Map<IEnumerable<RentalContactDTO>>(list);
            return Result<IEnumerable<RentalContactDTO>>.Success(mapped);
        }

        // 🔹 Lấy hợp đồng theo RentalOrderId
        public async Task<Result<RentalContactDTO?>> GetByRentalOrderIdAsync(int rentalOrderId)
        {
            var entity = await _repo.GetByRentalOrderIdAsync(rentalOrderId);
            if (entity == null || entity.IsDeleted)
                return Result<RentalContactDTO?>.Failure("Không tìm thấy hợp đồng.");

            return Result<RentalContactDTO?>.Success(_mapper.Map<RentalContactDTO>(entity));
        }

        // 🔹 Thêm hợp đồng
        public async Task<Result<string>> AddAsync(RentalContactCreateDTO dto)
        {
            var entity = _mapper.Map<RentalContact>(dto);
            entity.IsDeleted = false;

            await _repo.AddAsync(entity);
            return Result<string>.Success("Thêm hợp đồng thành công.");
        }

        // 🔹 Cập nhật hợp đồng theo RentalOrderId
        public async Task<Result<string>> UpdateAsync(RentalContactUpdateDTO dto)
        {
            if (dto.RentalOrderId == null)
                return Result<string>.Failure("Thiếu thông tin RentalOrderId để cập nhật.");

            var existing = await _repo.GetByRentalOrderIdAsync(dto.RentalOrderId.Value);
            if (existing == null)
                return Result<string>.Failure("Không tìm thấy hợp đồng để cập nhật.");

            // Cập nhật thủ công (hoặc có thể dùng AutoMapper)
            existing.RentalDate = dto.RentalDate;
            existing.RentalPeriod = dto.RentalPeriod;
            existing.ReturnDate = dto.ReturnDate;
            existing.TerminationClause = dto.TerminationClause;
            existing.Status = dto.Status;
            existing.LesseeId = dto.LesseeId;
            existing.LessorId = dto.LessorId;

            await _repo.UpdateAsync(existing);
            return Result<string>.Success("Cập nhật hợp đồng thành công.");
        }

        // 🔹 Xóa hợp đồng theo RentalOrderId
        public async Task<Result<string>> DeleteAsync(int rentalOrderId)
        {
            var existing = await _repo.GetByRentalOrderIdAsync(rentalOrderId);
            if (existing == null)
                return Result<string>.Failure("Không tìm thấy hợp đồng để xóa.");

            await _repo.DeleteAsync(existing.Id); // dùng Id để đánh dấu IsDeleted
            return Result<string>.Success("Xóa hợp đồng thành công.");
        }
    }
}
