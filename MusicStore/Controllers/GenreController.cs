using Microsoft.AspNetCore.Mvc;
using MusicStore.Model.Abstract;
using MusicStore.Model.Entities;
using MusicStore.Models.Admin.Genre;
using MusicStore.Models;

namespace MusicStore.Controllers
{
    [Route("store/genres")]
    public class GenreController : Controller
    {
        private readonly IEntitiesRepository<Genre> _genreRepository;
        private readonly IEntitiesRepository<Album> _albumRepository;

        public GenreController(
            IEntitiesRepository<Genre> genreRepository,
            IEntitiesRepository<Album> albumRepository)
        {
            _genreRepository = genreRepository;
            _albumRepository = albumRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var genres = _genreRepository.GetAll().ToList();
            var viewModel = new MusicStore.Models.Admin.Genre.GenreListViewModel
            {
                Genres = genres.Select(g => new MusicStore.Models.Admin.Genre.GenreViewModel
                {
                    GenreId = g.GenreId,
                    Name = g.Name,
                    Description = g.Description
                }).ToList()
            };
            return View(viewModel);
        }

        [HttpGet("browse/{id}")]
        public IActionResult Browse(int id)
        {
            var genre = _genreRepository.Get(id);
            if (genre == null) return NotFound();

            var albums = _albumRepository.GetAll()
                .Where(a => a.GenreId == id)
                .Select(a => new AlbumViewModel
                {
                    Id = a.Id,
                    Title = a.Title,
                    Artist = a.Artist,
                    Price = a.Price,
                    coverUrl = a.coverUrl,
                    ReleaseDate = a.ReleaseDate,
                    GenreName = genre.Name
                }).ToList();

            var viewModel = new GenreBrowseViewModel
            {
                GenreName = genre.Name,
                Albums = albums
            };

            return View(viewModel);
        }
    }
}