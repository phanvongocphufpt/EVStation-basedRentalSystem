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
            public string Role { get; set; } // 1. Customer, 2. Staff, 3. Admin
            public bool IsActive { get; set; }
            public int? DriverLicenseId { get; set; }
            public int? CitizenId { get; set; }
    }
}
