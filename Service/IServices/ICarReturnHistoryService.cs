using Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IServices
{
    public interface ICarReturnHistoryService
    {
        Task<IEnumerable<CarReturnHistoryDTO>> GetAllAsync();
        Task<CarReturnHistoryDTO?> GetByIdAsync(int id);
        Task AddAsync(CarReturnHistoryCreateDTO dto);
        Task UpdateAsync(int id, CarReturnHistoryCreateDTO dto);
        Task DeleteAsync(int id);
    }
}
