
using Microsoft.OpenApi.Models;
using Notes.Repository;
using Notes.Services;
using Notes.Settings;
using TafeWeatherStudiesAPI.Repository;

namespace TafeWeatherStudiesAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {

                var filepath = Path.Combine(AppContext.BaseDirectory, "TafeWeatherStudiesAPI.xml");

                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "My Weather API",
                    Version = "v1"
                });

                options.IncludeXmlComments(filepath);

                options.AddSecurityDefinition("apiKey", new OpenApiSecurityScheme
                {
                    Description = "Enter API Key",
                    Name = "apiKey",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "apiKey"
                            },
                            Name = "apiKey",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Google", p =>
                {
                    p.WithOrigins("https://www.google.com", "https://www.google.com.au");
                    p.AllowAnyHeader();
                    p.WithMethods("GET", "POST", "PUT", "DELETE", "PATCH");
                });
            });


            builder.Services.Configure<MongoConnectionSettings>(builder.Configuration.GetSection("ConnString"));
            //add any required class to the dependancy injection system that need to be shared by the system
            builder.Services.AddScoped<MongoConnectionBuilder>();
            builder.Services.AddScoped<ISensorDataRepository, SensorDataRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            
            
            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseCors("Google");

            app.MapControllers();

            app.Run();
        }
    }
}
