

using ET3DeliveryPlanner.Data.MappingProfiles;

namespace ET3DeliveryPlanner.Data.Services
{
    public interface ITripPlannerService
    {
        Task<List<Trip>> CreateTripsAsync();

        Task<TripSummary> GetSummaryAsync();
    }
}
