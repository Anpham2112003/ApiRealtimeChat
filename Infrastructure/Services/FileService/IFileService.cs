using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.FileService
{
    public interface IFileService
    {
        public  Task WriteFileAsync(string storePath, string fileName, IFormFile file);


        public void RemoveFile(string filepath);
    }
}
