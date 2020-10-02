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
        private readonly ApplicationDbContext _context;
        private string _notification = string.Empty;

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
            ViewData["Notification"] = _notification;
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
                    Poster = Tools.Tools.ReadImage(model.Poster)
                };
                _context.Films.Add(film);
                _context.SaveChanges();
                _notification = "Фильм успешно добавлен";
                return RedirectToAction("AdminPage");
            }

            ModelState.AddModelError("", "Форма заполнена неправильно");
            return View("AddNewFilm");
        }


        [HttpGet]
        [Authorize]
        public IActionResult EditFilm(int id)
        {
            var film = _context.Films.Include(f=>f.User).FirstOrDefault(f => f.FilmId == id);
            var form = new FilmForm();
            if (film != null)
            {
                if (film.User.UserName != User.Identity.Name) return StatusCode(403);
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

                return View(form);
            }

            return View(form);
        }


        [HttpPost]
        [Authorize]
        public IActionResult EditFilm(FilmForm form)
        {
            var film = _context.Films.Include(f=>f.User).FirstOrDefault(f => f.FilmId == form.FilmId);
            if (ModelState.IsValid)
            {
                if (film != null)
                {
                    if (film.User.UserName != User.Identity.Name) return StatusCode(403);
                    film.Title = form.Title;
                    film.YearPublished = form.YearPublished;
                    film.Description = form.Description;
                    film.Director = form.Director;
                    if (form.Poster != null) film.Poster = Tools.Tools.ReadImage(form.Poster);
                    _context.Update(film);
                    _context.SaveChanges();
                    _notification = "Информация успешно изменена";
                    return RedirectToAction("AdminPage");
                }
            }
            else
            {
                ModelState.AddModelError("", "Форма заполнена неправильно");
            }

            return View("EditFilm", form);
        }

        [Authorize]
        [HttpPost]
        public IActionResult RemoveFilm(int id)
        {
            var film = _context.Films.Include(f=>f.User).FirstOrDefault(f => f.FilmId == id);
            if (film == null) return StatusCode(404);
            if (film.User.UserName != User.Identity.Name) return StatusCode(403);
            _context.Films.Attach(film);
            _context.Films.Remove(film);
            _context.SaveChanges();
            return RedirectToAction("AdminPage");
        }
    }
}