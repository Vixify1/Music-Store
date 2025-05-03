using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.Model.Entities
{
    public class Reviews
    {
        [Key]
        public int ReviewId { get; set; }

        public int Rating { get; set; }

        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        [Required]
        public int AlbumId { get; set; }
        [ForeignKey("AlbumId")]
        public Album Album { get; set; }
    }
}
