using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FilmCatalog.Data;
using FilmCatalog.Models;
using FilmCatalog.Models.Forms;
using FilmCatalog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FilmCatalog.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly FilmService _filmService;
        private static string _notification = string.Empty;

        public AdminController(ILogger<HomeController> logger, 
            ApplicationDbContext context, 
            FilmService filmService, 
            IHttpContextAccessor ctxAccessor)
        {
            _logger = logger;
            _context = context;
            _filmService = filmService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AdminPage()
        {
            var myFilms = from film in _context.Films where film.User.UserName == User.Identity.Name select film;
            ViewData["myFilms"] = await myFilms.ToListAsync();
            ViewData["Notification"] = _notification;
            _notification = string.Empty;
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNewFilm(FilmForm model)
        {
            if (ModelState.IsValid)
            {
                await _filmService.AddNewFilm(model);
                _notification = $"Фильм {model.Title} успешно добавлен.";
                return RedirectToAction("AdminPage");
            }

            ModelState.AddModelError("", "Форма заполнена неправильно");
            return View("AddNewFilm");
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditFilm(int id)
        {
            var form = await _filmService.EditFilm(id);
            return View(form);
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFilm(FilmForm form)
        {
            if (ModelState.IsValid)
            {
                await _filmService.EditFilm(form);
                _notification = $"Информация в фильме {form.Title} успешно изменена.";
                return RedirectToAction("AdminPage");
            }

            ModelState.AddModelError("", "Форма заполнена неправильно");

            return View("EditFilm", form);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RemoveFilm(int id)
        {
            await _filmService.RemoveFilm(id);
            _notification = "Фильм успешно удален.";
            return RedirectToAction("AdminPage");
        }
    }
}