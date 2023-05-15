using JorkFoodSite.Shared;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace JorkFoodSite.Client.Pages;

public partial class Edit
{
    [Inject] private HttpClient Http { get; set; } = null!;

    public List<ProductGroupDTO>? ProductGroups { get; set; }
    public ProductDTO? EditProduct { get; set; }
    public bool EditIsPromo { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Constants.OrdersChanged += StateHasChanged;

        ProductGroups = await Http.GetFromJsonAsync<List<ProductGroupDTO>>("App/Menu");
    }

    public void EditProductClicked(ProductDTO product, bool isPromo)
    {
        EditIsPromo = isPromo;
        EditProduct = new ProductDTO
        {
            Id = product.Id,
            Ingredients = product.Ingredients,
            BoxCount = product.BoxCount,
            BoxPrice = product.BoxPrice,
            Grams = product.Grams,
            OldPromoPrice = product.OldPromoPrice,
            Price = product.Price,
            Title = product.Title,
            ProductGroupId = product.ProductGroupId,
        };
    }

    public void CancelEdit()
    {
        EditProduct = null;
    }

    public async Task OnConfirmEdit()
    {
        await Http.PostAsJsonAsync("App/ReplaceOrder", EditProduct);
        EditProduct = null;
        ProductGroups = await Http.GetFromJsonAsync<List<ProductGroupDTO>>("App/Menu");
    }
}
