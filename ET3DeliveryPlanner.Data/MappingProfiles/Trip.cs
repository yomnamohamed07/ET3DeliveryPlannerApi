

using ET3DeliveryPlanner.Data.Entities;

namespace ET3DeliveryPlanner.Data.MappingProfiles
{
    public class Trip
    {
        public int TripId { get; set; }

        public List<Delivery> Deliveries { get; set; } = new();

        public decimal TotalWeight { get; set; }
    }
}
