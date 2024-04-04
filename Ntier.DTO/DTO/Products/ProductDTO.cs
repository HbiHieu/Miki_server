using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntier.DTO.DTO.Products
{
    public class ProductDTO
    {
        public string Id { get; set; }
        public string name { get; set; }
        public int? CategoryId { get; set; }

        public ICollection<StockDTO> stocks { get; set; }
        public ICollection<ImageDTO> pictures { get; set; }
        public int? sale { get; set; }
        public string desc { get; set; }

    }
}
