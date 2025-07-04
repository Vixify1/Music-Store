﻿using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using MusicStore.Model.Abstract;
using MusicStore.Model.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using MusicStore.Models.Admin.Artist;

namespace MusicStore.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("admin/artists")]
    public class ArtistController : Controller
    {
        private readonly IEntitiesRepository<ArtistEntities> _artistRepository;
        private readonly IEntitiesRepository<Genre> _genreRepository;

        public ArtistController(IEntitiesRepository<ArtistEntities> artistRepository, IEntitiesRepository<Genre> genreRepository)
        {
            _artistRepository = artistRepository;
            _genreRepository = genreRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var artists = _artistRepository.GetAll().ToList();
            return View(artists);
        }

        [HttpGet("{id}")]
        public IActionResult Details(int id)
        {
            var artist = _artistRepository.Get(id);
            if (artist == null)
            {
                return NotFound();
            }
            return View(artist);
        }

        [HttpGet("create")]
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

        [HttpPost("create")]
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

        [HttpGet("edit/{id}")]
        public IActionResult Edit(int id)
        {
            var artist = _artistRepository.Get(id);
            if (artist == null)
            {
                return NotFound();
            }
            return View(artist);
        }

        [HttpPost("edit/{id}")]
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

        [HttpGet("delete/{id}")]
        public IActionResult Delete(int id)
        {
            var artist = _artistRepository.Get(id);
            if (artist == null)
            {
                return NotFound();
            }
            return View(artist);
        }

        [HttpPost("delete/{id}")]
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