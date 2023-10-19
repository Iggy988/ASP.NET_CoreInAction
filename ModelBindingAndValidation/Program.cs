using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(opt => {
  opt.SerializerOptions.AllowTrailingCommas = true;
  opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
  opt.SerializerOptions.PropertyNameCaseInsensitive = true;
});

var app = builder.Build();

//app.MapGet("/products/{id}/paged", ([FromRoute] int id, [FromQuery] int page, [FromHeader(Name="")] int pageSize) => $"Recived id {id}, page{page}, pageSize {pageSize}");
//app.MapGet("/product/{id}", (ProductId) => $"Recived {id}");

app.MapGet("/stock/{id?}", (int? id) => $"Received {id}"); 
app.MapGet("/stock2", (int? id) => $"Received {id}"); 
app.MapPost("/stock", (ProductId? product) => $"Received {product}");

app.MapGet("/stock", StockWithDefaultValue);

//app.MapGet("/products/search", ([FromQuery(Name = "id")] int[] id) => $"Received {id.Length} ids");


app.MapGet("/links", ([FromServices]LinkGenerator links) => 
{
  string link = links.GetPathByName("products");
  return $"View the product at {link}";
});

app.MapGet("/upload", (IFormFileCollection files) =>
{
  foreach (IFormFile file in files)
  {
  }
});
string StockWithDefaultValue(int id = 0) => $"Received {id}";

app.MapPost("/sizes", (SizeDetails size) => $"Received {size}");

app.Run();

public record SizeDetails(double height, double width) 
{
  public static async ValueTask<SizeDetails?> BindAsync(HttpContext context) 
  {
    using var sr = new StreamReader(context.Request.Body); 
    string? line1 = await sr.ReadLineAsync(context.RequestAborted); 
    if (line1 is null) { return null; } 
    string? line2 = await sr.ReadLineAsync(context.RequestAborted); 
    if (line2 is null) { return null; } 
    return double.TryParse(line1, out double height) && double.TryParse(line2, out double width) 
      ? new SizeDetails(height, width) 
      : null; 
  }
}

public interface IFormFile
{
      string ContentType { get; }
      long Length { get; }
      string FileName { get; }
      Stream OpenReadStream();
}

readonly record struct ProductId(int id)
{
  public static bool TryParse(string? s, out ProductId result)
  {
    if(s is not null && s.StartsWith('p') && int.TryParse(s.AsSpan().Slice(1), out int id))
    {
      result = new ProductId(id);
      return true;
    }
    result = default;
    return false;
  }
}


