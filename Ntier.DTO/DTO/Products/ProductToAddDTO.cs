using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntier.DTO.DTO.Products
{
    public class ProductToAddDTO
    {
        public string Id { get; set; }
        public string name { get; set; }

        public int categoryId { get; set; }

        public ICollection<StockDTO>? stock { get; set; }
        public int sale {get; set; } 
        public string desc { get; set; }
        public int? MinPrice { get; set; }
    }
}
