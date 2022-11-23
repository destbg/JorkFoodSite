namespace JorkFoodSite.Server.Entities;

public class Order
{
    public string Id { get; set; }
    public string PersonName { get; set; }
    public string ProductId { get; set; }
    public int Count { get; set; }
    public virtual Product Product { get; set; }
}
