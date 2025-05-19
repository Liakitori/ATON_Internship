
using ATON_Internship.Models;

namespace ATON_Internship
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Читаем настройки JWT
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            // Add services to the container.


            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
