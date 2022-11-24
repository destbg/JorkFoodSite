using JorkFoodSite.Shared;
using Microsoft.AspNetCore.SignalR.Client;

namespace JorkFoodSite.Client;

public static class Constants
{
    public static string PersonName { get; set; } = null!;
    public static HubConnection Connection { get; set; } = null!;
    public static List<PersonOrderDTO> Orders { get; set; } = null!;
}
