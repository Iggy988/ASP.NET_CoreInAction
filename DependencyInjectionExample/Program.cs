var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/register/{username}", RegisterUser);

app.Run();


string RegisterUser(string username)
{
  var emailSender = new EmailSender()
  emailSender.SendEmail(username),;
  return $"Email sent to {username}";
}

public classEmailSender
{
  public void SendEmail(string username)
  {
    Console.WriteLine($"Email sent to {username}!");
  }
}
