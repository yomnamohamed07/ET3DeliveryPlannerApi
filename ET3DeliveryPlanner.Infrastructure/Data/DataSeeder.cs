

using ET3DeliveryPlanner.Data.Entities;
using System.Text.Json;

namespace ET3DeliveryPlanner.Infrastructure.Data
{

    public static class DataSeeder
    {
        public static async Task SeedAsync(DeliveryPlannerDbContext context)
        {
            if (!context.Deliveries.Any())
            {
                var path = "../ET3DeliveryPlanner.Infrastructure/Data/DataSeeding/Delivery.json";

                var DeliveryData = File.ReadAllText(path);

                var Deliveries = JsonSerializer.Deserialize<List<Delivery>>(DeliveryData);

                if (Deliveries?.Count > 0)
                {
                    foreach (var Delivery in Deliveries)
                    {
                        await context.Deliveries.AddAsync(Delivery);
                    }

                    await context.SaveChangesAsync();
                }
            }
        }
    }
}

