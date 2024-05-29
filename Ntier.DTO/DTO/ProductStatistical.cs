using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntier.DTO.DTO
{
    public class ProductStatistical
    {
        public long STT { get; set; }
        public string Name { get; set; }
        public int Sale { get;set; }
        public string TenTheLoai { get; set; }
        public string Create_At { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
        public string StatusName { get; set; }
    }
}
