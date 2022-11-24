using JorkFoodSite.Server.Entities;
using JorkFoodSite.Shared;
using Microsoft.AspNetCore.SignalR;

namespace JorkFoodSite.Server.Hubs;

public class AppHub : Hub
{
    private readonly AppDbContext context;

    public AppHub(AppDbContext context)
    {
        this.context = context;
    }

    public void SubmitOrder(SubmitOrderDTO order)
    {
        Order prevOrder = context.Orders.FirstOrDefault(f => f.PersonName == order.PersonName && f.ProductId == order.ProductId);

        if (prevOrder != null)
        {
            prevOrder.Count++;
        }
        else
        {
            context.Orders.Add(new Order
            {
                Id = Guid.NewGuid().ToString("N"),
                PersonName = order.PersonName,
                ProductId = order.ProductId,
                Count = 1
            });
        }

        context.SaveChanges();
    }

    public void CancelOrder(SubmitOrderDTO order)
    {
        Order orderEntity = context.Orders.FirstOrDefault(f => f.PersonName == order.PersonName && f.ProductId == order.ProductId);

        if (orderEntity.Count > 1)
        {
            orderEntity.Count--;
        }
        else
        {
            context.Orders.Remove(orderEntity);
        }

        context.SaveChanges();
    }
}
