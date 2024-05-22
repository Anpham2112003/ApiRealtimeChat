using MediatR;
using System.Reflection;
using AutoMapper;
using Application.Features.Account;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;
using Domain.Settings;
using Microsoft.OpenApi.Models;
using Infrastructure.Services.HubServices;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Diagnostics;
using ApiRealtimeChat.Controllers;
using Domain.Ultils;
using Infrastructure.InjectServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(op =>
    {
        op.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(op =>
{
    op.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
    {
        Version = "v1",
        Title = "RealtimeChat",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Email = "anpham2112003@gmail.com",
            Name = "An",
        },
    });

    op.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter a valild token",
        Name = "Authorization",
        Scheme = "Bearer"
    });
    op.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddAuthentication(op =>
{
    op.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    op.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

})
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, op =>
    {
        op.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
        {
            ValidateLifetime = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:AccessKey"])),
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidAudience = JwtSetting.Audience,
            ValidIssuer = JwtSetting.Isssser
        };

        op.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/Chat"))
                {
                    Debug.WriteLine(path);
                    context.Token = accessToken;
                    
                }
                return Task.CompletedTask;
            }
        };
        
    })
    .AddGoogle(GoogleDefaults.AuthenticationScheme, op =>
    {
        
        op.ClientId = builder.Configuration["Google:Id"];
        op.ClientSecret = builder.Configuration["Google:Key"];
        op.CallbackPath = "/api/signin-google";
       
    })
    
    .AddCookie();


builder.Services.AddSignalR();

builder.Services.AddMediatR(op =>
{
    op.RegisterServicesFromAssembly(typeof(CreateAccountCommand).Assembly);
});


builder.Services.addInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();



app.MapControllers();

app.MapHub<HubService>("/Chat");

app.Run();
