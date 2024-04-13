using MediatR;
using Infrastructure.Dependency;
using System.Reflection;
using AutoMapper;
using Application.Features.Account;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddMediatR(op =>
{
    op.RegisterServicesFromAssembly(Assembly.GetAssembly(typeof(CreateAccountCommand)));
});

builder.Services.AddAutoMapper(Assembly.GetAssembly(typeof(Mapper)));

builder.Services.addInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
