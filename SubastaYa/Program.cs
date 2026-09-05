using Application.Interfaces;
using Infraestructure.Persistence;
using Infraestructure.Persistence.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Application.UseCases.Billeteras.Handlers;
using Application.UseCases.Subastas.Handlers;
using Application.UseCases.Handlers;


var builder = WebApplication.CreateBuilder(args);

//Contexto de Base de Datos
builder.Services.AddDbContext<SubastaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

//Repositorios y UnitOfWork
builder.Services.AddScoped<ISubastaRepository, SubastaRepository>();
builder.Services.AddScoped<IBilleteraRepository, BilleteraRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<DepositCommandHandler>();
builder.Services.AddScoped<ListarBilleteraQueryHandler>();
builder.Services.AddScoped<CreateSubastaCommandHandler>();
builder.Services.AddScoped<CreateBidCommandHandler>();
builder.Services.AddScoped<ListarSubastasQueryHandler>();
builder.Services.AddScoped<ObtenerSubastaQueryHandler>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//Middlewares
app.UseMiddleware<SubastaYa.Middlewares.ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();