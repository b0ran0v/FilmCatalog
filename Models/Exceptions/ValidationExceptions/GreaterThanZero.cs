using System.ComponentModel.DataAnnotations;

namespace FilmCatalog.Models.Exceptions.ValidationExceptions
{
    public class GreaterThanZero : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var x = (int)value;
            return x > 0;
        }
    }
}