using System;
using System.Collections.Generic;

namespace Ntier.DAL.Entities
{
    public partial class Cart
    {
        public Cart()
        {
            CartDetails = new HashSet<CartDetail>();
        }

        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? CreateAt { get; set; }

        public virtual User? User { get; set; }
        public virtual ICollection<CartDetail> CartDetails { get; set; }
    }
}
