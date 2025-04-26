using Microsoft.AspNetCore.Mvc;
using MusicStore.Model.Abstract;
using MusicStore.Models.Admin.Genre;
using MusicStore.Model.Entities;
using Microsoft.AspNetCore.Identity;
namespace MusicStore.Controllers.Admin
{
    public class GenreController : Controller
    {
        private readonly IEntitiesRepository<Genre> _genreRepository;

        public GenreController(IEntitiesRepository<Genre> genreRepository)
        {
            _genreRepository = genreRepository;
        }

        public IActionResult Index(string searchName)
        {

            var genres = _genreRepository.GetAll().ToList();

            if (!string.IsNullOrEmpty(searchName))
            {
                genres = genres.Where(u => u.Name.Contains(searchName)).ToList();
            }
            var genre = new GenreListViewModel
            {

                NameFilter = searchName,
                Genres = genres.Select(c => new GenreViewModel
                {
                    GenreId = c.GenreId,
                    Name = c.Name,
                    Description = c.Description
                }).ToList()
            };
            return View(genre);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(GenreViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var genre = new Genre { Name = model.Name, Description = model.Description };
            _genreRepository.Add(genre);

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var genre = _genreRepository.Get(id);
            if (genre == null) return NotFound();

            var model = new GenreViewModel
            {
                GenreId = genre.GenreId,
                Name = genre.Name,
                Description = genre.Description
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(GenreViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var genre = _genreRepository.Get(model.GenreId);
            if (genre == null) return NotFound();

            genre.Name = model.Name;
            genre.Description = model.Description;
            _genreRepository.Update(genre);
            genre.Description = model.Description;
            _genreRepository.Update(genre);

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var genre = _genreRepository.Get(id);
            if (genre == null) return NotFound();

            var model = new GenreViewModel
            {
                GenreId = genre.GenreId,
                Name = genre.Name,
                Description = genre.Description
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var genre = _genreRepository.Get(id);
            if (genre == null) return NotFound();

            _genreRepository.Remove(genre);
            return RedirectToAction("Index");
        }
    }
}