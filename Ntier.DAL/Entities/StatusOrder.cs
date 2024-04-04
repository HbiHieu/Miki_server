using System;
using System.Collections.Generic;

namespace Ntier.DAL.Entities
{
    public partial class StatusOrder
    {
        public StatusOrder()
        {
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime? ModifiedTime { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
