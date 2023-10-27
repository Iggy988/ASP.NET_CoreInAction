var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<DataContext>();
var app = builder.Build();

var settings = app.Services.GetRequiredService<EmailServerSettings>();

builder.Host.UseDefaultServiceProvider(o => 
{
  o.ValidateScopes = true;
  o.ValidateOnBuild = true; 
});

await using (var scope = app.Services.CreateAsyncScope()) 
{
  var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>(); 
  Console.WriteLine($"Retrieved scope: {dbContext.RowCount}");
}

app.Run();
