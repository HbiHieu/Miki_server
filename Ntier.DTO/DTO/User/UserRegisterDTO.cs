using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntier.DTO.DTO.User
{
    public class UserRegisterDTO
    {
        public string Id { get; set; } = null!;
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
