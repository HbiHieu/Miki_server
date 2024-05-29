using System;
using System.Collections.Generic;

namespace Ntier.DAL.Entities
{
    public partial class Product
    {
        public Product()
        {
            CartDetails = new HashSet<CartDetail>();
            Comments = new HashSet<Comment>();
            ProductImages = new HashSet<ProductImage>();
            ProductSizeDetails = new HashSet<ProductSizeDetail>();
        }

        public string Id { get; set; } = null!;
        public string? Name { get; set; }
        public int? Sale { get; set; }
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public DateTime? CreateAt { get; set; }
        public int? MinPrice { get; set; }
        public int? Quantity { get; set; }
        public int? Status { get; set; }

        public virtual Category? Category { get; set; }
        public virtual ICollection<CartDetail> CartDetails { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
        public virtual ICollection<ProductSizeDetail> ProductSizeDetails { get; set; }
    }
}
