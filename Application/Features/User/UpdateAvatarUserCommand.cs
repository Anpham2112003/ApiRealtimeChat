
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
        public string? AccountId { get; set; }
        public IFormFile? Image {  get; set; }
    }

    public class HandUpdateAvartaUser : IRequestHandler<UpdateAvatarUserCommand, Result<UploadAvatarResponeModel>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAwsServices _awsServices;
        private readonly IConfiguration _configuration;

        public HandUpdateAvartaUser(IUnitOfWork unitOfWork, IAwsServices awsServices, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _awsServices = awsServices;
            _configuration = configuration;
        }

        public async Task<Result<UploadAvatarResponeModel>> Handle(UpdateAvatarUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                StringBuilder key = new StringBuilder();
                key.Append("avatar");
                key.Append(request.AccountId);
                key.Append(request.Image!.Name);
                key.Append(Random.Shared.Next(1, 100000000).ToString());
                key.Append(Path.GetExtension(request.Image!.FileName));

                await _awsServices.UploadFileAsync(_configuration["Aws:Bucket"]!, key.ToString(), request.Image);

                var filter = Builders<UserCollection>.Filter.Eq(x => x.AccountId,request.AccountId);

                var update = Builders<UserCollection>.Update.Set(x => x.Avatar, _configuration["Aws:Perfix"] + key.ToString());

                var result = await _unitOfWork.userRepository.UpdateAsync(filter, update);

                if (result.ModifiedCount == 0) return Result<UploadAvatarResponeModel>.Failuer(UserError.UserNotFound(request.AccountId!));

                return Result<UploadAvatarResponeModel>
                    .Success(new UploadAvatarResponeModel("Upload success!", request.AccountId, _configuration["Aws:Perfix"]?.ToString() +key));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            
            
        }

       
    }
}
