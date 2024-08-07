﻿using Microsoft.Extensions.DependencyInjection;
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
using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.AspNetCore.Http;
using StackExchange.Redis;
using Infrastructure.Services.RedisSevices;
using Infrastructure.Services.FileService;
using Infrastructure.Services.MailService;
using Infrastructure.Services.AwsService;


namespace Infrastructure.InjectServices
{
    public static class DependencyInject
    {
        public static IServiceCollection addInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSetting>(configuration.GetSection(JwtSetting.Jwtseting));

            services.Configure<MongoDbSetting>(configuration.GetSection(MongoDbSetting.Setting));

            services.Configure<MailSetting>(configuration.GetSection(MailSetting.Setting));

            services.AddScoped<IMongoDB, MongoDBcontext>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            var awsOption = new AWSOptions()
            {
                Region = RegionEndpoint.APSoutheast2,
                Credentials = new BasicAWSCredentials(configuration["Aws:Id"], configuration["Aws:Key"]),
            };

            services.AddDefaultAWSOptions(awsOption);

            services.AddScoped<IAwsServices, AwsService>();

            services.AddAWSService<IAmazonS3>();

            services.AddScoped<IMailerServices, MailerServices>();

            services.AddHttpContextAccessor();

            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost"));

            services.AddScoped<IFileService, FileService>();

            services.AddScoped<IRedisService,RedisSevice>();

            return services;
        }
    }
}
