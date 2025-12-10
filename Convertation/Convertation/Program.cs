using Convertation.Data;
using Convertation.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "http://localhost:5173",
                "https://localhost:3000",
                "https://localhost:5173"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddSingleton<FileDbContext>();
builder.Services.AddScoped<IConversionService, ConversionService>();

builder.Services.AddHttpClient("ExchangeApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7093/");
    client.Timeout = TimeSpan.FromSeconds(15);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowReactApp");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var env = app.Services.GetRequiredService<IWebHostEnvironment>();
var dataDir = Path.Combine(env.ContentRootPath, "wwwroot", "data");
Directory.CreateDirectory(dataDir);

app.Run();
