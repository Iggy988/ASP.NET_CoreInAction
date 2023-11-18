using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using EntityFrameworkExample;
using EntityFrameworkExample.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x => x.SwaggerDoc("v1", new OpenApiInfo { Title = "Recipe App", Version = "v1" }));

var connString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connString!));
builder.Services.AddScoped<RecipeService>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI();

var routes = app.MapGroup("")
    .WithParameterValidation()
    .WithOpenApi()
    .WithTags("Recipes");

routes.MapGet("/", async (RecipeService service) =>
{
    return await service.GetRecipes();
})
    .WithSummary("List recipes");

routes.MapPost("/", async (CreateRecipeCommand input, RecipeService service) =>
{
    var id = await service.CreateRecipe(input);
    return Results.CreatedAtRoute("view-recipe", new { id });
})
    .WithSummary("Create recipe")
    .Produces(StatusCodes.Status201Created);

routes.MapGet("/{id}", async (int id, RecipeService service) =>
{
    var recipe = await service.GetRecipeDetail(id);
    return recipe is null
        ? Results.Problem(statusCode: 404)
        : Results.Ok(recipe);
})
    .WithName("view-recipe")
    .WithSummary("Get recipe")
    .ProducesProblem(404)
    .Produces<RecipeDetailViewModel>();

routes.MapDelete("/{id}", async (int id, RecipeService service) =>
{
    await service.DeleteRecipe(id);
    return Results.NoContent();
})
    .WithSummary("Delete recipe")
    .Produces(201);

routes.MapPut("/{id}", async (int id, UpdateRecipeCommand input, RecipeService service) =>
{
    if (await service.IsAvailableForUpdate(id))
    {
        await service.UpdateRecipe(input);
        return Results.NoContent();
    }
    return Results.Problem(statusCode: 404);
})
    .WithSummary("Update recipe")
    .ProducesProblem(404)
    .Produces(204);

app.Run();
