using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntier.DTO.DTO.User
{
    public class UserDTO
    {
        public string Id { get; set; } = null!;
        public string Email { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get;set; }

        public string Role { get; set; }
        public string Access_token { get; set; }

        public DateTime Expire_At { get; set; }

        public string Refresh_Token { get; set; } 
        public int cartId { get; set; }
    }
}
