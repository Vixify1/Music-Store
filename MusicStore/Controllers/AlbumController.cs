using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStore.Model.Entities;
using MusicStore.Model.Abstract;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using MusicStore.Models.Admin.Genre;

namespace MusicStore.Models
{
    public class AlbumController : Controller
    {
        private readonly IEntitiesRepository<Album> _albumRepository;
        private readonly IEntitiesRepository<Genre> _genreRepository;
        private readonly IEntitiesRepository<ArtistEntities> _artistRepository;

        public AlbumController(
            IEntitiesRepository<Album> albumRepository,
            IEntitiesRepository<Genre> genreRepository,
            IEntitiesRepository<ArtistEntities> artistRepository)
        {
            _albumRepository = albumRepository;
            _genreRepository = genreRepository;
            _artistRepository = artistRepository;
        }
        [Authorize]
        public IActionResult Index()
        {
            var albums = _albumRepository.GetAll()
                .Include(a => a.Genre).ToList(); // This is important to load the related Genre


            var viewModels = albums.Select(a => new AlbumViewModel
            {
                Id = a.Id,
                GenreName = a.Genre?.Name,
                Title = a.Title,
                Artist = a.Artist,
                ReleaseDate = a.ReleaseDate,
                Price = a.Price,
                coverUrl = a.coverUrl
            }).ToList();

            return View(viewModels);
        }
        [Authorize]

        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();

            var album = _albumRepository.Get(id.Value);
            if (album == null) return NotFound();
            // Get the genre information
            var genre = _genreRepository.Get(album.GenreId);

            // Map to view model
            var viewModel = new AlbumViewModel
            {
                Id = album.Id,
                Title = album.Title,
                Artist = album.Artist,
                ReleaseDate = album.ReleaseDate,
                GenreId = album.GenreId,
                GenreName = genre?.Name, // Use the genre name
                Price = album.Price,
                coverUrl = album.coverUrl
            };

            return View(viewModel);
        }
        [Authorize]

        public IActionResult Create()
        {
            var viewModel = new CreateAlbumViewModel
            {
                Genres = _genreRepository.GetAll()
                    .Select(g => new SelectListItem { Value = g.GenreId.ToString(), Text = g.Name })
                    .ToList(),
                Artists = _artistRepository.GetAll()
                    .Select(a => new SelectListItem { Value = a.Name, Text = a.Name })
                    .ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateAlbumViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                }

                // Repopulate dropdowns
                viewModel.Genres = _genreRepository.GetAll()
                    .Select(g => new SelectListItem { Value = g.GenreId.ToString(), Text = g.Name })
                    .ToList();
                viewModel.Artists = _artistRepository.GetAll()
                    .Select(a => new SelectListItem { Value = a.Name, Text = a.Name })
                    .ToList();
                return View(viewModel);
            }

            var album = new Album
            {
                Title = viewModel.Title,
                Artist = viewModel.SelectedArtist,
                ReleaseDate = viewModel.ReleaseDate,
                GenreId = viewModel.GenreId,
                Price = viewModel.Price,
                coverUrl = viewModel.coverUrl,
                createdAt = DateTime.Now,
                updatedAt = DateTime.Now
            };

            _albumRepository.Add(album);
            _albumRepository.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [Authorize]

        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var album = _albumRepository.Get(id.Value);
            if (album == null) return NotFound();

            // Populate genres dropdown
            ViewBag.Genres = new SelectList(_genreRepository.GetAll(), "GenreId", "Name", album.GenreId);


            return View(album);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Album album)
        {
            if (id != album.Id) return NotFound();

            ModelState.Remove("Genre"); // Remove validation for navigation property

            if (ModelState.IsValid)
            {
                try
                {
                    // Update timestamp
                    album.updatedAt = DateTime.Now;

                    _albumRepository.Update(album);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlbumExistsAsync(album.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            // If validation fails, repopulate the dropdown
            ViewBag.Genres = new SelectList(_genreRepository.GetAll(), "GenreId", "Name", album.GenreId);
            return View(album);
        }
        [Authorize]

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var album = _albumRepository.Get(id.Value);
            if (album == null) return NotFound();


            // Get the genre information
            var genre = _genreRepository.Get(album.GenreId);

            // Map to view model
            var viewModel = new AlbumViewModel
            {
                Id = album.Id,
                Title = album.Title,
                Artist = album.Artist,
                ReleaseDate = album.ReleaseDate,
                GenreId = album.GenreId,
                GenreName = genre?.Name, // Use the genre name instead of just the ID
                Price = album.Price,
                coverUrl = album.coverUrl
            };

            return View(viewModel);
        }



        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var album = _albumRepository.Get(id);
            if (album != null)
            {
                _albumRepository.Remove(album);

            }


            return RedirectToAction(nameof(Index));
        }

        // Helping method for syin
        private bool AlbumExistsAsync(int id)
        {
            return _albumRepository.Get(id) != null;
        }


        }
    }
