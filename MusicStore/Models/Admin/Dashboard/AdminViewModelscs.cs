using MusicStore.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MusicStore.Models.ViewModels.Admin
{
        // Dashboard ViewModel
        public class DashboardViewModel
        {
            public int TotalAlbums { get; set; }
            public int TotalArtists { get; set; }
            public int TotalGenres { get; set; }
            public int TotalCustomers { get; set; }
             public int OrderTotal { get; set; }
            public List<OrderSummaryViewModel> RecentOrders { get; set; }
            public List<AlbumSummaryViewModel> TopSellingAlbums { get; set; }
        }

    // Album ViewModels
    public class AlbumsViewModel
    {
        public List<AlbumViewModel> Albums { get; set; }
    }

    public class AlbumAdminViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Genre { get; set; }
        public decimal Price { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string CoverUrl { get; set; }
        public int Sales { get; set; } // Optional: for tracking sales data
    }

    public class AlbumSummaryViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ArtistName { get; set; }
        public string GenreName { get; set; }
        public decimal Price { get; set; }
        public string CoverUrl { get; set; }
        public int SalesCount { get; set; }
    }

    public class AlbumFormViewModel
    {
        public int? Id { get; set; } // Nullable for create operations

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Artist is required")]
        [StringLength(100, ErrorMessage = "Artist name cannot exceed 100 characters")]
        public string Artist { get; set; }

        [Required(ErrorMessage = "Genre is required")]
        public int GenreId { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 1000.00, ErrorMessage = "Price must be between 0.01 and 1000.00")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Release date is required")]
        [Display(Name = "Release Date")]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        [Display(Name = "Album Cover")]
        public string CoverUrl { get; set; }

        // For dropdown list in the form
        public List<GenreAdminViewModel> AvailableGenres { get; set; }
    }

    // Artist ViewModels
    public class ArtistsAdminViewModel
    {
        public List<ArtistAdminViewModel> Artists { get; set; }
    }

    public class ArtistAdminViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Genre { get; set; }
        public string Country { get; set; }
        public int AlbumCount { get; set; } // Number of albums by this artist
    }

    public class ArtistFormViewModel
    {
        public int? Id { get; set; } // Nullable for create operations

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Genre is required")]
        [MaxLength(50, ErrorMessage = "Genre cannot exceed 50 characters")]
        public string Genre { get; set; }

        [MaxLength(50, ErrorMessage = "Country cannot exceed 50 characters")]
        public string Country { get; set; }
    }

    // Genre ViewModels
    public class GenresAdminViewModel
    {
        public List<GenreAdminViewModel> Genres { get; set; }
    }

    public class GenreAdminViewModel
    {
        public int GenreId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int AlbumCount { get; set; } // Number of albums in this genre
    }

    public class GenreFormViewModel
    {
        public int? GenreId { get; set; } // Nullable for create operations

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; }

        [MaxLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        public string Description { get; set; }
    }

    // Customer ViewModels
    public class CustomersAdminViewModel
    {
        public List<CustomerAdminViewModel> Customers { get; set; }
    }

    public class CustomerAdminViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public int OrderCount { get; set; }
        public List<OrderSummaryViewModel> Orders { get; set; }
    }


    public class OrderSummaryViewModel
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string StatusBadge { get; internal set; } = "";
    }

  

    // Admin Profile ViewModel
    public class AdminProfileViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        public string Phone { get; set; }

        [Display(Name = "Current Password")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Password must be at least {2} characters long.", MinimumLength = 6)]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}