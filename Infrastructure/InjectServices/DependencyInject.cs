using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Domain.Settings;
using Infrastructure.MongoDBContext;
using Infrastructure.Unit0fWork;
using System.Reflection;


namespace Infrastructure.Dependency
{
    public static class DependencyInject
    {
        public static IServiceCollection addInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSetting>(configuration.GetSection(JwtSetting.Jwtseting));

            services.Configure<MongoDbSetting>(configuration.GetSection(MongoDbSetting.Setting));


            services.AddScoped<IMongoDB, MongoDBcontext>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

           

            return services;
        }
    }
}
