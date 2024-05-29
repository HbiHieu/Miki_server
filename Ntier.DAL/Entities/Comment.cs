using System;
using System.Collections.Generic;

namespace Ntier.DAL.Entities
{
    public partial class Comment
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? ProductId { get; set; }
        public string? CreateAt { get; set; }
        public string? Content { get; set; }
        public int ? Rating { get; set; }

        public virtual Product? Product { get; set; }
        public virtual User? User { get; set; }
    }

    public partial class CommentDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? UserId { get; set; }
        public string? CreateAt { get; set; }
        public string? Content { get; set; }
        public int? Rating { get; set; }
    }
}
