using Application.Interfaces;
using Infraestructure.Data;
using Infraestructure.Repositories;
using Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

/*Contexto*/
builder.Services.AddDbContext<SubastaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    );

//Repositorios
builder.Services.AddScoped<ISubastaRepository, SubastaRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();





builder.Services.AddScoped<ISubastaRepository, Infrastructure.Persistence.Repositories.SubastaRepository>();

builder.Services.AddScoped<IUnitOfWork, Infrastructure.Persistence.UnitOfWork>();

builder.Services.AddScoped<IBilleteraRepository, Infraestructure.Persistence.Repositories.BilleteraRepository>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Application.UseCases.Subastas.Commands.CreateSubastaCommand).Assembly));


var app = builder.Build();

app.UseMiddleware<SubastaYa.Middlewares.ExceptionMiddleware>();



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
