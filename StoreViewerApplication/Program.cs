WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.Source.Clear();
builder.Configuration.AddJsonFile("appsettings.json", optional: true);

var app = builder.Build();

app.MapGet("/", () => app.Configuration.AsEnumerable());

app.Run();
