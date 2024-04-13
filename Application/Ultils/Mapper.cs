using Application.Features.Account;
using AutoMapper;
using Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Ultils
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<CreateAccountCommand, AccountCollection>()
                .ForMember(x => x.Id, m => m.MapFrom(x => x.Id))
                .ForMember(x => x.Email, m => m.MapFrom(x => x.Email))
                .ForMember(x => x.Password, m => m.MapFrom(x => x.Password))
                .ForMember(x => x.CreatedAt, m => m.MapFrom(x => x.Created))
                .ForMember(x=>x.IsDelete,m=>m.MapFrom(x=>x.IsDelete))
                .ForMember(x=>x.DeletedAt,m=>m.Ignore());
                
            
        }
    }
}
