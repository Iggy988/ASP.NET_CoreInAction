var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseWelcomePage("/");
app.UseDeveloperExceptionPage();
app.UseStaticFiles();
app.UseRouting();

app.MapGet("/", () => "Welcome page");

app.Run();
