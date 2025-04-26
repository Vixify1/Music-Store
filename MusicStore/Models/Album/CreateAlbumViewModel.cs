using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MusicStore.Models
{
    public class CreateAlbumViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Artist is required")]
        public string SelectedArtist { get; set; }

        [Required(ErrorMessage = "Release date is required")]
        public DateTime ReleaseDate { get; set; }

        [Required(ErrorMessage = "Genre is required")]
        public int GenreId { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.0, 1000.0, ErrorMessage = "Price must be between 0 and 1000")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Cover URL is required")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string coverUrl { get; set; }

        public List<SelectListItem>? Genres { get; set; }
        public List<SelectListItem>? Artists { get; set; }
    }
}