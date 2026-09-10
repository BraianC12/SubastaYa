using Application.Interfaces;
using Application.UseCases.Billeteras.Handlers;
using Application.UseCases.Handlers;
using Application.UseCases.Subastas.Handlers;
using Application.UseCases.Usuarios.Handlers;
using Infraestructure.Persistence;
using Infraestructure.Persistence.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//contexto de Base de Datos
builder.Services.AddDbContext<SubastaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//repositorios y UnitOfWork
builder.Services.AddScoped<ISubastaRepository, SubastaRepository>();
builder.Services.AddScoped<IBilleteraRepository, BilleteraRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuditorialLogRepository, AuditoriaLogRepository>();
builder.Services.AddScoped<ITransaccionLedgerRepository, TransaccionLedgerRepository>();
builder.Services.AddScoped<IAdjudicarSubastasCommandHandler, AdjudicarSubastasCommandHandler>();


//handlers por Interfaz (Arquitectura Limpia)
builder.Services.AddScoped<IDepositCommandHandler, DepositCommandHandler>();
builder.Services.AddScoped<IListarBilleteraQueryHandler, ListarBilleteraQueryHandler>();
builder.Services.AddScoped<ICreateSubastaCommandHandler, CreateSubastaCommandHandler>();
builder.Services.AddScoped<ICreateBidCommandHandler, CreateBidCommandHandler>();
builder.Services.AddScoped<IListarSubastasQueryHandler, ListarSubastasQueryHandler>();
builder.Services.AddScoped<IObtenerSubastaQueryHandler, ObtenerSubastaQueryHandler>();
builder.Services.AddScoped<IGetUserQueryHandler, GetUserQueryHandler>();
builder.Services.AddScoped<ICreateUserCommandHandler, CreateUserCommandHandler>();
builder.Services.AddHostedService<SubastaYa.Workers.AdjudicacionWorker>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middlewares
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