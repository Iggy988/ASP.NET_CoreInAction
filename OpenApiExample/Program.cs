using System.Collections.Concurrent;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen(x =>x.SwaggerDoc("v1", new OpenApiInfo()
{
  Title = "Fruitify",
  Description = "An API for interacting with fruit stock",
  Version = "1.0"
})); 

WebApplication app = builder.Build();

var _fruit = new ConcurrentDictionary<string, Fruit>();
app.UseSwagger(); 
app.UseSwaggerUI(); 
app.MapGet("/fruit/{id}", (string id) => _fruit.TryGetValue(id, out var fruit) ? TypedResults.Ok(fruit) : Results.Problem(statusCode: 404));
app.MapPost("/fruit/{id}", (string id, Fruit fruit) =>
_fruit.TryAdd(id, fruit) ? TypedResults.Created($"/fruit/{id}", fruit) : Results.ValidationProblem(new Dictionary<string, string[]>
{
  { "id", new[] { "A fruit with this id already exists" } }
}));
app.Run();
record Fruit(string Name, int Stock);