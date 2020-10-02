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
using Microsoft.EntityFrameworkCore;

namespace FilmCatalog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            return View(await PaginatedList<Film>.CreateAsync(_context.Films, pageNumber, 3));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        public IActionResult Film(int id)
        {
            var film = _context.Films.Include(f => f.User).FirstOrDefault(f => f.FilmId == id);
            if (film == null) return StatusCode(404);
            return View(film);
        }
    }
}