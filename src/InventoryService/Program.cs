using Microsoft.EntityFrameworkCore;
using InventoryService.Data;

var builder = WebApplication.CreateBuilder(args);

var secretPath = "/mnt/secrets-store/sql-connection-string";
var connectionString = builder.Environment.IsProduction() && File.Exists(secretPath)
    ? File.ReadAllText(secretPath)
    : builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<InventoryDbContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
        sqlOptions.CommandTimeout(120);
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

if (!string.IsNullOrEmpty(connectionString))
{
    try
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
        await Task.Delay(5000);
        await dbContext.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"--> Greška prilikom primjene migracija za InventoryService: {ex.Message}");
    }
}

app.Run();

public partial class Program { }