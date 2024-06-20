
using Application.Errors;
using Domain.Entities;
using Domain.ResponeModel;
using Domain.Ultils;
using Infrastructure.Services;
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
        public HandUpdateAvartaUser(IUnitOfWork unitOfWork, IAwsServices awsServices, IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _awsServices = awsServices;
            _configuration = configuration;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<UploadAvatarResponeModel>> Handle(UpdateAvatarUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _contextAccessor.HttpContext!.User.GetIdFromClaim();

                StringBuilder key = new StringBuilder();
                key.Append("avatar");
                key.Append(userId);
                key.Append(request.Image!.Name);
                key.Append(ObjectId.GenerateNewId());
                key.Append(Path.GetExtension(request.Image!.FileName));

                var awsUrl = _configuration["Aws:Perfix"] + key.ToString();

                await _awsServices.UploadFileAsync(_configuration["Aws:Bucket"]!, key.ToString(), request.Image);

                await _unitOfWork.userRepository.UpdateAvatarUser(userId, awsUrl);

              
                return Result<UploadAvatarResponeModel>
                    .Success(new UploadAvatarResponeModel("Upload success!", userId, awsUrl));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            
            
        }

       
    }
}
