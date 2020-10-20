using System.ComponentModel.DataAnnotations;
using FilmCatalog.Models.Authorization;

namespace FilmCatalog.Models
{
    public class Like
    {
        [Key] public int LikeId { get; set; }
        [Required] public Film Film { get; set; }
        [Required] public ApplicationUser User { get; set; }
    }
}