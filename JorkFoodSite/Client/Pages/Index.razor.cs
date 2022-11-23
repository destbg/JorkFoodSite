using JorkFoodSite.Shared;
using Microsoft.AspNetCore.Components;
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
        await Http.PostAsJsonAsync("App/SubmitOrder", new SubmitOrderDTO
        {
            PersonName = Constants.PersonName,
            ProductId = product.Id
        });
        Orders = await Http.GetFromJsonAsync<List<PersonOrderDTO>>("App/PersonOrders/" + Constants.PersonName);
    }

    private Task CancelOrder(PersonOrderDTO order)
    {
        Orders!.Remove(order);

        return Http.PostAsJsonAsync("App/CancelOrder", new SubmitOrderDTO
        {
            PersonName = Constants.PersonName,
            ProductId = order.ProductId,
        });
    }
}
