builder.Services.AddScoped( provider => new EmailServerSettings ( 
  Host: "smtp.server.com", 
  Port: 25 
));

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

var app = builder.Build();



app.MapGet("/", () => "Hello World!");

app.MapGet("/register/{username}", RegisterUser);

app.MapGet("/links", (LinkGenerator links) => 
{
  string link = links.GetPathByName("products");
  return $"View the product at {link}";
});

app.MapRazorPages();

app.Run();


string RegisterUser(string username, IEmailSender emailSender) 
{
  emailSender.SendEmail(username); 
  return $"Email sent to {username}!"; 
}

public interface IEmailSender
{
  public void SendEmail(string username);
}

