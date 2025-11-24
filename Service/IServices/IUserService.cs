using Repository.Entities;
using Service.Common;
using Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IServices
{
    public interface IUserService
    {
        Task<Result<IEnumerable<UserDTO>>> GetAllAsync();
        Task<Result<UserDTO>> GetByIdAsync(int id);
        Task<Result<CreateStaffUserDTO>> AddAsync(CreateStaffUserDTO staffUserDTO);
        Task<Result<UpdateUserDTO>> UpdateAsync(UpdateUserDTO updateUserDTO);
        Task<Result<UpdateCustomerNameDTO>> UpdateCustomerNameAsync(UpdateCustomerNameDTO updateUserDTO);
        Task<Result<UpdateCustomerProfileDTO>> UpdateCustomerProfileAsync(UpdateCustomerProfileDTO updateProfileDTO);
        Task<Result<UpdateCustomerPasswordDTO>> UpdateCustomerPasswordAsync(UpdateCustomerPasswordDTO updateUserDTO);
        Task<Result<UpdateUserActiveStatusDTO>> UpdateUserActiveStatusAsync(UpdateUserActiveStatusDTO updateUserActiveStatusDTO);
        Task<Result<UpdateStaffRentalLocationDTO>> UpdateStaffRentalLocationAsync(UpdateStaffRentalLocationDTO updateStaffRentalLocationDTO);
        Task DeleteAsync(int id);
    }
}
