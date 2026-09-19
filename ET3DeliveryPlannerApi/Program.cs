using ET3DeliveryPlanner.Data.Repositories;
using ET3DeliveryPlanner.Data.Services;
using ET3DeliveryPlanner.Infrastructure.Data;
using ET3DeliveryPlanner.Infrastructure.Repositories;
using ET3DeliveryPlanner.Services.Services;
using ET3DeliveryPlannerApi.MiddleWares;
using Microsoft.EntityFrameworkCore;

namespace ET3DeliveryPlannerApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<DeliveryPlannerDbContext>(
                options =>
                {
                    options.UseSqlServer(
                        builder.Configuration
                            .GetConnectionString("DefaultConnection"));
                });

            builder.Services.AddScoped< IDeliveryRepository,DeliveryRepository>();

            builder.Services.AddScoped<ITripPlannerService,TripPlannerService>();

            var app = builder.Build();

            // Database Migration & Data Seeding
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();

                try
                {
                    var dbcontext =
                        services.GetRequiredService<DeliveryPlannerDbContext>();

                    await dbcontext.Database.MigrateAsync();

                    await DataSeeder.SeedAsync(dbcontext);
                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<Program>();

                    logger.LogError(
                        ex,
                        "Error occurred during startup seeding");
                }
            }

            app.UseMiddleware<ExceptionMiddleWare>();

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