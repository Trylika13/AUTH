using System.Text;
using AUTH.Core.Interfaces.Repositories;
using AUTH.Core.Interfaces.Services;
using AUTH.Core.Services;
using AUTH.Infrastructure.Data;
using AUTH.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// --- 1. SERVICES DE BASE ---
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// --- 2. BASE DE DONNÉES (EF CORE) ---
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- 3. INJECTION DE DÉPENDANCES (CLEAN ARCHI) ---
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

// --- 4. SÉCURITÉ & AUTHENTIFICATION (JWT) ---
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// --- 5. CONFIGURATION DU CORS (POUR ANGULAR) ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy => 
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// --- 6. PIPELINE HTTP (MIDDLEWARES) ---

// L'ordre ici est CRITIQUE :
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection(); 

app.UseCors("AllowAngular"); 

app.UseAuthentication(); // Qui est l'utilisateur ?
app.UseAuthorization();  // A-t-il le droit d'accéder à la ressource ?

app.MapControllers();

app.Run();