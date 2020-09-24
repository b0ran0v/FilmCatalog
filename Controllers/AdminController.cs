using System;
using System.IO;
using System.Linq;
using FilmCatalog.Data;
using FilmCatalog.Models;
using FilmCatalog.Models.Forms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FilmCatalog.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ApplicationDbContext _context;

        public AdminController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Authorize]
        public IActionResult AdminPage()
        {
            var myFilms = from film in _context.Films where film.User.UserName == User.Identity.Name select film;
            ViewData["myFilms"] = myFilms.ToList();
            return View();
        }

        [HttpGet]
        [Authorize]
        public IActionResult AddNewFilm()
        {
            return View();
        }


        [HttpPost]
        [Authorize]
        public IActionResult AddNewFilm(FilmForm model)
        {
            if (ModelState.IsValid)
            {
                Film film = new Film
                {
                    Title = model.Title,
                    YearPublished = model.YearPublished,
                    Description = model.Description,
                    Director = model.Director,
                    User = _context.Users.FirstOrDefault(user => user.UserName == User.Identity.Name),
                    Poster = ReadImage(model.Poster)
                };
                _context.Films.Add(film);
                _context.SaveChanges();
                ModelState.AddModelError("", "Фильм успешно добавлен");
                return View("AddNewFilm");
            }

            ModelState.AddModelError("", "Форма заполнена неправильно");
            return View("AddNewFilm");
        }


        [HttpGet]
        [Authorize]
        public IActionResult EditFilm(int id)
        {
            var film = _context.Films.FirstOrDefault(f => f.FilmId == id);
            var form = new FilmForm();
            if (film != null)
            {
                form.Title = film.Title;
                form.YearPublished = film.YearPublished;
                form.Director = film.Director;
                form.Description = film.Description;
                form.FilmId = film.FilmId;
                if (film.Poster != null)
                {
                    var stream = new MemoryStream(film.Poster);
                    IFormFile file = new FormFile(stream, 0, film.Poster.Length, "name", "fileName");
                    form.Poster = file;
                }
            }
            return View(form);
        }

        [HttpPost]
        [Authorize]
        public IActionResult EditFilm(FilmForm form)
        {
            var film = _context.Films.FirstOrDefault(f => f.FilmId == form.FilmId);
            if (ModelState.IsValid)
            {
                if (film != null)
                {
                    film.Title = form.Title;
                    film.YearPublished = form.YearPublished;
                    film.Description = form.Description;
                    film.Director = form.Director;
                    if(form.Poster!=null) film.Poster = ReadImage(form.Poster);
                    _context.Update(film);
                    _context.SaveChanges();
                    ModelState.AddModelError("","Информация успешно изменена");
                }
            }
            else
            {
                ModelState.AddModelError("","Форма заполнена неправильно");
            }

            return View("EditFilm", form);
        }

        public IActionResult RemoveFilm(int id)
        {
            var filmToDelete = new Film {FilmId = id};
            _context.Films.Attach(filmToDelete);
            _context.Films.Remove(filmToDelete);
            _context.SaveChanges();
            return RedirectToAction("AdminPage");
        }

        private byte[] ReadImage(IFormFile file)
        {
            var memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            var bytes = memoryStream.ToArray();
            return bytes;
        }
    }
}