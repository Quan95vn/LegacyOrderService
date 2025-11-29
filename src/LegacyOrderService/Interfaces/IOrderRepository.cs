using LegacyOrderService.Models;

namespace LegacyOrderService.Interfaces;

public interface IOrderRepository
{
    void Save(Order order);
}
