var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/register/{username}", RegisterUser);

app.Run();


string RegisterUser(string username, EmailSender emailSender) 
{
  emailSender.SendEmail(username); 
  return $"Email sent to {username}!"; 
}

public classEmailSender
{
  private readonly NetworkClient _client;
  private readonly MessageFactory _factory; 
  public EmailSender(MessageFactory factory, NetworkClient client); 

  
  public void SendEmail(string username)
  {
    var email = _factory.Create(username); 
    _client.SendEmail(email);
    Console.WriteLine($"Email sent to {username}!");
  }
}

public class NetworkClient
{
  private readonly EmailServerSettings _settings;
  public NetworkClient(EmailServerSettings settings)
  {
    _settings = settings;
  }
}
