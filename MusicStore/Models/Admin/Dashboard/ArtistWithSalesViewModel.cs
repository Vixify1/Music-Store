using System.ComponentModel.DataAnnotations;

namespace MusicStore.Models.Admin.Artist
{
    public class ArtistWithSalesViewModel
    {
        public string ArtistName { get; set; }
        public string Genre { get; set; }
        public string Country { get; set; }
        public int AlbumsSold { get; set; }
    }

}
