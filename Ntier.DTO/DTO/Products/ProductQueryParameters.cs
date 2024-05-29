using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntier.DTO.DTO.Products
{
    public class ProductQueryParameters
    {
        public int page { get; set; }
        public string sortBy { get; set; }

        public string order { get; set; }

        public int limit { get; set; } = 4;

        public string keySearch { get; set; } = "";
    }
}
