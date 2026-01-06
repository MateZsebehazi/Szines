using Microsoft.EntityFrameworkCore;
using szines.API.Data;
using szines.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<ColorEventService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration["db:conn"]));

builder.Services.AddCors();

if (builder.Environment.IsProduction())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(int.Parse(builder.Configuration["settings:port"] ?? "5000"));
    });
}

if (builder.Environment.IsStaging())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(int.Parse(builder.Configuration["settings:test_port"] ?? "5001"));
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(t => t
                .WithOrigins(builder.Configuration["settings:frontend"] ?? "http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod());

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();

        context.Database.Migrate();

        Console.WriteLine("Adatb�zis sikeresen friss�tve!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Hiba a migr�ci� sor�n: {ex.Message}");
    }
}

app.Run();

app.Run();
