using System.Linq;
using System.Threading.Tasks;
using FilmCatalog.Data;
using FilmCatalog.Models.Forms;
using FilmCatalog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            FilmService filmService)
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
                _notification = await _filmService.AddNewFilm(model);
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
            return form == null ? (IActionResult)new NotFoundResult() : View(form);
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFilm(FilmForm form)
        {
            if (ModelState.IsValid)
            {
                _notification = await _filmService.EditFilm(form);
                return _notification == null ? (IActionResult)new NotFoundResult() : RedirectToAction("AdminPage");
            }

            ModelState.AddModelError("", "Форма заполнена неправильно");

            return View("EditFilm", form);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFilm(int id)
        {
            _notification = await _filmService.RemoveFilm(id);
            return _notification == null ? (IActionResult)new NotFoundResult() : RedirectToAction("AdminPage");
        }
    }
}