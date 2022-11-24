namespace JorkFoodSite.Shared;

public class OrderDTO
{
    public string ProductId { get; set; }
    public string Name { get; set; }
    public int Count { get; set; }
    public double Price { get; set; }
    public List<OrderPersonDTO> People { get; set; }
}
