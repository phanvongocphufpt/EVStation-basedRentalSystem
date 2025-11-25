using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Service.DTOs;
using Service.IServices;

namespace EVStation_basedRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetAllUser()
        {
            var Users = await _userService.GetAllAsync();
            return Ok(Users);
        }
        [HttpGet("GetById")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }
        [HttpGet("GetBankingInfo")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetBankingInfo(int userId)
        {
            var bankingInfo = await _userService.GetBankInfoAsync(userId);
            if (bankingInfo == null)
                return NotFound();
            return Ok(bankingInfo);
        }
        [HttpPost("CreateStaff")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateStaff([FromBody] CreateStaffUserDTO staffUserDTO)
        {
            if (staffUserDTO == null)
                return BadRequest("Invalid data.");

            var result = await _userService.AddAsync(staffUserDTO);
            return Ok(result);
        }

        [HttpPut]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Update([FromBody] UpdateUserDTO updateUserDTO)
        {
            if (updateUserDTO == null)
                return BadRequest("Invalid data.");

            var result = await _userService.UpdateAsync(updateUserDTO);
            return Ok(result);
        }
        [HttpPut("UpdateBankingInfo")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> UpdateBankingInfo([FromBody] UpdateBankInfoDTO updateBankingInfoDTO)
        {
            if (updateBankingInfoDTO == null)
                return BadRequest("Invalid data.");
            var result = await _userService.UpdateBankInfoAsync(updateBankingInfoDTO);
            return Ok(result);
        }
        [HttpPut("UpdateCustomerName")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> UpdateCustomerName([FromBody] UpdateCustomerNameDTO updateUserDTO)
        {
            if (updateUserDTO == null)
                return BadRequest("Invalid data.");

            var result = await _userService.UpdateCustomerNameAsync(updateUserDTO);
            return Ok(result);
        }

        [HttpPut("UpdateCustomerPassword")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> UpdateCustomerPassword([FromBody] UpdateCustomerPasswordDTO updateUserDTO)
        {
            if (updateUserDTO == null)
                return BadRequest("Invalid data.");

            var result = await _userService.UpdateCustomerPasswordAsync(updateUserDTO);
            return Ok(result);
        }

        [HttpPut("UpdateUserActiveStatus")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateUserActiveStatus([FromBody] UpdateUserActiveStatusDTO updateUserActiveStatusDTO)
        {
            if (updateUserActiveStatusDTO == null)
                return BadRequest(new { message = "Dữ liệu cập nhật trạng thái người dùng không được để trống." });

            if (updateUserActiveStatusDTO.UserId <= 0)
                return BadRequest(new { message = "Mã người dùng không hợp lệ." });

            if (!ModelState.IsValid)
                return BadRequest(new { message = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại thông tin.", errors = ModelState });

            var result = await _userService.UpdateUserActiveStatusAsync(updateUserActiveStatusDTO);
            if (!result.IsSuccess)
                return NotFound(new { message = result.Message ?? "Không tìm thấy người dùng hoặc không thể cập nhật trạng thái." });

            return Ok(new { message = result.Message ?? "Cập nhật trạng thái người dùng thành công." });
        }

        [HttpPut("UpdateStaffRentalLocation")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateStaffRentalLocation([FromBody] UpdateStaffRentalLocationDTO updateStaffRentalLocationDTO)
        {
            if (updateStaffRentalLocationDTO == null)
                return BadRequest(new { message = "Dữ liệu cập nhật địa điểm cho thuê không được để trống." });

            if (updateStaffRentalLocationDTO.UserId <= 0)
                return BadRequest(new { message = "Mã người dùng không hợp lệ." });

            if (updateStaffRentalLocationDTO.RentalLocationId <= 0)
                return BadRequest(new { message = "Mã địa điểm cho thuê không hợp lệ." });

            if (!ModelState.IsValid)
                return BadRequest(new { message = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại thông tin.", errors = ModelState });

            var result = await _userService.UpdateStaffRentalLocationAsync(updateStaffRentalLocationDTO);
            if (!result.IsSuccess)
                return NotFound(new { message = result.Message ?? "Không tìm thấy nhân viên hoặc không thể cập nhật địa điểm cho thuê." });

            return Ok(new { message = result.Message ?? "Cập nhật địa điểm cho thuê của nhân viên thành công." });
        }
        
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteAsync(id);
            return Ok("User deleted successfully.");
        }
    }
}
