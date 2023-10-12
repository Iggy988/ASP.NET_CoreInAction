using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var _fruit = new ConcurrentDictionary<string, Fruit>(); 

app.MapGet("/fruit", () => /*Fruit.All*/ _fruit);

//var getFruit = (string id) => Fruit.All[id];
app.MapGet("/fruit/{id}", /*getFruit*/ (string id) => 
            _fruit.TryGetValue(id, out var fruit) ? TypedResults.Ok(fruit) : Results.Problem(statusCode: 404));

app.MapPost("/fruit/{id}", /*Handlers.AddFruit*/ (string id, Fruit fruit) =>
{
            if (string.IsNullOrEmpty(id) || !id.StartsWith('f')) 
            {
                        return Results.ValidationProblem(new Dictionary<string, string[]>
                        {
                        {"id", new[] {"Invalid format. Id must start with 'f'"}}
                        });
            }
            return _fruit.TryGetValue(id, out var fruit)
                        ? TypedResults.Ok(fruit)
                        : Results.Problem(statusCode: 404);
});

//Handlers handlers = new Handlers();
app.MapPut("/fruit/{id}", /*handlers.ReplaceFruit*/ (string id, Fruit fruit) =>
{
    _fruit[id] = fruit;
    return Results.NoContent();
});

app.MapDelete("/fruit/{id}", /*handlers.DeleteFruit*/ (string id) => 
{
    _fruit.TryRemove(id, out _);
    return Results.NoContent();
});


app.Run();


record Fruit(string Name, int Stock)
{
    public static readonly Dictionary<string, Fruit> All = new();
};

class Handlers
{
    public void ReplaceFruit(string id, Fruit fruit) 
    {
        Fruit.All[id] = fruit;
    }
    public static void AddFruit(string id, Fruit fruit) 
    {
        Fruit.All.Add(id, fruit);
    }
    public void DeleteFruit(string id) 
    {
        Fruit.All.Remove(id);
    }
}
