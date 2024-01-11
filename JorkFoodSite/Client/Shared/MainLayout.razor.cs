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

    public string Name { get; set; } = null!;
    public bool PickedName { get; set; } = true;
    public bool HasInitialized { get; set; }

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
            Constants.TranslateName();
            Name = Constants.PersonName;

            Constants.Orders = await Http.GetFromJsonAsync<List<PersonOrderDTO>>("App/PersonOrders/" + Constants.PersonName);
            Constants.ChangeOrders();
        }

        await Constants.Connection.StartAsync();

        HasInitialized = true;
    }

    private async Task OnPickName()
    {
        if (!string.IsNullOrEmpty(Name))
        {
            Constants.PersonName = Name;
            Constants.TranslateName();
            Name = Constants.PersonName;
            await JS.InvokeVoidAsync("window.localStorage.setItem", "Name", Name);
            URIHelper.NavigateTo(URIHelper.Uri, forceLoad: true);
        }
    }
}
