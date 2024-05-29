using Ntier.DTO.DTO.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntier.DTO.DTO.Order
{
    public class ProductOrderDTO
    {
        public string ProductID { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string Url { get; set; }
        public int Quantity { get;set; }
        public int? SizeId { get; set; }

    }
    public class OrderDetail_DTO
    {
        public int? Id { get; set; }
        public string UserId { get; set; }
        public string? CreateAt { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string? Status { get;set; }
        public ICollection<ProductOrderDTO> Products { get; set; }
    }

    public class OrderDetail_Client
    {
        public string UserId { get; set; }
        public string? CreateAt { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string ProductID { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }
        public string Picture { get; set; }
        public int Quantity { get; set; }
        public int? Size { get; set; }
    }
}
