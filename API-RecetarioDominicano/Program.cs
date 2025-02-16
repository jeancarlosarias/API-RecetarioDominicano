using API_RecetarioDominicano.Models.Context;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace API_RecetarioDominicano
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
            builder.Services.AddSwaggerGen();

            //Configuracion del DbContext
            builder.Services.AddDbContext<DbConnectionContext>(options =>
                    options.UseMySql(builder.Configuration.GetConnectionString("DB1_ConnectionString"), new MySqlServerVersion(new Version(8, 0, 22))));


            //Agregar loger
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(
                "Logs\\Log-.json",
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 14,
                shared: true
                ).CreateLogger();

            builder.Host.UseSerilog();

            builder.Services.AddHttpClient();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
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
