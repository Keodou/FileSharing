var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
if (!Directory.Exists(uploadsFolder))
{
    Directory.CreateDirectory(uploadsFolder);
}

app.MapPost("/upload", async (HttpContext context) =>
{
    var file = context.Request.Form.Files["file"];
    if (file == null || file.Length == 0)
    {
        return Results.BadRequest("Файл не найден");
    }

    var filePath = Path.Combine(uploadsFolder, file.FileName);
    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }

    return Results.Ok(new { file.FileName, Size = file.Length });
});

app.MapGet("/download/{fileName}", (string fileName) =>
{
    var filePath = Path.Combine(uploadsFolder, fileName);
    if (!File.Exists(filePath))
    {
        return Results.NotFound("Файл не найден.");
    }

    return Results.File(filePath, "application/octet-stream", fileName);
});

app.MapGet("files/", () =>
{
    var files = Directory.GetFiles(uploadsFolder);
    if (files == null)
    {
        return Results.NotFound("Нет загруженных файлов");
    }

    return Results.Ok(files
        .Select(Path.GetFileName)
        .ToList());
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
