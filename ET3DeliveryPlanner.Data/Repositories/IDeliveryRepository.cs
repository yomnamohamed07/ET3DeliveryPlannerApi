

using ET3DeliveryPlanner.Data.Entities;

namespace ET3DeliveryPlanner.Data.Repositories
{
    public interface IDeliveryRepository
    {
        Task<List<Delivery>> GetAllAsync();

        Task<Delivery?> GetByIdAsync(int id);

        Task AddAsync(Delivery delivery);
    }
}
