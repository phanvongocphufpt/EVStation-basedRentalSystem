using Service.Common;
using Service.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.IServices
{
    public interface IPaymentService
    {
        Task<Result<IEnumerable<PaymentDTO>>> GetAllAsync();
        Task<Result<IEnumerable<RevenueByLocationDTO>>> GetRevenueByLocationAsync();
        Task<Result<IEnumerable<PaymentDTO>>> GetAllByUserIdAsync(int id);
        Task<Result<PaymentDTO>> GetByIdAsync(int id);
        Task<Result<UpdatePaymentStatusDTO>> UpdatePaymentStatusAsync(UpdatePaymentStatusDTO updatePaymentDTO);

        // Thêm phương thức tạo Payment từ order
        Task<Result<PaymentWithLocationDTO>> CreatePaymentFromOrderAsync(CreatePaymentWithOrderDTO dto);
    }
}
