using Microsoft.EntityFrameworkCore;
using szines.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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
        options.ListenAnyIP(int.Parse(builder.Configuration["settings:port"] ?? "6500"));
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
                .AllowCredentials()
                .AllowAnyMethod());

app.MapControllers();

app.Run();
