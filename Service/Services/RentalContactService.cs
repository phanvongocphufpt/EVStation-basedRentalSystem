using Repository.Entities;
using Repository.IRepositories;
using Repository.Repositories;
using Service.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class RentalContactService : IRentalContactService
    {
        private readonly IRentalContactRepository _rentalContactRepository;

        public RentalContactService(IRentalContactRepository rentalContactRepository)
        {
            _rentalContactRepository = rentalContactRepository;
        }

        public Task<IEnumerable<RentalContact>> GetAllAsync()
        {
            return _rentalContactRepository.GetAllAsync();
        }

        public Task<RentalContact> GetByIdAsync(int id)
        {
            return _rentalContactRepository.GetByIdAsync(id);
        }

        public Task AddAsync(RentalContact rentalContact)
        {
            return _rentalContactRepository.AddAsync(rentalContact);
        }

        public Task UpdateAsync(RentalContact rentalContact)
        {
            return _rentalContactRepository.UpdateAsync(rentalContact);
        }

        public Task DeleteAsync(int id)
        {
            return _rentalContactRepository.DeleteAsync(id);
        }
        public Task<bool> ExistsAsync(int id)
        {
            return _rentalContactRepository.ExistsAsync(id);
        }

    }
}


