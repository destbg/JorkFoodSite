namespace JorkFoodSite.Server.Entities;

public class Product
{
    public string Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public int Order { get; set; }
    public string ProductGroupId { get; set; }
    public virtual ProductGroup ProductGroup { get; set; }
    public ICollection<Order> Orders { get; set; }
}
