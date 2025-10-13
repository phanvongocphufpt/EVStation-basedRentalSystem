using Repository.Entities;
using Repository.IRepositories;
using Service.Common;
using Service.IServices;

namespace Service.Services
{
    public class RentalOrderService : IRentalOrderService
    {
        private readonly IRentalOrderRepository _rentalOrderRepository;

        public RentalOrderService(IRentalOrderRepository rentalOrderRepository)
        {
            _rentalOrderRepository = rentalOrderRepository;
        }

        public async Task<Result<IEnumerable<RentalOrder>>> GetAllAsync()
        {
            try
            {
                var items = await _rentalOrderRepository.GetAllAsync();
                return Result<IEnumerable<RentalOrder>>.Success(items, "Rental orders retrieved successfully");
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<RentalOrder>>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<Result<RentalOrder>> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0) return Result<RentalOrder>.Failure("Invalid id");
                var item = await _rentalOrderRepository.GetByIdAsync(id);
                if (item == null) return Result<RentalOrder>.Failure("Rental order not found");
                return Result<RentalOrder>.Success(item, "OK");
            }
            catch (Exception ex)
            {
                return Result<RentalOrder>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<Result<RentalOrder>> CreateAsync(RentalOrder rentalOrder)
        {
            try
            {
                if (rentalOrder == null) return Result<RentalOrder>.Failure("Body required");
                if (rentalOrder.UserId <= 0 || rentalOrder.CarId <= 0 || rentalOrder.RentalContactId <= 0)
                    return Result<RentalOrder>.Failure("Invalid foreign keys");
                var created = await _rentalOrderRepository.CreateAsync(rentalOrder);
                return Result<RentalOrder>.Success(created, "Created");
            }
            catch (Exception ex)
            {
                return Result<RentalOrder>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<Result<RentalOrder>> UpdateAsync(RentalOrder rentalOrder)
        {
            try
            {
                if (rentalOrder == null || rentalOrder.Id <= 0)
                    return Result<RentalOrder>.Failure("Invalid payload");
                var exists = await _rentalOrderRepository.GetByIdAsync(rentalOrder.Id);
                if (exists == null) return Result<RentalOrder>.Failure("Rental order not found");
                var updated = await _rentalOrderRepository.UpdateAsync(rentalOrder);
                return Result<RentalOrder>.Success(updated, "Updated");
            }
            catch (Exception ex)
            {
                return Result<RentalOrder>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            try
            {
                if (id <= 0) return Result<bool>.Failure("Invalid id");
                var exists = await _rentalOrderRepository.GetByIdAsync(id);
                if (exists == null) return Result<bool>.Failure("Rental order not found");
                var ok = await _rentalOrderRepository.DeleteAsync(id);
                return ok ? Result<bool>.Success(true, "Deleted") : Result<bool>.Failure("Delete failed");
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<Result<Pagination<RentalOrder>>> GetPagedAsync(int pageIndex, int pageSize)
        {
            try
            {
                if (pageIndex < 0 || pageSize <= 0)
                    return Result<Pagination<RentalOrder>>.Failure("Invalid paging");
                var page = await _rentalOrderRepository.GetPagedAsync(pageIndex, pageSize);
                return Result<Pagination<RentalOrder>>.Success(page, "OK");
            }
            catch (Exception ex)
            {
                return Result<Pagination<RentalOrder>>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<RentalOrder>>> GetByUserIdAsync(int userId)
        {
            try
            {
                if (userId <= 0) return Result<IEnumerable<RentalOrder>>.Failure("Invalid userId");
                var items = await _rentalOrderRepository.GetByUserIdAsync(userId);
                return Result<IEnumerable<RentalOrder>>.Success(items, "OK");
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<RentalOrder>>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<RentalOrder>>> GetByCarIdAsync(int carId)
        {
            try
            {
                if (carId <= 0) return Result<IEnumerable<RentalOrder>>.Failure("Invalid carId");
                var items = await _rentalOrderRepository.GetByCarIdAsync(carId);
                return Result<IEnumerable<RentalOrder>>.Success(items, "OK");
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<RentalOrder>>.Failure($"Error: {ex.Message}");
            }
        }
    }
}


