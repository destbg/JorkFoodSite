using System.Net.Http.Json;
using JorkFoodSite.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

namespace JorkFoodSite.Client.Shared;

public partial class MainLayout
{
    [Inject] private IJSRuntime JS { get; set; } = null!;
    [Inject] private HttpClient Http { get; set; } = null!;
    [Inject] private NavigationManager URIHelper { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public bool PickedName { get; set; } = true;
    public bool HasInitialized { get; set; }
    public bool HasUnavailableOrder { get; set; }
    public bool IsOrdersVisible { get; set; }
    public bool IsOrdersClosing { get; set; }
    public List<string> OrderNames { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        Constants.Connection = new HubConnectionBuilder()
            .WithUrl(URIHelper.ToAbsoluteUri("/hub"))
            .Build();

        Name = await JS.InvokeAsync<string>("window.localStorage.getItem", "Name");
        Constants.PersonName = Name;

        if (string.IsNullOrEmpty(Name))
        {
            PickedName = false;
            return;
        }
        else
        {
            Constants.Orders = await Http.GetFromJsonAsync<List<PersonOrderDTO>>("App/PersonOrders/" + Constants.PersonName);
        }

        Constants.Connection.On("OrderUnavailable", async (string id, string name) =>
        {
            if (Constants.PersonName != "admin" && Constants.Orders!.Any(f => f.ProductId == id))
            {
                if (!HasUnavailableOrder)
                {
                    OrderNames = new();
                }

                OrderNames.Add(name);
                HasUnavailableOrder = true;
                Constants.Orders = await Http.GetFromJsonAsync<List<PersonOrderDTO>>("App/PersonOrders/" + Constants.PersonName);
                await InvokeAsync(StateHasChanged);
            }
        });

        await Constants.Connection.StartAsync();

        HasInitialized = true;
    }

    private async Task OnPickName()
    {
        if (!string.IsNullOrEmpty(Name))
        {
            Constants.PersonName = Name;
            await JS.InvokeVoidAsync("window.localStorage.setItem", "Name", Name);
            URIHelper.NavigateTo(URIHelper.Uri, forceLoad: true);
        }
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
