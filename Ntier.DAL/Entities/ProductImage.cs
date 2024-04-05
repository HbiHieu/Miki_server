using System;
using System.Collections.Generic;

namespace Ntier.DAL.Entities
{
    public partial class ProductImage
    {
        public string Url { get; set; } = null!;
        public string? ProductId { get; set; }
        public int? Index { get; set; }

        public virtual Product? Product { get; set; }
    }
}
