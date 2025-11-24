using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs
{
    public class UserDTO
    {
            public int UserId { get; set; }
            public string Email { get; set; }
            public string FullName { get; set; }
            public DateTime? DateOfBirth { get; set; } // Ngày sinh
            public string? Address { get; set; } // Địa chỉ
            public string? Occupation { get; set; } // Nghề nghiệp
            public string? PhoneNumber { get; set; } // Số điện thoại
            public int? RentalLocationId { get; set; }
            public string Role { get; set; } // 1. Customer, 2. Staff, 3. Admin
            public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
    }

    public class CreateStaffUserDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; } 
        public int RentalLocationId { get; set; }
    }

    public class UpdateUserDTO
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public DateTime? DateOfBirth { get; set; } // Ngày sinh
        public string? Address { get; set; } // Địa chỉ
        public string? Occupation { get; set; } // Nghề nghiệp
    }
    public class UpdateCustomerNameDTO
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
    }

    // DTO để cập nhật thông tin cá nhân của customer
    public class UpdateCustomerProfileDTO
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? Occupation { get; set; }
    }
    public class UpdateCustomerPasswordDTO
    {
        public int UserId { get; set; }
        public string oldPassword { get; set; }
        public string newPassword { get; set; }
    }
    public class UpdateUserActiveStatusDTO
    {
        public int UserId { get; set; }
        public bool IsActive { get; set; }
    }
    public class UpdateStaffRentalLocationDTO
    {
        public int UserId { get; set; }
        public int RentalLocationId { get; set; }
    }
}
//check