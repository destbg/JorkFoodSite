using JorkFoodSite.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http.Json;

namespace JorkFoodSite.Client.Pages;

public partial class Index
{
    [Inject] private HttpClient Http { get; set; } = null!;

    public List<ProductGroupDTO>? ProductGroups { get; set; }
    public List<PersonOrderDTO>? Orders { get; set; }
    public bool IsOrdersVisible { get; set; }

    protected override async Task OnInitializedAsync()
    {
        ProductGroups = await Http.GetFromJsonAsync<List<ProductGroupDTO>>("App/Menu");

        if (!string.IsNullOrEmpty(Constants.PersonName))
        {
            Orders = await Http.GetFromJsonAsync<List<PersonOrderDTO>>("App/PersonOrders/" + Constants.PersonName);
        }
    }

    private async Task OnClicked(ProductDTO product)
    {
        if (Orders == null || !Orders.Any(f => f.ProductId == product.Id))
        {
            await Constants.Connection.SendAsync("SubmitOrder", new SubmitOrderDTO
            {
                PersonName = Constants.PersonName,
                ProductId = product.Id
            });
            Orders = await Http.GetFromJsonAsync<List<PersonOrderDTO>>("App/PersonOrders/" + Constants.PersonName);
        }
    }

    private void OpenOrders()
    {
        IsOrdersVisible = true;
    }

    private void CloseOrders()
    {
        IsOrdersVisible = false;
    }

    private Task CancelOrder(PersonOrderDTO order)
    {
        if (order.Count > 1)
        {
            order.Count--;
        }
        else
        {
            Orders!.Remove(order);
        }

        return Constants.Connection.SendAsync("CancelOrder", new SubmitOrderDTO
        {
            PersonName = Constants.PersonName,
            ProductId = order.ProductId
        });
    }

    private async Task AddOrder(PersonOrderDTO order)
    {
        await Constants.Connection.SendAsync("SubmitOrder", new SubmitOrderDTO
        {
            PersonName = Constants.PersonName,
            ProductId = order.ProductId
        });
        order.Count++;
    }
}
