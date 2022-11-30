using JorkFoodSite.Shared;
using Microsoft.AspNetCore.SignalR.Client;

namespace JorkFoodSite.Client.Shared;

public partial class PersonOrders
{
    public bool IsOrdersVisible { get; set; }
    public bool IsOrdersClosing { get; set; }

    protected override void OnInitialized()
    {
        Constants.OrdersChanged += StateHasChanged;
    }

    private void OpenOrders()
    {
        IsOrdersVisible = true;
    }

    private async Task CloseOrders()
    {
        IsOrdersClosing = true;
        StateHasChanged();
        await Task.Delay(100);
        IsOrdersVisible = false;
        IsOrdersClosing = false;
        StateHasChanged();
    }

    private static Task CancelOrder(PersonOrderDTO order)
    {
        if (order.Count > 1)
        {
            order.Count--;
        }
        else
        {
            Constants.Orders!.Remove(order);
            Constants.ChangeOrders();
        }

        return Constants.Connection.SendAsync("CancelOrder", new SubmitOrderDTO
        {
            PersonName = Constants.PersonName,
            ProductId = order.ProductId
        });
    }

    private static async Task AddOrder(PersonOrderDTO order)
    {
        await Constants.Connection.SendAsync("SubmitOrder", new SubmitOrderDTO
        {
            PersonName = Constants.PersonName,
            ProductId = order.ProductId
        });
        order.Count++;
    }
}
