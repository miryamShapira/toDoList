using Microsoft.AspNetCore.Mvc;
using TodoAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
//  using Microsoft.AspNetCore.OpenApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ToDoDbContext>(ctx => ctx.UseMySql("name=ToDoDB", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.32-mysql")));

const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                        policy.WithOrigins("http://localhost:3000","http://localhost:3001")
                          .AllowCredentials()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
                          //   policy.WithOrigins("*");

                      });
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    options => options.SwaggerDoc("v1", new OpenApiInfo
{
    Version = "v1",
    Title = "ToDo API",
    Description = "An ASP.NET Core Web API for managing ToDo items",
    TermsOfService = new Uri("https://example.com/terms"),
    Contact = new OpenApiContact
    {
        Name = "Example Contact",
        Url = new Uri("https://example.com/contact")
    },
    License = new OpenApiLicense
    {
        Name = "Example License",
        Url = new Uri("https://example.com/license")
    }
}));
var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/items", (ToDoDbContext db) =>
{
    var list = db.Items?.ToList();

    return Results.Ok(list);
});



app.MapPost("/addTask", async ([FromBody] Item item, ToDoDbContext db) =>
{
    await db.Items.AddRangeAsync(item);
    await db.SaveChangesAsync();

    return Results.Ok(item);
});
app.MapPut("/completeTask/{taskId}", async ([FromRoute] int taskId, ToDoDbContext db) =>
{
if (await db.Items.FindAsync(taskId) is Item item)
    {   
    item.IsComplete=true;
    db.Items.Update(item);
    await db.SaveChangesAsync();
    return Results.Ok(item);
    }
 return Results.NotFound();
});

app.MapDelete("/{taskId}", async ([FromRoute] int taskId, ToDoDbContext db) =>
{
    if (await db.Items.FindAsync(taskId) is Item item)
    {
        db.Items.Remove(item);
        await db.SaveChangesAsync();
        return Results.Ok(item);
    }

    return Results.NotFound();
});
app.UseStaticFiles();
app.UseCors(MyAllowSpecificOrigins);
app.Run();
