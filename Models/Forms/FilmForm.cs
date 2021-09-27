using System.ComponentModel.DataAnnotations;
using FilmCatalog.Models.Exceptions.ValidationExceptions;
using Microsoft.AspNetCore.Http;

namespace FilmCatalog.Models.Forms
{
    public class FilmForm
    {
        [Required] public string Title { get; set; }

        [Required] public string Description { get; set; }

        [Required]
        [GreaterThanZero(ErrorMessage = "Only positive number allowed.")]
        public int YearPublished { get; set; }

        [Required] public string Director { get; set; }

        public IFormFile Poster { get; set; }

        public long FilmId { get; set; }
    }
}