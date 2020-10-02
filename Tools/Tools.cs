using System.IO;
using Microsoft.AspNetCore.Http;

namespace FilmCatalog.Tools
{
    public class Tools
    {
        public static byte[] ReadImage(IFormFile file)
        {
            var memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            var bytes = memoryStream.ToArray();
            return bytes;
        }
    }
}