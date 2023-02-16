using System.Text;
using JorkFoodSite.Shared;
using Microsoft.AspNetCore.SignalR.Client;

namespace JorkFoodSite.Client;

public static class Constants
{
    public static string PersonName { get; set; } = null!;
    public static HubConnection Connection { get; set; } = null!;
    public static List<PersonOrderDTO>? Orders { get; set; }

    public static readonly Dictionary<char, string> LatinToCyrillic = new()
    {
        { 'a', "а" },
        { 'b', "б" },
        { 'c', "ц" },
        { 'd', "д" },
        { 'e', "е" },
        { 'f', "ф" },
        { 'g', "г" },
        { 'h', "х" },
        { 'i', "и" },
        { 'j', "дж" },
        { 'k', "к" },
        { 'l', "л" },
        { 'm', "м" },
        { 'n', "н" },
        { 'o', "о" },
        { 'p', "п" },
        { 'q', "я" },
        { 'r', "р" },
        { 's', "с" },
        { 't', "т" },
        { 'u', "у" },
        { 'v', "ж" },
        { 'w', "в" },
        { 'x', "ь" },
        { 'y', "ъ" },
        { 'z', "з" },
    };

    public static event Action? OrdersChanged;

    public static void ChangeOrders()
    {
        OrdersChanged?.Invoke();
    }

    public static void TranslateName()
    {
        StringBuilder builder = new();

        for (int i = 0; i < PersonName.Length; i++)
        {
            char c = PersonName[i];
            char cLower = char.ToLower(c);

            if (i == 0)
            {
                c = char.ToUpper(c);
            }

            if (LatinToCyrillic.TryGetValue(cLower, out string? value))
            {
                builder.Append(c != cLower ? value.ToUpper() : value);
            }
            else
            {
                builder.Append(c);
            }
        }

        PersonName = builder.ToString();
    }
}
