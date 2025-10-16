using Microsoft.EntityFrameworkCore;
using Repository.Context;
using Repository.Entities;
using Repository.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class RentalContactRepository : IRentalContactRepository
    {
        private readonly EVSDbContext _context;
        public RentalContactRepository(EVSDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<RentalContact>> GetAllAsync()
        {
            return await _context.RentalContacts.Include(r => r.Lessee)
                .Include(r => r.Lessor)
                .Include(r => r.RentalOrder)
                .ToListAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var rentalContact = await _context.RentalContacts.FindAsync(id);
            if (rentalContact != null)
            {
                _context.RentalContacts.Remove(rentalContact);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.RentalContacts.AnyAsync(r => r.Id == id);
        }

        public async Task AddAsync(RentalContact rentalContact)
        {
            await _context.RentalContacts.AddAsync(rentalContact);
            await _context.SaveChangesAsync();
        }
        public async Task<RentalContact?> GetByIdAsync(int id)
        {
            return await _context.RentalContacts
                .Include(r => r.Lessee)
                .Include(r => r.Lessor)
                .Include(r => r.RentalOrder)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task UpdateAsync(RentalContact rentalContact)
        {
            _context.RentalContacts.Update(rentalContact);
            await _context.SaveChangesAsync();
        }

    }
    }
