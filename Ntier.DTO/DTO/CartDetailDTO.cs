using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntier.DTO.DTO
{
    public class CartDetailDTO
    {
        public int? CartID { get; set; }
        public string? ProductID { get; set; }
        public string? Name { get; set; }
        public int? Quantity { get; set; }
        public int?Price { get; set; }
        public string? UserID { get; set; }
        public int? SizeId { get; set; }
        public string? URL { get; set; }         
    }
}
