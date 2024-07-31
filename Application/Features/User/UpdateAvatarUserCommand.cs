
using Application.Errors;
using Domain.Entities;
using Domain.ResponeModel;
using Domain.Ultils;
using Infrastructure.Services.AwsService;
using Infrastructure.Services.FileService;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.User
{
    public class UpdateAvatarUserCommand:IRequest<Result<UploadAvatarResponeModel>>
    {
        public IFormFile? Image {  get; set; }
    }

    public class HandUpdateAvartaUser : IRequestHandler<UpdateAvatarUserCommand, Result<UploadAvatarResponeModel>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAwsServices _awsServices;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IFileService _fileService;
        public HandUpdateAvartaUser(IUnitOfWork unitOfWork, IAwsServices awsServices, IConfiguration configuration, IHttpContextAccessor contextAccessor, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _awsServices = awsServices;
            _configuration = configuration;
            _contextAccessor = contextAccessor;
            _fileService = fileService;
        }

        public async Task<Result<UploadAvatarResponeModel>> Handle(UpdateAvatarUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _contextAccessor.HttpContext!.User.GetIdFromClaim();

                if (request.Image ==null) return Result<UploadAvatarResponeModel>.Failuer(new Error("File", "File null"));

                var fileName = GenerateRandomFileName.GenerateFromFile(request.Image!);

               

               var storePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads","Images");

                var filePath = Path.Combine("Uploads","Images", fileName);

                await _fileService.WriteFileAsync(storePath,fileName, request.Image!);

                await _unitOfWork.userRepository.UpdateAvatarUser(userId, filePath);

                //var awsUrl = _configuration["Aws:Perfix"] + key.ToString();

                //await _awsServices.UploadFileAsync(_configuration["Aws:Bucket"]!, fileName.ToString(), request.Image!);


                return Result<UploadAvatarResponeModel>
                    .Success(new UploadAvatarResponeModel("Upload success!", userId, filePath));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            
            
        }

       
    }
}
