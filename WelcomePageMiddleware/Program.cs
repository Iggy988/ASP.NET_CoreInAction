using System.Runtime.CompilerServices;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}

app.MapHealthChecks("/healthz");

app.UseStatusCodePages();
//app.UseWelcomePage("/");
//app.UseDeveloperExceptionPage();
//app.UseStaticFiles();
//app.UseRouting();

//app.MapGet("/", () => "Welcome page");
//app.MapGet("/", void () => throw new Exception());
app.MapGet("/", () => Results.NotFound());

app.Run();
