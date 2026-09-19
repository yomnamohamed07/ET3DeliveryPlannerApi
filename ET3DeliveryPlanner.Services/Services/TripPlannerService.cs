using ET3DeliveryPlanner.Data.MappingProfiles;
using ET3DeliveryPlanner.Data.Repositories;
using ET3DeliveryPlanner.Data.Services;

namespace ET3DeliveryPlanner.Services.Services
{
    public class TripPlannerService : ITripPlannerService
    {
        private const decimal MaxTripWeight = 10m;

        private readonly IDeliveryRepository _deliveryRepository;

        public TripPlannerService(
            IDeliveryRepository deliveryRepository)
        {
            _deliveryRepository = deliveryRepository;
        }

        public async Task<List<Trip>> CreateTripsAsync()
        {
            var deliveries = await _deliveryRepository.GetAllAsync();

            var validDeliveries = deliveries
                .Where(d =>
                    d.PackageWeight > 0 &&
                    d.PackageWeight <= MaxTripWeight)
                .OrderBy(d => d.Priority)
                .ThenBy(d => d.Area)
                .ThenBy(d => d.Id)
                .ToList();

            var trips = new List<Trip>();

            foreach (var delivery in validDeliveries)
            {
                // 1. Same priority + same area
                var selectedTrip = trips
                    .Where(t =>
                        t.TotalWeight + delivery.PackageWeight <= MaxTripWeight &&
                        t.Deliveries.Any(d =>
                            d.Priority == delivery.Priority &&
                            string.Equals(
                                d.Area,
                                delivery.Area,
                                StringComparison.OrdinalIgnoreCase)))
                    .OrderByDescending(t => t.TotalWeight)
                    .FirstOrDefault();

                // 2. Same priority
                if (selectedTrip == null)
                {
                    selectedTrip = trips
                        .Where(t =>
                            t.TotalWeight + delivery.PackageWeight <= MaxTripWeight &&
                            t.Deliveries.Any(d =>
                                d.Priority == delivery.Priority))
                        .OrderByDescending(t => t.TotalWeight)
                        .FirstOrDefault();
                }

                // 3. Same area
                if (selectedTrip == null)
                {
                    selectedTrip = trips
                        .Where(t =>
                            t.TotalWeight + delivery.PackageWeight <= MaxTripWeight &&
                            t.Deliveries.Any(d =>
                                string.Equals(
                                    d.Area,
                                    delivery.Area,
                                    StringComparison.OrdinalIgnoreCase)))
                        .OrderByDescending(t => t.TotalWeight)
                        .FirstOrDefault();
                }

                // 4. Any trip with enough capacity
                if (selectedTrip == null)
                {
                    selectedTrip = trips
                        .Where(t =>
                            t.TotalWeight + delivery.PackageWeight <= MaxTripWeight)
                        .OrderByDescending(t => t.TotalWeight)
                        .FirstOrDefault();
                }

                // 5. Create new trip
                if (selectedTrip == null)
                {
                    selectedTrip = new Trip
                    {
                        TripId = trips.Count + 1
                    };

                    trips.Add(selectedTrip);
                }

                selectedTrip.Deliveries.Add(delivery);

                selectedTrip.TotalWeight += delivery.PackageWeight;
            }

            return trips;
        }
        public async Task<TripSummary> GetSummaryAsync()
        {
            var deliveries =
                await _deliveryRepository.GetAllAsync();

            var validDeliveries = deliveries
                .Where(d =>
                    d.PackageWeight > 0 &&
                    d.PackageWeight <= MaxTripWeight)
                .ToList();

            var invalidDeliveries = deliveries
                .Where(d =>
                    d.PackageWeight <= 0 ||
                    d.PackageWeight > MaxTripWeight)
                .ToList();

            var trips = await CreateTripsAsync();

            var totalWeight =
                validDeliveries.Sum(d => d.PackageWeight);

            var averageDeliveriesPerTrip =
                trips.Count == 0
                    ? 0
                    : (decimal)validDeliveries.Count / trips.Count;

            return new TripSummary
            {
                TotalDeliveries = deliveries.Count,
                ValidDeliveries = validDeliveries.Count,
                InvalidDeliveries = invalidDeliveries.Count,
                TotalTrips = trips.Count,
                TotalWeight = totalWeight,
                AverageDeliveriesPerTrip =
                    Math.Round(
                        averageDeliveriesPerTrip,
                        2)
            };
        }
    }
}