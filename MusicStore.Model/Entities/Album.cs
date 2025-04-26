using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicStore.Model.Entities
{
    public class Album
    {
        [Key]
        public int Id { get; set; }

        public DateTime ReleaseDate { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(100)]
        public string Artist { get; set; }

        public int GenreId { get; set; }

        [Range(0.0, 1000.0)]
        public decimal Price { get; set; }

        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public string coverUrl { get; set; }

        [ForeignKey("GenreId")]
        public Genre Genre { get; set; }
    }
    public class AlbumItem
    {
        public int Id { get; set; }
        public double Price { get; set; }
        public string Title { get; set; }
        public string Quantity { get; set; }
    }
}
