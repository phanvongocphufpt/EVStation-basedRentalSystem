using Repository.Entities;
using Repository.IRepositories;
using Service.Common;
using Service.Common.Service.Common;
using Service.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Services
{
    public class RentalContactService : IRentalContactService
    {
        private readonly IRentalContactRepository _repository;

        public RentalContactService(IRentalContactRepository repository)
        {
            _repository = repository;
        }

        // ✅ Lấy tất cả liên hệ thuê xe
        public async Task<Result<IEnumerable<RentalContact>>> GetAllAsync()
        {
            try
            {
                var data = await _repository.GetAllAsync();
                if (data == null || !data.Any())
                    return Result<IEnumerable<RentalContact>>.Failure("Không có liên hệ thuê xe nào.");

                return Result<IEnumerable<RentalContact>>.Success(data, "Lấy danh sách liên hệ thuê xe thành công.");
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<RentalContact>>.Failure($"Lỗi khi lấy danh sách: {ex.Message}");
            }
        }

        // ✅ Lấy liên hệ theo RentalOrderId
        public async Task<Result<RentalContact>> GetByRentalOrderIdAsync(int rentalOrderId)
        {
            try
            {
                var contact = await _repository.GetByRentalOrderIdAsync(rentalOrderId);
                if (contact == null)
                    return Result<RentalContact>.Failure("Không tìm thấy thông tin liên hệ thuê xe.");

                return Result<RentalContact>.Success(contact, "Lấy thông tin liên hệ thuê xe thành công.");
            }
            catch (Exception ex)
            {
                return Result<RentalContact>.Failure($"Lỗi khi lấy thông tin: {ex.Message}");
            }
        }

        // ✅ Thêm mới liên hệ
        public async Task<Result<RentalContact>> AddAsync(RentalContact contact)
        {
            try
            {
                await _repository.AddAsync(contact);
                return Result<RentalContact>.Success(contact, "Thêm liên hệ thuê xe thành công.");
            }
            catch (Exception ex)
            {
                return Result<RentalContact>.Failure($"Lỗi khi thêm liên hệ: {ex.Message}");
            }
        }

        // ✅ Cập nhật liên hệ
        public async Task<Result<RentalContact>> UpdateAsync(RentalContact contact)
        {
            try
            {
                await _repository.UpdateAsync(contact);
                return Result<RentalContact>.Success(contact, "Cập nhật liên hệ thuê xe thành công.");
            }
            catch (Exception ex)
            {
                return Result<RentalContact>.Failure($"Lỗi khi cập nhật liên hệ: {ex.Message}");
            }
        }

        // ✅ Xóa liên hệ
        public async Task<Result<bool>> DeleteAsync(int id)
        {
            try
            {
                await _repository.DeleteAsync(id);
                return Result<bool>.Success(true, "Xóa liên hệ thuê xe thành công.");
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Lỗi khi xóa liên hệ: {ex.Message}");
            }
        }
    }
}
