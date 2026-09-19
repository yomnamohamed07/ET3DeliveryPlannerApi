

namespace ET3DeliveryPlanner.Data.MappingProfiles
{
    public class TripSummary
    {
        public int TotalDeliveries { get; set; }

        public int ValidDeliveries { get; set; }

        public int InvalidDeliveries { get; set; }

        public int TotalTrips { get; set; }

        public decimal TotalWeight { get; set; }

        public decimal AverageDeliveriesPerTrip { get; set; }
    }
}
