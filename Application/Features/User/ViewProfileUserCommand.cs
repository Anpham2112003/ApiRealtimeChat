using Domain.ResponeModel;
using Domain.Ultils;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.User
{
    public class ViewProfileUserCommand:IRequest<Result<ViewProfileResponeModel>>
    {
        public string Id { get; set; }

        public ViewProfileUserCommand(string id)
        {
            Id = id;
        }
    }

    public class HandViewProfileUserCommand : IRequestHandler<ViewProfileUserCommand, Result<ViewProfileResponeModel>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        public HandViewProfileUserCommand(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<ViewProfileResponeModel>> Handle(ViewProfileUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _contextAccessor.HttpContext!.User.GetIdFromClaim();

                var result = await _unitOfWork.userRepository.ViewProfileUser(request.Id, userId);

                if (request is null) return Result<ViewProfileResponeModel>.Failuer(new Error("Not found", ""));

                return Result<ViewProfileResponeModel>.Success(result);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
