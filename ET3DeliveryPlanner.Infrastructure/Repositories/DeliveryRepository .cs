

using ET3DeliveryPlanner.Data.Entities;
using ET3DeliveryPlanner.Data.Repositories;
using ET3DeliveryPlanner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace ET3DeliveryPlanner.Infrastructure.Repositories
{
    public class DeliveryRepository : IDeliveryRepository
    {
        private readonly DeliveryPlannerDbContext _context;

        public DeliveryRepository(DeliveryPlannerDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Delivery delivery)
        {
            await _context.Deliveries.AddAsync(delivery);

            await _context.SaveChangesAsync();
        }

        public async Task<List<Delivery>> GetAllAsync()
        {
            return await _context.Deliveries
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Delivery?> GetByIdAsync(int id)
        {
            return await _context.Deliveries
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);
        }
    }
}
