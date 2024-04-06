using System;
using System.Collections.Generic;

namespace Ntier.DAL.Entities
{
    public partial class RefreshToken
    {
        public int Id { get; set; }
        public DateTime? ExpireAt { get; set; }
        public string? RefreshTk { get; set; }
        public string? Userid { get; set; }

        public virtual User? User { get; set; }
    }
}
