using Microsoft.AspNetCore.SignalR.Client;

namespace JorkFoodSite.Client;

public static class Constants
{
    public static string PersonName { get; set; }
    public static HubConnection Connection { get; set; }
}
