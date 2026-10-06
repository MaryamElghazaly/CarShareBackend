using Microsoft.EntityFrameworkCore;
using CarShare.DAL.Data;
using CarShare.DAL.Interfaces;
using CarShare.DAL.Repositories;
using CarShare.BLL.Mappings;
using CarShare.BLL.Interfaces;
using CarShare.BLL.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace CarShare
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register DbContext
            builder.Services.AddDbContext<CarShareDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register UnitOfWork (includes all repositories)
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Optionally register specialized repositories individually
            builder.Services.AddScoped<ICarRepository, CarRepository>();

            // Add AutoMapper with your profile
            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

            // Add services to DI container
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ICarService, CarService>();
            builder.Services.AddScoped<IRentalService, RentalService>();

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // To allow Swagger UI to test authenticated endpoints
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter JWT Bearer token like: Bearer {your token}",
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            // Add JWT authentication
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
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "DefaultSecretKeyForCarShareApp2025!")),
                        NameClaimType = JwtRegisteredClaimNames.Sub
                    };
                });

            // Add IConfiguration to DI
            builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

            // Add CORS Policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp", policy =>
                {
                    policy.WithOrigins("http://localhost:5173", "http://localhost:3000", "http://localhost:5174", "http://127.0.0.1:5173")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });


            var app = builder.Build();

            // Auto-migrate and seed initial demo accounts
            try
            {
                using var scope = app.Services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<CarShareDbContext>();
                await DatabaseSeeder.SeedAsync(dbContext);
            }
            catch (Exception ex)
            {
                var logger = app.Services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while migrating/seeding the database.");
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCors("AllowReactApp"); 

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            await app.RunAsync();
        }
    }
}
