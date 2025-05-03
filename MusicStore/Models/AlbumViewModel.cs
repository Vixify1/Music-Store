using MusicStore.Model.Entities;
using System.ComponentModel.DataAnnotations;

namespace MusicStore.Models
{
    public class AlbumViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(100)]
        public string Artist { get; set; }

        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        public int GenreId { get; set; }
        public string GenreName { get; set; }

        [Range(0.0, 1000.0)]
        public decimal Price { get; set; }

        public string coverUrl { get; set; }


        // Add this property for reviews
        public List<Reviews> Reviews { get; set; }
    }
}

