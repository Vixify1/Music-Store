using System.ComponentModel.DataAnnotations;

namespace MusicStore.Models.Admin.Genre
{
    public class GenreViewModel
    {
        public int GenreId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }
    }
}
