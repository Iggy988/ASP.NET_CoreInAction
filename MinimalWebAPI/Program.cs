using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var _fruit = new ConcurrentDictionary<string, Fruit>(); 

app.MapGet("/fruit", () => /*Fruit.All*/ _fruit);

//var getFruit = (string id) => Fruit.All[id];
app.MapGet("/fruit/{id}", /*getFruit*/ (string id) => 
            _fruit.TryGetValue(id, out var fruit) ? TypedResults.Ok(fruit) : Results.Problem(statusCode: 404))
                        .AddEndpointFilter(ValidationHelper.ValidateId)
                        .AddEndpointFilter(async(context, next) =>
                        {
                                    app.Logger.LogInformation("Executing filter...");
                                    object? result = await next(context);
                                    app.Logger.LogInformation($"Handler result: {result}");
                                    return result;
                        });

app.MapPost("/fruit/{id}", /*Handlers.AddFruit*/ (string id, Fruit fruit) =>
            _fruit.TryAdd(id, fruit)
                        ? TypedResults.Created($"/fruit/{id}", fruit)
                        : Results.ValidationProblem(new Dictionary<string, string[]> 
                                    { 
                                                {"id", new[] {"A fruit with this id already exists"}} 
                                    }));

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

class ValidationHelper
{
            internal static async ValueTask<object?> ValidateId( EndpointFilterInvocationContext context, EndpointFilterDelegate next) #D
            {
                        var id = context.GetArgument<string>(0); 
                        if (string.IsNullOrEmpty(id) || !id.StartsWith('f'))
                        {
                                    return Results.ValidationProblem(
                                                new Dictionary<string, string[]>
                                                {
                                                            {"id", new[]{"Invalid format. Id must start with 'f'"}}
                                                });
                        }
                        return await next(context); 
            }
}


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
