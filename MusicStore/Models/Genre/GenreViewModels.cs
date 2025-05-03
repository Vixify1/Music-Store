using System.Collections.Generic;

namespace MusicStore.Models
{
    public class GenreListViewModel
    {
        public List<GenreViewModel> Genres { get; set; }
    }

    public class GenreViewModel
    {
        public int GenreId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class GenreBrowseViewModel
    {
        public string GenreName { get; set; }
        public List<AlbumViewModel> Albums { get; set; }
    }
}