using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Ultils
{
    public static class GenerateRandomFileName
    {
        public static IEnumerable<string> GenerateFromFiles(IEnumerable<IFormFile> files)
        {
            if(!files.Any()) return Enumerable.Empty<string>();

            var fileNames = new List<string>();

            foreach (var file in files)
            {
                var fileName = Guid.NewGuid().ToString()+Path.GetExtension(Path.GetFileName(file.FileName));

                fileNames.Add(fileName);
            }

            return fileNames;
        }

        public static string GenerateFromFile(IFormFile file)
        {
            if (file is null) return String.Empty;

            var fileName = Guid.NewGuid().ToString() +Path.GetExtension(Path.GetFileName(file.FileName));

            return fileName;
        }
    }
}
