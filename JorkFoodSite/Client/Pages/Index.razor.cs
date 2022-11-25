using JorkFoodSite.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http.Json;

namespace JorkFoodSite.Client.Pages;

public partial class Index
{
    [Inject] private HttpClient Http { get; set; } = null!;

    public List<ProductGroupDTO>? ProductGroups { get; set; }

    protected override async Task OnInitializedAsync()
    {
        ProductGroups = await Http.GetFromJsonAsync<List<ProductGroupDTO>>("App/Menu");
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
}
