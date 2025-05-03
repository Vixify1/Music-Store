using System.Collections.Generic;

namespace MusicStore.Models
{
    public class ArtistBrowseViewModel
    {
        public string ArtistName { get; set; }
        public List<AlbumViewModel> Albums { get; set; }
    }
}