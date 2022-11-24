using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

namespace JorkFoodSite.Client.Shared;

public partial class MainLayout
{
    [Inject] private IJSRuntime JS { get; set; } = null!;
    [Inject] private NavigationManager URIHelper { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public bool PickedName { get; set; }
    public bool HasInitialized { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Name = await JS.InvokeAsync<string>("window.localStorage.getItem", "Name");
        Constants.PersonName = Name;

        if (!string.IsNullOrEmpty(Name))
        {
            PickedName = true;
        }

        Constants.Connection = new HubConnectionBuilder()
            .WithUrl(URIHelper.ToAbsoluteUri("/hub"))
            .Build();

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
}
