var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

builder.Host.UseDefaultServiceProvider(o => 
{
  o.ValidateScopes = true;
  o.ValidateOnBuild = true; 
});

app.Run();
