using Service.DTOs;


namespace Service.IServices
{
    public interface ICarDeliveryHistoryService
    {
        Task<(IEnumerable<CarDeliveryHistoryDTO> data, int total)> GetAllAsync(int pageIndex, int pageSize);
        Task<CarDeliveryHistoryDTO?> GetByIdAsync(int id);
        Task AddAsync(CarDeliveryHistoryCreateDTO dto);
        Task UpdateAsync(int id, CarDeliveryHistoryCreateDTO dto);
        Task DeleteAsync(int id);
    }
}
