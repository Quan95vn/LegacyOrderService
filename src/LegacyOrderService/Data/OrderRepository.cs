using LegacyOrderService.Models;
using LegacyOrderService.Interfaces;

namespace LegacyOrderService.Data;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _context;

    public OrderRepository(OrderDbContext context)
    {
        _context = context;
    }

    public void Save(Order order)
    {
        _context.Orders.Add(order);
        _context.SaveChanges();
    }
}
