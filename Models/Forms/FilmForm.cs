using System.ComponentModel.DataAnnotations;
using FilmCatalog.Models.Authorization;
using Microsoft.AspNetCore.Http;

namespace FilmCatalog.Models.Forms
{
    public class FilmForm
    {
        [Required] public string Title { get; set; }
        [Required] public string Description { get; set; }
        [Required] public int YearPublished { get; set; }
        [Required] public string Director { get; set; }
        public IFormFile Poster { get; set; }
        public int FilmId { get; set; }
    }
}