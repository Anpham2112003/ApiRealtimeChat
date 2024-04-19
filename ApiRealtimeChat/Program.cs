using MediatR;
using Infrastructure.Dependency;
using System.Reflection;
using AutoMapper;
using Application.Features.Account;
using Domain.Entites;
using Application.Ultils;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(op =>
    {
        op.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(op =>
{
    op.DefaultScheme=CookieAuthenticationDefaults.AuthenticationScheme;
    op.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;


})
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddGoogle(GoogleDefaults.AuthenticationScheme, op =>
    {
        op.ClientId = builder.Configuration["Google:Id"];
        op.ClientSecret = builder.Configuration["Google:Key"];
        op.CallbackPath = "/api/signin-google";
       



    })
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, op =>
    {
        op.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
        {
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:AccessKey"])),

        };

        
        
    });

builder.Services.AddMediatR(op =>
{
    op.RegisterServicesFromAssembly(typeof(CreateAccountCommand).Assembly);
});

builder.Services.AddAutoMapper(op => { op.AddMaps(typeof(MapData).Assembly); });

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

app.Run();
