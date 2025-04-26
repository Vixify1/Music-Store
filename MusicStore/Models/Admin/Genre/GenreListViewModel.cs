namespace MusicStore.Models.Admin.Genre
{
    public class GenreListViewModel
    {
        public string? NameFilter { get; set; }
        public List<GenreViewModel> Genres{ get; set; } = new();
    }
}
