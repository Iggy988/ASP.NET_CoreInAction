var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

//app.MapGet("/products/{id}/paged", ([FromRoute] int id, [FromQuery] int page, [FromHeader(Name="")] int pageSize) => $"Recived id {id}, page{page}, pageSize {pageSize}");

app.MapGet("/product/{id}", (ProductId) => $"Recived {id}");

app.Run();


readonly record struct ProductId(int id, string Name, int Stock)
{
  public static bool TryParse(string? s, out ProductId result)
  {
    if(s is not null && s.StartsWith('p') && int.TryParse(s.AsSpan().Slice(1), out int id)))
    {
      result = new Product(id);
      return true;
    }
    result = default;
    return false;
  }
}
