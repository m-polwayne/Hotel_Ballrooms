using BallroomManager.Api.Data;
using BallroomManager.Api.Services; 
using BallroomManager.Api.Filters;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs; 
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Ballroom Manager API",
        Description = "An API to manage hotel ballrooms"
    });

    // Configure Swagger to handle file uploads
    options.OperationFilter<SwaggerFileOperationFilter>();
});

// Add DbContext
builder.Services.AddDbContext<BallroomDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Blob Storage
builder.Services.AddSingleton(x =>
    new BlobServiceClient(builder.Configuration.GetSection("AzureBlobStorage:ConnectionString").Value));

// Add Services
builder.Services.AddScoped<IBallroomService, BallroomService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        corsBuilder =>
        {
            corsBuilder.WithOrigins(
                    "https://ballroomsstore.z1.web.core.windows.net",
                    "http://localhost:5244",
                    "http://localhost:8000",
                    "http://localhost:5080",
                    "https://ballrooms-web.azurewebsites.net",
                    "https://ballrooms-booking.azurewebsites.net"
                )
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options => 
    {
        options.SerializeAsV2 = false;
    });
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ballroom Manager API V1");
    });
}
else 
{
    // Enable HSTS in production
    app.UseHsts();
}

// Ensure database is created and migrations are applied
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BallroomDbContext>();
    dbContext.Database.Migrate();
}

// Use CORS before other middleware
app.UseCors("AllowSpecificOrigin");

// Always use HTTPS in production
app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();