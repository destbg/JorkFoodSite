using JorkFoodSite.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http.Json;

namespace JorkFoodSite.Client.Pages;

public partial class Index
{
    [Inject] private HttpClient Http { get; set; } = null!;

    public List<ProductGroupDTO>? ProductGroups { get; set; }
    public bool IsOrdersVisible { get; set; }

    protected override async Task OnInitializedAsync()
    {
        ProductGroups = await Http.GetFromJsonAsync<List<ProductGroupDTO>>("App/Menu");

        if (!string.IsNullOrEmpty(Constants.PersonName))
        {
            Constants.Orders = await Http.GetFromJsonAsync<List<PersonOrderDTO>>("App/PersonOrders/" + Constants.PersonName);
        }
    }

    private static async Task OnClicked(ProductDTO product)
    {
        if (Constants.Orders == null || !Constants.Orders.Any(f => f.ProductId == product.Id))
        {
            await Constants.Connection.SendAsync("SubmitOrder", new SubmitOrderDTO
            {
                PersonName = Constants.PersonName,
                ProductId = product.Id
            });

            Constants.Orders ??= new List<PersonOrderDTO>();

            Constants.Orders.Add(new PersonOrderDTO
            {
                ProductId = product.Id,
                Count = 1,
                Price = product.Price,
                Name = product.Name,
            });
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

    private static Task CancelOrder(PersonOrderDTO order)
    {
        if (order.Count > 1)
        {
            order.Count--;
        }
        else
        {
            Constants.Orders!.Remove(order);
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
