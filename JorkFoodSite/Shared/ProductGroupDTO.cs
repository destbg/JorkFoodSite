namespace JorkFoodSite.Shared;

public class ProductGroupDTO
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<ProductDTO> Products { get; set; }
}
