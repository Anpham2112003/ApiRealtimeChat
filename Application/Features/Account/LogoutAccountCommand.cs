using Domain.Ultils;
using Infrastructure.Services.RedisSevices;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Account
{
    public class LogoutAccountCommand:IRequest<Result<string>>
    {
        public string token { get; set; }

        public LogoutAccountCommand(string token)
        {
            this.token = token;
        }
    }
    public class HandLogoutAccountCommand : IRequestHandler<LogoutAccountCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisService _redisService;
        private readonly IConfiguration _configuration;

        public HandLogoutAccountCommand(IUnitOfWork unitOfWork, IRedisService redisService, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _redisService = redisService;
            _configuration = configuration;
        }

        public async Task<Result<string>> Handle(LogoutAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var validatetoken = JwtLibrary.TryValidateToken(request.token, _configuration["Jwt:RefreshKey"]!, out var claims);

                if (!validatetoken) return Result<string>.Failuer(new Error("Unauthori", ""));

                var user = claims!.GetIdFromClaim();

                await _unitOfWork.accountRepository.UpdateTokenUser(user!, "");

                await _redisService.RemoveHash(user!);

                return Result<string>.Success("Ok!");
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
