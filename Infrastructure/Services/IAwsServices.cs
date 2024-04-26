using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services
{
    public interface IAwsServices
    {
        public Task UploadFileAsync(string bucket,string key, IFormFile file);
        public Task RemoveFileAsync(string bucket,string key);
    }
}
