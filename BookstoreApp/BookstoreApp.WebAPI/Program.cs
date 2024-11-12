using BookstoreApp.Application.Interfaces;
using BookstoreApp.Application.Services;
using BookstoreApp.Infrastructure.Persistence;
using BookstoreApp.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
// Register the BookstoreDbContext with EF Core
builder.Services.AddDbContext<BookstoreDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("BookstoreApp.WebAPI")));
// Register the BookRepository
builder.Services.AddScoped<IBookRepository, BookRepository>();
// Register the Static BookService
builder.Services.AddSingleton<IBookService, BookService>();
// Register the BookService from the Infrastructure Layer
builder.Services.AddScoped<IBookService, BookServiceDB>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
