

using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using ET3DeliveryPlanner.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ET3DeliveryPlanner.Infrastructure.Data
{
    public class DeliveryPlannerDbContext : DbContext
    {
        public DeliveryPlannerDbContext(
            DbContextOptions<DeliveryPlannerDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                Assembly.GetExecutingAssembly());
        }

        public DbSet<Delivery> Deliveries { get; set; }
    }
}
