using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FilmCatalog.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FilmCatalog.Models;
using FilmCatalog.Models.Forms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace FilmCatalog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            return View(await PaginatedList<Film>.CreateAsync(_context.Films, pageNumber, 3));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }


        [HttpGet]
        public IActionResult Film(int id)
        {
            var film = _context.Films.Include(f => f.User).
                Include(f => f.Likes).FirstOrDefault(f => f.FilmId == id);
            if (film == null) return NotFound();
            string likeClass = "fa fa-thumbs-up";
            ViewData["LikeClass"] = likeClass;
            if (User.Identity.IsAuthenticated)
            {
                ViewData["LikeClass"] = CurrentUserLiked(film.FilmId) ? $"{likeClass} fa fa-thumbs-down" : likeClass;   
            }
            return View(film);
        }
        
        [HttpGet]
        [Authorize]
        public IActionResult Like(int id)
        {
            var film = _context.Films.Include(f => f.User).
                Include(f => f.Likes).FirstOrDefault(f => f.FilmId == id);
            if (film == null) return NotFound();

            if (!CurrentUserLiked(film.FilmId))
            {
                Like newLike = new Like
                {
                    Film = film, 
                    User = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name)
                };
                _context.Likes.Add(newLike);
            }
            else
            {
                var likeToRemove = _context.Likes.Include(l => l.User).Include(l => l.Film)
                    .FirstOrDefault(l => l.Film.FilmId == id && l.User.UserName == User.Identity.Name) ?? new Like();
                _context.Attach(likeToRemove);
                _context.Likes.Remove(likeToRemove);   
            }

            _context.SaveChanges();
            return RedirectToAction("Film", new {id});
        }

        public bool CurrentUserLiked(int id)
        {
            return _context.Likes.Include(f=>f.User).Include(f=>f.Film).
                FirstOrDefault(l => l.Film.FilmId==id && l.User.UserName == User.Identity.Name)!=null;
        }
    }
}