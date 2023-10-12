using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var _fruit = new ConcurrentDictionary<string, Fruit>(); 

app.MapGet("/fruit", () => /*Fruit.All*/ _fruit);

//var getFruit = (string id) => Fruit.All[id];
app.MapGet("/fruit/{id}", /*getFruit*/ (string id) => 
            _fruit.TryGetValue(id, out var fruit) ? TypedResults.Ok(fruit) : Results.Problem(statusCode: 404))
                        .AddEndpointFilterFactory(ValidationHelper.ValidateIdFactory);
                        //.AddEndpointFilter(ValidationHelper.ValidateId)
                        //.AddEndpointFilter(async(context, next) =>
                        //{
                         //           app.Logger.LogInformation("Executing filter...");
                         //           object? result = await next(context);
                         //           app.Logger.LogInformation($"Handler result: {result}");
                         //           return result;
                       // });


app.MapPost("/fruit/{id}", /*Handlers.AddFruit*/ (string id, Fruit fruit) =>
            _fruit.TryAdd(id, fruit)
                        ? TypedResults.Created($"/fruit/{id}", fruit)
                        : Results.ValidationProblem(new Dictionary<string, string[]> 
                                    { 
                                                {"id", new[] {"A fruit with this id already exists"}} 
                                    }))         
            .AddEndpointFilterFactory(ValidationHelper.ValidateIdFactory);

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
            internal static EndpointFilterDelegate ValidateIdFactory(EndpointFilterFactoryContext context, EndpointFilterDelegate next)
            {
            ParameterInfo[] parameters = context.MethodInfo.GetParameters(); 
            int? idPosition = null; 
            for (int i = 0; i < parameters.Length; i++) 
            { 
                        if (parameters[i].Name == "id" && parameters[i].ParameterType == typeof(string)) 
                        { 
                                    idPosition = i; 
                                    break; 
                        } 
            } 
            if (!idPosition.HasValue) 
            { 
                        return next; 
            } 
            return async (invocationContext) => 
            {
                        var id = invocationContext.GetArgument<string>(idPosition.Value); 
                        if (string.IsNullOrEmpty(id) || !id.StartsWith('f')) 
                        { 
                                    return Results.ValidationProblem( new Dictionary<string, string[]> 
                                    {{ "id", new[] { "Id must start with 'f'" }}}); 
                        } 
                        return await next(invocationContext); 
            };
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
