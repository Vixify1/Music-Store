using System.ComponentModel.DataAnnotations;

namespace MusicStore.Models.Admin.Artist
{
    public class ArtistViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Genre { get; set; }
        public string Country { get; set; }
    }
}
