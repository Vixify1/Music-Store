using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using MusicStore.Model.Abstract;
using MusicStore.Model.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using MusicStore.Models;

namespace MusicStore.Controllers
{
    [Route("store/artists")]
    public class ArtistController : Controller
    {
        private readonly IEntitiesRepository<ArtistEntities> _artistRepository;
        private readonly IEntitiesRepository<Genre> _genreRepository;
        private readonly IEntitiesRepository<Album> _albumRepository;

        public ArtistController(
            IEntitiesRepository<ArtistEntities> artistRepository,
            IEntitiesRepository<Genre> genreRepository,
            IEntitiesRepository<Album> albumRepository)
        {
            _artistRepository = artistRepository;
            _genreRepository = genreRepository;
            _albumRepository = albumRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var artists = _artistRepository.GetAll().ToList();
            return View(artists);
        }

        [Authorize]
        public IActionResult Details(int id)
        {
            var artist = _artistRepository.Get(id);
            if (artist == null)
            {
                return NotFound();
            }
            return View(artist);
        }

        [HttpGet("browse/{id}")]
        public IActionResult Browse(int id)
        {
            var artist = _artistRepository.Get(id);
            if (artist == null) return NotFound();

            var albums = _albumRepository.GetAll()
                .Where(a => a.Artist == artist.Name)
                .Select(a => new AlbumViewModel
                {
                    Id = a.Id,
                    Title = a.Title,
                    Artist = a.Artist,
                    Price = a.Price,
                    coverUrl = a.coverUrl,
                    ReleaseDate = a.ReleaseDate,
                    GenreName = a.Genre.Name
                }).ToList();

            var viewModel = new ArtistBrowseViewModel
            {
                ArtistName = artist.Name,
                Albums = albums
            };

            return View(viewModel);
        }

        [Authorize]
        public IActionResult Create()
        {
            var genres = _genreRepository.GetAll().Select(a => new SelectListItem
            {
                Value = a.Name,
                Text = a.Name
            }).ToList();

            var model = new ArtistEntities
            {
                Generes = genres
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ArtistEntities artist)
        {
            if (ModelState.IsValid)
            {
                _artistRepository.Add(artist);
                return RedirectToAction(nameof(Index));
            }
            return View(artist);
        }

        [Authorize]
        public IActionResult Edit(int id)
        {
            var artist = _artistRepository.Get(id);
            if (artist == null)
            {
                return NotFound();
            }
            return View(artist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ArtistEntities artist)
        {
            if (id != artist.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _artistRepository.Update(artist);
                return RedirectToAction(nameof(Index));
            }
            return View(artist);
        }

        [Authorize]
        public IActionResult Delete(int id)
        {
            var artist = _artistRepository.Get(id);
            if (artist == null)
            {
                return NotFound();
            }
            return View(artist);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var artist = _artistRepository.Get(id);
            if (artist == null)
            {
                return NotFound();
            }

            _artistRepository.Remove(artist);
            return RedirectToAction(nameof(Index));
        }
    }
}