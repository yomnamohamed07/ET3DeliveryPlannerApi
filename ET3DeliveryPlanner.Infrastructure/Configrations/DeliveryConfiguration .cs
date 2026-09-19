

using ET3DeliveryPlanner.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ET3DeliveryPlanner.Infrastructure.Configrations
{
    public class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
    {
        public void Configure(
            EntityTypeBuilder<Delivery> builder)
        {
            builder.ToTable("Deliveries");

            builder.HasKey(d => d.Id);

          

            builder.Property(d => d.Area)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.Priority)
                .IsRequired();

            builder.Property(d => d.PackageWeight)
                .IsRequired()
                .HasPrecision(10, 2);

            builder.HasCheckConstraint(
                "CK_Deliveries_PackageWeight",
                "[PackageWeight] > 0 AND [PackageWeight] <= 10");
        }
    }
}
