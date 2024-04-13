using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entites;
using MongoDB.Bson;
using Infrastructure.Unit0fWork;
using AutoMapper;
namespace Application.Features.Account
{
    public class CreateAccountCommand:IRequest<CreateAccountCommand>
    {
        public ObjectId Id = ObjectId.GenerateNewId();
        public string? FistName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public DateTime Created => DateTime.UtcNow;
        public bool IsDelete => false;
    }

    public class HandCreateAccountCommand : IRequestHandler<CreateAccountCommand, CreateAccountCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public HandCreateAccountCommand(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CreateAccountCommand> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            var user = _mapper.Map<AccountCollection>(request);
            var transaction = await _unitOfWork.TransactionAsync();
            await transaction.WithTransactionAsync(async (x, y) =>
            {
                await _unitOfWork.accountRepository.InsertAsync(user);
                return "success";
            });

            return request;
        }
    }
}
