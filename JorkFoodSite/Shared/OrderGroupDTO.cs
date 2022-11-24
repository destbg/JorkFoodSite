namespace JorkFoodSite.Shared;

public class OrderGroupDTO
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<OrderDTO> Orders { get; set; }
}
