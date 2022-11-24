namespace JorkFoodSite.Server.Entities;

public class ProductGroup
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Order { get; set; }
    public ICollection<Product> Products { get; set; }
}
