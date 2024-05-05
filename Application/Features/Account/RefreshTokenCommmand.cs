using Application.Errors;
using Application.Ultils;
using Domain.ResponeModel;
using Domain.Settings;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Account
{
    public class RefreshTokenCommmand:IRequest<Result<LoginResponseModel>>
    {
        public string? Token {  get; set; }
    }

    public class HandRefreshTokenCommand : IRequestHandler<RefreshTokenCommmand, Result<LoginResponseModel>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptionsMonitor<JwtSetting> _options;

        public HandRefreshTokenCommand(IUnitOfWork unitOfWork, IOptionsMonitor<JwtSetting> options)
        {
            _unitOfWork = unitOfWork;
            _options = options;
        }

        public async Task<Result<LoginResponseModel>> Handle(RefreshTokenCommmand request, CancellationToken cancellationToken)
        {
            try
            {
                

                var resultValidate = JwtLibrary.TryValidateToken(request.Token!,_options.CurrentValue.ReFreshKey!, out var claims);

                if (!resultValidate.Equals(Error.None)) return Result<LoginResponseModel>.Failuer(resultValidate);

                var accesssToken = JwtLibrary.GenerateToken(_options.CurrentValue.AccessKey!, claims!.Claims, DateTime.UtcNow.AddMinutes(1));
                var refreshToken = JwtLibrary.GenerateToken(_options.CurrentValue.ReFreshKey!,claims.Claims, DateTime.UtcNow.AddDays(7));

                return Result<LoginResponseModel>.Success();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
