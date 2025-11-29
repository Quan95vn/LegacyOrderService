using LegacyOrderService.Models;

namespace LegacyOrderService.Interfaces;

public interface IOrderRepository
{
    Task SaveAsync(Order order, CancellationToken cancellationToken = default);
}
