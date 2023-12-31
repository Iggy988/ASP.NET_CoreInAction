using System.Collections.Concurrent;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<RouteOptions>(o =>
{
    o.LowercaseUrls = true;
    o.AppendTrailingSlash = true;
    o.LowercaseQueryStrings = false;
});


var app = builder.Build();

app.MapGet("/HealthCheck", () => Results.Ok()).WithName("healthcheck");
app.MapGet("/{name}", (string name) => name).WithName("product");
app.MapGet("/", (LinkGenerator links) => new[]
{
    links.GetPathByName("healthcheck"),
    links.GetPathByName("product", new { Name = "Big-Widget", Q = "Test"})
});

var _fruit = new ConcurrentDictionary<string, Fruit>();

app.MapGet("/product/{name}", (string name) => $"The product is {name}")    
    .WithName("product"); 
app.MapGet("/links", (LinkGenerator links) => 
{
    string link = links.GetPathByName("product", new { name = "big-widget" }); 
    return $"View the product at {link}"; 
});

RouteGroupBuilder fruitApi = app.MapGroup("/fruit");

fruitApi.MapGet("/", () =>  _fruit);

RouteGroupBuilder fruitApiWithValidation = fruitApi.MapGroup("/").AddEndpointFilterFactory(ValidationHelper.ValidateIdFactory);

fruitApiWithValidation.MapGet("/{id}",  (string id) =>
    _fruit.TryGetValue(id, out var fruit) ? TypedResults.Ok(fruit) : Results.Problem(statusCode: 404))
            .AddEndpointFilter<IdValidationFilter>();



fruitApiWithValidation.MapPost("/{id}", /*Handlers.AddFruit*/ (string id, Fruit fruit) =>
    _fruit.TryAdd(id, fruit)
                ? TypedResults.Created($"/fruit/{id}", fruit)
                : Results.ValidationProblem(new Dictionary<string, string[]>
                            {
                                        {"id", new[] {"A fruit with this id already exists"}}
                            }));         

//Handlers handlers = new Handlers();
fruitApiWithValidation.MapPut("/{id}", (string id, Fruit fruit) =>
{
    _fruit[id] = fruit;
    return Results.NoContent();
});

fruitApiWithValidation.MapDelete("/fruit/{id}", /*handlers.DeleteFruit*/ (string id) => 
{
    _fruit.TryRemove(id, out _);
    return Results.NoContent();
});

app.MapGet("/test", () => "Hello world!").WithName("hello"); 
app.MapGet("/redirect-me",() => Results.RedirectToRoute("hello"));

app.Run();

class IdValidationFilter : IEndpointFilter 
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next) 
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
                return Results.ValidationProblem( new Dictionary<string, string[]> {{ "id", new[] { "Id must start with 'f'" }}}); 
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
