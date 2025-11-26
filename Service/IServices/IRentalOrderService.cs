using Microsoft.AspNetCore.Http;
using Service.Common;
using Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IServices
{
    public interface IRentalOrderService
    {
        Task<Result<IEnumerable<RentalOrderDTO>>> GetAllAsync();
        Task<Result<IEnumerable<RentalOrderWithDetailsDTO>>> GetOrderByLocationAsync(int locationId);
        Task<Result<IEnumerable<RentalOrderDTO>>> GetByPhoneNumber(string phoneNumber);
        Task<Result<RentalOrderDTO>> GetByIdAsync(int id);
        Task<Result<RentalOrderWithDetailsDTO>> GetByIdWithDetailsAsync(int orderId);
        Task<Result<IEnumerable<RentalOrderDTO>>> GetByUserIdAsync(int id);
        Task<Result<CreateRentalOrderResponseDTO>> CreateAsync(CreateRentalOrderDTO createRentalOrderDTO, HttpContext httpContext);
        Task<Result<CreateRentalOrderResponseDTO>> CreateWithMomoAsync(
    CreateRentalOrderDTO createRentalOrderDTO);
        Task<Result<bool>> CancelOrderAsync(int orderId);
        Task<Result<bool>> CancelOrderForStaffAsync(int orderId);
        Task<Result<UpdateRentalOrderTotalDTO>> UpdateTotalAsync(UpdateRentalOrderTotalDTO updateRentalOrderTotalDTO);
        Task<Result<bool>> ConfirmOrderPaymentAsync(ConfirmOrderPaymentDTO dto);
        Task<Result<bool>> ConfirmTotalAsync(int orderId);
        Task<PaymentCallbackResult> ProcessVnpayIpnAsync(IQueryCollection queryParams);
        Task<PaymentCallbackResult> ProcessVnpayCallbackAsync(IQueryCollection queryParams);
        Task<PaymentCallbackResult> ProcessVnpayCallbackManualAsync(string txnRef, string responseCode);
        Task<PaymentCallbackResult> ProcessMomoCallbackManualAsync(string requestId, string resultCode);
        Task<Result<bool>> AddContactToOrderAsync(AddContactToOrderDTO dto);
        Task<Result<GetContactFromOrderDTO>> GetContactFromOrderDTO(int orderId);
        Task<Result<bool>> UpdateContact(GetContactFromOrderDTO dto);
    }
}
