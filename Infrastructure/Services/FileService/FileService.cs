using Domain.Enums;
using Infrastructure.InjectServices;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.FileService
{
    public class FileService : IFileService
    {
        

        public  void RemoveFile(string filepath)
        {
			try
			{
				 File.Delete(filepath);
			}
			catch (Exception)
			{

				throw;
			}
        }

        public async Task WriteFileAsync(string storePath,string fileName, IFormFile file)
        {
			try
			{
				

				if(!Directory.Exists(storePath))
				{
					Directory.CreateDirectory(storePath);
				}

				var filepath = Path.Combine(storePath, fileName);

				using(var stream = new FileStream(filepath,FileMode.Create))
				{
					await file.CopyToAsync(stream);
				}

			}
			catch (Exception)
			{

				throw;
			}
        }
    }
}
