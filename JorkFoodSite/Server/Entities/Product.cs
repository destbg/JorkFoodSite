namespace JorkFoodSite.Server.Entities;

public class Product
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Ingredients { get; set; }
    public int? Grams { get; set; }
    public double Price { get; set; }
    public double? OldPromoPrice { get; set; }
    public double BoxPrice { get; set; }
    public int BoxCount { get; set; }
    public int Order { get; set; }
    public string ProductGroupId { get; set; }
    public virtual ProductGroup ProductGroup { get; set; }
    public ICollection<Order> Orders { get; set; }
}
