using System.Globalization;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using JorkFoodSite.Shared;
using Microsoft.AspNetCore.Components;

namespace JorkFoodSite.Client.Pages;

public partial class Add
{
    [Inject] private HttpClient Http { get; set; } = null!;
    [Inject] private NavigationManager URIHelper { get; set; } = null!;

    public string Menu { get; set; } = string.Empty;

    private async Task Submit()
    {
        if (string.IsNullOrEmpty(Menu))
        {
            return;
        }

        string[] split = Menu
            .Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(f => f.Trim().Replace("*", string.Empty).Replace("\"", string.Empty))
            .ToArray();

        List<ProductGroupDTO> list = new();
        ProductGroupDTO? currentGroup = null;
        Regex regex = MoneyRegex();
        int startIndex = -1;

        for (int i = 0; i < split.Length; i++)
        {
            string text = split[i].Trim();

            if (text == text.ToUpper() || text == "други" || text == "🥡 други 🥡")
            {
                if (startIndex != -1 && currentGroup != null)
                {
                    string name = string.Join(" ", split[startIndex..i]);

                    if (!double.TryParse(regex.Match(name).Groups[1].Value.Replace(",", "."), CultureInfo.InvariantCulture, out double price))
                    {
                        price = 10000;
                    }

                    currentGroup.Products.Add(new ProductDTO
                    {
                        Name = name,
                        Price = price
                    });

                    startIndex = -1;
                }

                currentGroup = new ProductGroupDTO
                {
                    Name = text,
                    Products = new List<ProductDTO>()
                };
                list.Add(currentGroup);
            }
            else if (currentGroup != null)
            {
                if (regex.IsMatch(text))
                {
                    if (startIndex != -1)
                    {
                        string name = string.Join(" ", split[startIndex..i]);

                        if (!double.TryParse(regex.Match(name).Groups[1].Value.Replace(",", "."), CultureInfo.InvariantCulture, out double price))
                        {
                            price = 10000;
                        }

                        currentGroup.Products.Add(new ProductDTO
                        {
                            Name = name,
                            Price = price
                        });
                    }

                    startIndex = i;
                }
            }
        }

        if (currentGroup != null && !string.IsNullOrEmpty(split[^1]))
        {
            string name = string.Join(" ", split[startIndex..^1]);

            if (!double.TryParse(regex.Match(name).Groups[1].Value.Replace(",", "."), CultureInfo.InvariantCulture, out double price))
            {
                price = 10000;
            }

            currentGroup.Products.Add(new ProductDTO
            {
                Name = name,
                Price = price
            });
        }

        await Http.PostAsJsonAsync("App/ReplaceMenu", list);
        URIHelper.NavigateTo("/");
    }

    [GeneratedRegex("([0-9,]+)[ ]*лв")]
    private static partial Regex MoneyRegex();
}
