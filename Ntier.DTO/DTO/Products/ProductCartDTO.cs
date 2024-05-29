using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntier.DTO.DTO.Products
{
    public class ProductCartDTO
    {
        public int cartID { get; set; }
        public string productID { get; set; }
        public int quantity { get; set; }
        public string userId { get; set; }
        public int SizeId { get; set; }
        public int price { get; set; }
    }
}
