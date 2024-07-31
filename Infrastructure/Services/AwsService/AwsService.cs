using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.AwsService
{
    public class AwsService : IAwsServices
    {
        private readonly IAmazonS3 _amazonS3;

        public AwsService(IAmazonS3 amazonS3)
        {
            _amazonS3 = amazonS3;
        }

        public async Task RemoveFileAsync(string bucket, string key)
        {
            var objectRequest = new DeleteObjectRequest()
            {
                BucketName = bucket,
                Key = key
            };
            await _amazonS3.DeleteObjectAsync(objectRequest);
        }

        public async Task UploadFileAsync(string bucket, string key, IFormFile file)
        {

            var objectRequest = new PutObjectRequest()
            {
                AutoCloseStream = true,
                BucketName = bucket,
                Key = key,
                CannedACL = S3CannedACL.PublicRead,
                AutoResetStreamPosition = true,
                InputStream = file.OpenReadStream()
            };

            await _amazonS3.PutObjectAsync(objectRequest);

        }
    }
}
