var builder = WebApplication.CreateBuilder(args);

// 🔹 Ajouter les services ici avant de construire l'application
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

builder.Services.AddControllers();

// 🔹 Ajouter les services métier
builder.Services.AddScoped<IXmlFileService, XmlFileService>();

// 🔹 Construire l'application
var app = builder.Build();

// 🔹 Activer CORS
app.UseCors("AllowAll");

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// 🔹 Lancer l'application
app.Run();
