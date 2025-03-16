using dotenv.net;
using Ecommerce.Services;
using Ecommerce.Services.DAO.Connexion;
using Ecommerce.Services.DAO.Implementations;
using Ecommerce.Services.DAO.Interfaces.IDAO;
using Ecommerce.Services.DAO.Interfaces.IRepository;
using Ecommerce.Services.DAO.Interfaces.IServices;
using Ecommerce.Services.DAO.Models;
using Ecommerce.Services.DAO.Repositories;
using Ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoFramework;
using Nest;
using Stripe;
using System.Text;
using IMongoDbConnection = Ecommerce.Services.DAO.Interfaces.IRepository.IMongoDbConnection;

DotEnv.Load();

// Chargement des variables d'environnement
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
var stripeSecretKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");
var stripePiblishableKey = Environment.GetEnvironmentVariable("STRIPE_PUBLISHABLE_KEY");
var stripeWebHook = Environment.GetEnvironmentVariable("STRIPE_WEBHOOK");
var smptpserver = Environment.GetEnvironmentVariable("EMAIL_SETTINGS_SMTP_SERVER");
var stmpPort = Environment.GetEnvironmentVariable("EMAIL_SETTINGS_PORT");
var smtpUsername = Environment.GetEnvironmentVariable("EMAIL_SETTINGS_USERNAME");
var smtpPassword = Environment.GetEnvironmentVariable("EMAIL_SETTINGS_PASSWORD");
var smtpFromEmail = Environment.GetEnvironmentVariable("EMAIL_SETTINGS_FROM_EMAIL");
var redisConnection = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING");

var builder = WebApplication.CreateBuilder(args);

// Configuration de Stripe
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

// Configuration MongoDB
var mongoConnectionString = builder.Configuration.GetSection("MongoConnection")["DefaultConnection"];
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoConnection"));

builder.Services.AddSingleton<IMongoDbConnection>(provider =>
{
    var mongoDbSettings = provider.GetRequiredService<IOptions<MongoDbSettings>>();
    return MongoConnection.Instance(mongoDbSettings);
});

// Configuration du service Jwt
builder.Services.AddScoped<IJwtService>(provider =>
{
    if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
    {
        throw new InvalidOperationException("Les variables d'environnement JWT_KEY, JWT_ISSUER ou JWT_AUDIENCE ne sont pas définies.");
    }

    return new JwtService(jwtKey, jwtIssuer, jwtAudience);
});

// CORS - Politique pour accepter toutes les origines
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
});

// Authentification JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// Ajout des services
builder.Services.AddControllers();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "SampleApp_";
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ecommerce API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Veuillez entrer 'Bearer' suivi de votre token JWT.",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// Enregistrement des autres services
builder.Services.AddScoped<IUserDAO, UserDAO>();
builder.Services.AddScoped<IProductDAO, ProductDAO>();
builder.Services.AddScoped<IBookDAO, BookDAO>();
builder.Services.AddScoped<IOrderDAO, OrderDAO>();
builder.Services.AddScoped<IAppointmentDAO, AppointmentDAO>();
builder.Services.AddScoped<IArticleDAO, ArticleDAO>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBasketService, BasketService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionServices>();
builder.Services.AddScoped<IInvoiceService, InvoiceServices>();
builder.Services.AddScoped<INewsletterService, NewsletterService>();

var app = builder.Build();


// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ecommerce API v1"));
}

app.UseCors("AllowAllOrigins");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
