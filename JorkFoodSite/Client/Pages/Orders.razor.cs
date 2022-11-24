using System.Net.Http.Json;
using JorkFoodSite.Shared;
using Microsoft.AspNetCore.Components;

namespace JorkFoodSite.Client.Pages;

public partial class Orders
{
    [Inject] private HttpClient Http { get; set; } = null!;

    public List<OrderGroupDTO>? PersonOrders { get; set; }

    protected override async Task OnInitializedAsync()
    {
        PersonOrders = await Http.GetFromJsonAsync<List<OrderGroupDTO>>("App/Orders");
    }
}
