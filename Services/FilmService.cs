using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FilmCatalog.Controllers;
using FilmCatalog.Data;
using FilmCatalog.Models;
using FilmCatalog.Models.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FilmCatalog.Services
{
    public class FilmService
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _ctxAccessor;

        public FilmService(ILogger<HomeController> logger, ApplicationDbContext context,
            IHttpContextAccessor ctxAccessor)
        {
            _logger = logger;
            _context = context;
            _ctxAccessor = ctxAccessor;
        }

        public async Task AddNewFilm(FilmForm model)
        {
            var film = new Film
            {
                Title = model.Title,
                YearPublished = model.YearPublished,
                Description = model.Description,
                Director = model.Director,
                User = await _context.Users.FirstOrDefaultAsync(user =>
                    user.UserName == _ctxAccessor.HttpContext.User.Identity.Name)
            };

            if(model.Poster != null) film.Poster = Tools.Tools.ReadImage(model.Poster);
            
            await _context.Films.AddAsync(film);
            await _context.SaveChangesAsync();
        }

        public async Task<FilmForm> EditFilm(int id)
        {
            var film = await _context.Films.Include(f => f.User).FirstOrDefaultAsync(f => f.FilmId == id);

            var form = new FilmForm
            {
                Title = film.Title,
                YearPublished = film.YearPublished,
                Director = film.Director,
                Description = film.Description,
                FilmId = film.FilmId
            };

            if (film.Poster != null)
            {
                var stream = new MemoryStream(film.Poster);
                IFormFile file = new FormFile(stream, 0, film.Poster.Length, "name", "fileName");
                form.Poster = file;
            }

            return form;
        }

        public async Task EditFilm(FilmForm form)
        {
            var film = await _context.Films.Include(f => f.User).FirstOrDefaultAsync(f => f.FilmId == form.FilmId);

            if (film == null) return;

            film.Title = form.Title;
            film.YearPublished = form.YearPublished;
            film.Description = form.Description;
            film.Director = form.Director;
            if (form.Poster != null) film.Poster = Tools.Tools.ReadImage(form.Poster);
            _context.Update(film);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFilm(int id)
        {
            var film = await _context.Films.Include(f => f.User).FirstOrDefaultAsync(f => f.FilmId == id);

            if (film == null) return;
            _context.Films.Attach(film);
            _context.Films.Remove(film);
            await _context.SaveChangesAsync();
        }
    }
}