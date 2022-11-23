using JorkFoodSite.Shared;
using Microsoft.AspNetCore.Components;

namespace JorkFoodSite.Client.Shared;

public partial class ProductCard
{
    [Parameter] public List<ProductDTO>? Products { get; set; }
    [Parameter] public List<PersonOrderDTO>? Orders { get; set; }
    [Parameter] public EventCallback<ProductDTO> OnProductClicked { get; set; }
}
