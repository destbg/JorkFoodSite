namespace JorkFoodSite.Shared;

public class OrderDTO
{
    public string Name { get; set; }
    public int Count { get; set; }
    public double Price { get; set; }
    public List<OrderPersonDTO> People { get; set; }
}
