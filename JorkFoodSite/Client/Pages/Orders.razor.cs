using System.Net.Http.Json;
using JorkFoodSite.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace JorkFoodSite.Client.Pages;

public partial class Orders : IDisposable
{
    [Inject] private HttpClient Http { get; set; } = null!;

    public List<OrderGroupDTO>? AllOrders { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Constants.Connection.On("OrdersChanged", async () =>
        {
            AllOrders = await Http.GetFromJsonAsync<List<OrderGroupDTO>>("App/Orders");
            await InvokeAsync(StateHasChanged);
        });

        AllOrders = await Http.GetFromJsonAsync<List<OrderGroupDTO>>("App/Orders");
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        Constants.Connection.Remove("OrdersChanged");
    }
}
