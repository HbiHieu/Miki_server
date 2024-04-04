using System;
using System.Collections.Generic;

namespace Ntier.DAL.Entities
{
    public partial class ProductSizeDetail
    {
        public string ProductId { get; set; } = null!;
        public int SizeId { get; set; }
        public int? Quantity { get; set; }
        public int? Price { get; set; }

        public virtual Product Product { get; set; } = null!;
        public virtual ProductSize Size { get; set; } = null!;
    }
}
