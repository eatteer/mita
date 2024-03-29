﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Mita.Models
{
    public partial class Review
    {
        public Review()
        {
            Comments = new HashSet<Comment>();
        }

        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Body { get; set; } = null!;
        [JsonIgnore]
        public int? UserId { get; set; }
        [JsonIgnore]
        public int? MangaId { get; set; }

        public virtual Manga? Manga { get; set; }
        public virtual User? User { get; set; }
        [JsonIgnore]
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
