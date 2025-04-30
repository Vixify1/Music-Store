using System;

namespace MusicStore.Model.Entities
{
    public class AlbumEntities
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Genre { get; set; }
        public decimal Price { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string CoverUrl { get; set; }
    }
} 