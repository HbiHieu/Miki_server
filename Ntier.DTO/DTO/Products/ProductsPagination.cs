using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntier.DTO.DTO.Products
{
    public class ProductsPagination
    {
        public ICollection<ProductDTO?> data { get; set;}
        public Pagination pagination { get; set;}
    }
}
