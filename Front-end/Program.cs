var builder = WebApplication.CreateBuilder(args);

// Configuration CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:5001")  // L'URL de votre front-end Razor
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


var app = builder.Build();

app.UseCors("AllowSpecificOrigin"); // Appliquer la politique CORS

app.UseRouting();
app.MapControllers(); // API controllers
app.Run();
