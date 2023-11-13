using EntityFrameworkExample;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer();
});

var app = builder.Build();



app.MapGet("/", () => "Hello World!");

app.Run();
