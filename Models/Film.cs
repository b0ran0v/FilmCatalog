using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FilmCatalog.Models.Authorization;

namespace FilmCatalog.Models
{
    public class Film
    {
        [Key] public int FilmId { get; set; }
        [Required] public string Title { get; set; }
        [Required] public string Description { get; set; }
        [Required] public int YearPublished { get; set; }
        [Required] public string Director { get; set; }
        [Required] public ApplicationUser User { get; set; }
        [Required] public byte[] Poster { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
    }
}