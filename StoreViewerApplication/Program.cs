WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.Sources.Clear();
builder.Configuration.AddJsonFile("sharedSsettings.json", optional: true); 
builder.Configuration.AddJsonFile("appsettings.json", optional: true);
builder.Configuration.AddEnvironmentVariables();

var zoomLevel = builder.Configuration["MapSettings:DefaultZoomLevel"];
//var lat = builder.Configuration["MapSettings:DefaultLocation:Latitude"];
var lat = builder.Configuration.GetSection("MapSettings")["DefaultLocation:Latitude"];

var app = builder.Build();

app.MapGet("/", () => app.Configuration.AsEnumerable());

app.Run();
