using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStore.Model.Abstract;
using MusicStore.Model.Entities;
using MusicStore.Models;
using MusicStore.Models.Admin.Artist;
using MusicStore.Models.ViewModels.Admin;
using System.Linq;


namespace MusicStore.Controllers.Admin
{
    [Authorize(Roles = "Admin")]

    public class DashboardController : Controller
    {
        private readonly IEntitiesRepository<Album> _albumsRepository;
        private readonly IEntitiesRepository<ArtistEntities> _artistsRepository;
        private readonly IEntitiesRepository<Order> _ordersRepository;
        private readonly IEntitiesRepository<Genre> _genresRepository;
        private readonly IEntitiesRepository<Customer> _customersRepository;
        private readonly IEntitiesRepository<ArtistEntities> _artistEntitiesRepository;



        public DashboardController(IEntitiesRepository<Album> albumRepository,
                               IEntitiesRepository<ArtistEntities> artistsRepository,
                               IEntitiesRepository<Order> ordersRepository,
                               IEntitiesRepository<Genre> genresRepository,
                               IEntitiesRepository<Customer> customerRespository,
                               IEntitiesRepository<ArtistEntities> artistEntitiesRepository)
        {
            _albumsRepository = albumRepository;
            _artistsRepository = artistsRepository;
            _ordersRepository = ordersRepository;
            _genresRepository = genresRepository;
            _customersRepository = customerRespository;
            _artistEntitiesRepository = artistEntitiesRepository;
        }

        // Dashboard
        public async Task<IActionResult> Index()
        {
            var ordersWithCustomer = await _ordersRepository.GetAllWithIncludesAsync(o => o.Customer, o => o.OrderItems);
            var recentOrders = ordersWithCustomer
                                .OrderByDescending(o => o.OrderDate)
                                .Take(5)
                                .Select(o => new OrderSummaryViewModel
                                {
                                    OrderId = o.OrderId,
                                    CustomerName = o.Customer != null
                                    ? $"{o.Customer.firstName} {o.Customer.lastName}".Trim() : "Unknown",
                                    OrderDate = o.OrderDate,
                                    TotalAmount = o.OrderItems?.Sum(oi => oi.Quantity * oi.UnitPrice) ?? 0,
                                    Status = o.Status,
                                    StatusBadge = o.Status.ToString() == "Completed" ? "success"
                                                    : o.Status.ToString() == "Pending" ? "warning"
                                                    : o.Status.ToString() == "Cancelled" ? "danger" : "info"
                                }).ToList();



            var dashboardViewModel = new DashboardViewModel
            {
                TotalAlbums = await _albumsRepository.CountAsync(),
                TotalArtists = await _artistsRepository.CountAsync(),
                TotalGenres = await _genresRepository.CountAsync(),
                TotalCustomers = await _customersRepository.CountAsync(),
                OrderTotal = await _ordersRepository.CountAsync(),
                RecentOrders = recentOrders,

            };

            return View(dashboardViewModel);
        }


        public async Task<IActionResult> Albums(string? title, string? artist, int? genreId)
        {
            var albums = await _albumsRepository.GetAllAsync();
            var orderItems = await _ordersRepository.GetAllWithIncludesAsync(o => o.OrderItems);

            var albumStats = albums
                .Where(a =>
                    (string.IsNullOrEmpty(title) || a.Title.Contains(title, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrEmpty(artist) || a.Artist.Contains(artist, StringComparison.OrdinalIgnoreCase)) &&
                    (!genreId.HasValue || a.GenreId == genreId.Value))
                .Select(album => new AlbumListViewModel
                {
                    Id = album.Id,
                    Title = album.Title,
                    Artist = album.Artist,
                    TotalOrderedQuantity = orderItems
                        .SelectMany(o => o.OrderItems)
                        .Where(oi => oi.AlbumId == album.Id)
                        .Sum(oi => oi.Quantity)
                })
                .ToList();

            return View(albumStats);
        }


        public async Task<IActionResult> Artists()
        {
            // Get all orders with their associated items and albums
            var ordersWithItems = await _ordersRepository.GetAll()
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Album) // Ensure the Album details are loaded
                .ToListAsync();

            // Get the list of artists (you might already have this in your database, this is an assumption)
            var artistEntities = await _artistEntitiesRepository.GetAllAsync();

            // Group by artist and calculate total quantity sold
            var artistSales = ordersWithItems
                .SelectMany(o => o.OrderItems)  // Flatten the OrderItems from all orders
                .Where(oi => oi.Album != null)  // Ensure that the OrderItem has an associated Album
                .GroupBy(oi => oi.Album.Artist) // Group by the artist's name
                .Select(group =>
                {
                    // Find the matching artist details
                    var artist = artistEntities.FirstOrDefault(a => a.Name == group.Key);

                    return new ArtistWithSalesViewModel
                    {
                        ArtistName = group.Key,  // The artist name (from the Album entity)
                        AlbumsSold = group.Sum(oi => oi.Quantity),  // Sum of quantities sold
                        Genre = artist?.Genre ?? "Unknown",  // Get the Genre from ArtistEntities (or default to "Unknown")
                        Country = artist?.Country ?? "Unknown"  // Get the Country from ArtistEntities (or default to "Unknown")
                    };
                }).ToList();

            // Pass the result to the view
            return View(artistSales);
        }





    }
}
