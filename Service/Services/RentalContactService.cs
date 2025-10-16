using Repository.Entities;
using Repository.IRepositories;
using Service.IServices;
using System.Collections.Generic;
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

        public Task<IEnumerable<RentalContact>> GetAllAsync() => _repository.GetAllAsync();

        public Task<RentalContact?> GetByRentalOrderIdAsync(int rentalOrderId)
            => _repository.GetByRentalOrderIdAsync(rentalOrderId);

        public Task AddAsync(RentalContact contact) => _repository.AddAsync(contact);

        public Task UpdateAsync(RentalContact contact) => _repository.UpdateAsync(contact);

        public Task DeleteAsync(int id) => _repository.DeleteAsync(id);
    }
}
