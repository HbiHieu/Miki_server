using System;
using System.Collections.Generic;

namespace Ntier.DAL.Entities
{
    public partial class Order
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? CreateAt { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public int? StatusId { get; set; }

        public virtual StatusOrder? Status { get; set; }
        public virtual User? User { get; set; }
    }
}
