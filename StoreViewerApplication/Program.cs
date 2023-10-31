var builder = WebApplication.CreateBuilder(args);

builder.Configuration.Sources.Clear();
//builder.Configuration.AddJsonFile("sharedSsettings.json", optional: true); 
builder.Configuration.AddJsonFile("appsettings.json", optional: true);
builder.Configuration.AddEnvironmentVariables();

var zoomLevel = builder.Configuration["MapSettings:DefaultZoomLevel"];
//var lat = builder.Configuration["MapSettings:DefaultLocation:Latitude"];
var lat = builder.Configuration.GetSection("MapSettings")["DefaultLocation:Latitude"];

//dotnet user-secrets init
//dotnet user-secrets set "MapSettings:GoogleMapsApiKey" F5RJT9GFHKR7
if (builder.Environment.IsDevelopment())
{
  builder.Configuration.AddUserSecrets<Program>();
}

builder.Services.Configure<MapSettings>(builder.Configuration.GetSection("MapSettings"));
builder.Services.Configure<AppDisplaySettings>(builder.Configuration.GetSection("AppDisplaySettings"));
builder.Services.Configure<List<Store>>(builder.Configuration.GetSection("Stores"));

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/stores", (IOptions<List<Store>> opts) => opts.Value);
app.MapGet("/map-settings", (IOptions<MapSettings> opts) => opts.Value);
app.MapGet("/display-settings", (IOptions<AppDisplaySettings> opts) => opts.Value);

// Don't favour this approach
app.MapGet("/display-settings-alt", (IConfiguration config) => new
{
    title = config["AppDisplaySettings:Title"],
    showCopyright = bool.Parse(
        config["AppDisplaySettings:ShowCopyright"]!),
});

app.MapGet("/display-settings",
(IOptionsSnapshot<AppDisplaySettings> options) => 
{
  AppDisplaySettings settings = options.Value; 
  return new
  {
    title = settings.Title, 
    showCopyright = settings.ShowCopyright, 
  };
});

app.Run();

class MapSettings
{
    public string GoogleMapsApiKey { get; set; }
    public int DefaultZoomLevel { get; set; }
    public Location DefaultLocation { get; set; }
}
class Location
{
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
}
class Store
{
    public string Name { get; set; }
    public Location Location { get; set; }
}
class AppDisplaySettings
{
    public string Title { get; set; }
    public bool ShowCopyright { get; set; }
}
