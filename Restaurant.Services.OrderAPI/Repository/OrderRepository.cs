using Microsoft.EntityFrameworkCore;
using Restaurant.Services.OrderAPI.DbContexts;
using Restaurant.Services.OrderAPI.Models;

namespace Restaurant.Services.OrderAPI.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContext;


        public OrderRepository(DbContextOptions<ApplicationDbContext> dbContext)
        {
            _dbContext = dbContext;

        }
        public async Task<bool> AddOrder(OrderHeader orderHeader)
        {
            await using var _db = new ApplicationDbContext(_dbContext);
            _db.OrderHeaders.Add(orderHeader);
            _db.SaveChangesAsync();
            return true;
        }

        public async Task UpdateOrderPaymentStatus(int orderHeaderId, bool paid)
        {
            await using var _db = new ApplicationDbContext(_dbContext);
            var orderHeaderFromDb = await _db.OrderHeaders.FirstOrDefaultAsync(u => u.OrderHeaderId == orderHeaderId);
            if (orderHeaderFromDb != null)
            {
                orderHeaderFromDb.PaymentStatus = paid;
                await _db.SaveChangesAsync();
            }

        }
    }
}
