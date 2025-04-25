using FileSharing.WebApi.Extensions;
using FileSharing.WebApi.Infrastructure.Storage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerWithAuth();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAppServices(builder.Configuration);

var app = builder.Build();

var storageInitializer = app.Services.GetRequiredService<StorageInitializer>();
storageInitializer.EnsureUploadDirectoryExists();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
