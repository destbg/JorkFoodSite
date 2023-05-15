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

    public async Task SubmitOrder(SubmitOrderDTO order)
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
                Count = 1,
            });
        }

        context.SaveChanges();

        await Clients.All.SendAsync("OrdersChanged");
    }

    public async Task CancelOrder(SubmitOrderDTO order)
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

        await Clients.All.SendAsync("OrdersChanged");
    }

    public async Task CancelFullOrder(SubmitOrderDTO order)
    {
        Order orderEntity = context.Orders.FirstOrDefault(f => f.PersonName == order.PersonName && f.ProductId == order.ProductId);

        context.Orders.Remove(orderEntity);

        context.SaveChanges();

        await Clients.All.SendAsync("OrdersChanged");
    }
}
