using Repository.Entities;
using Repository.IRepositories;
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

        public async Task<IEnumerable<RentalOrder>> GetAllAsync()
        {
            return await _rentalOrderRepository.GetAllAsync();
        }

        public async Task<RentalOrder?> GetByIdAsync(int id)
        {
            return await _rentalOrderRepository.GetByIdAsync(id);
        }

        public async Task<RentalOrder> CreateAsync(RentalOrder rentalOrder)
        {
            return await _rentalOrderRepository.CreateAsync(rentalOrder);
        }

        public async Task<RentalOrder> UpdateAsync(RentalOrder rentalOrder)
        {
            return await _rentalOrderRepository.UpdateAsync(rentalOrder);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _rentalOrderRepository.DeleteAsync(id);
        }
    }
}


