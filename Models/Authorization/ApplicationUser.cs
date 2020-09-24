using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace FilmCatalog.Models.Authorization
{
    public class ApplicationUser: IdentityUser
    {
        public virtual ICollection<Film> Films { get; set; }
    }
}