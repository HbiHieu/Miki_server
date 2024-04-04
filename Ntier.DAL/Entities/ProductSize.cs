using System;
using System.Collections.Generic;

namespace Ntier.DAL.Entities
{
    public partial class ProductSize
    {
        public ProductSize()
        {
            ProductSizeDetails = new HashSet<ProductSizeDetail>();
        }

        public int Id { get; set; }
        public int? Size { get; set; }

        public virtual ICollection<ProductSizeDetail> ProductSizeDetails { get; set; }
    }
}
