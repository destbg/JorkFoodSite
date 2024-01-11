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
        Regex moneyRegex = MoneyRegex();
        Regex gramRegex = GramRegex();
        Regex promoRegex = PromoRegex();
        Regex quantityRegex = QuantityRegex();
        Regex ingredientsRegex = IngredientsRegex();
        Regex addinsRegex = AddinsRegex();
        int startIndex = -1;

        string GetTitle(int endIndex)
        {
            return string.Join(" ", split[startIndex..endIndex]);
        }

        void AddProduct(string title)
        {
            Match promoMatch = promoRegex.Match(title);

            if (promoMatch.Success)
            {
                double promoPrice = ParseDouble(promoMatch, 2);
                double oldPrice = ParseDouble(promoMatch, 3);

                currentGroup.Products.Add(new ProductDTO
                {
                    Title = promoMatch.Groups[1].Value,
                    Ingredients = title.Replace(promoMatch.Value, "")
                        .Replace("( промо пакетите за деня не могат да се комбинират с други наши промоции и или отстъпки )", "")
                        .Trim(),
                    Price = promoPrice,
                    OldPromoPrice = oldPrice,
                    BoxCount = 2,
                    BoxPrice = .2,
                });

                return;
            }

            Match moneyMatch = moneyRegex.Match(title);
            double price = ParseDouble(moneyMatch, 1);
            if (moneyMatch.Success)
            {
                title = title.Replace(moneyMatch.Value, "");
            }

            Match gramMatch = gramRegex.Match(title);
            int? grams = ParseInt(gramMatch, 1);
            if (gramMatch.Success)
            {
                title = title.Replace(gramMatch.Value, "");
            }

            Match quantityMatch = quantityRegex.Match(title);
            if (quantityMatch.Success)
            {
                title = title.Replace(quantityMatch.Value, quantityMatch.Value.Replace("/", " ").Replace(".", ""));
            }

            Match ingredientsMatch = ingredientsRegex.Match(title);
            string? ingredients = null;
            if (ingredientsMatch.Success && !ingredientsMatch.Value.All(f => char.IsLetter(f) || f == '/' || f == ' '))
            {
                title = title.Replace(ingredientsMatch.Value, "");
                ingredients = ingredientsMatch.Value.Replace("/", "");
            }

            currentGroup.Products.Add(new ProductDTO
            {
                Title = title.Trim(' ', ',', '.', '/').Replace("  ", " "),
                Ingredients = ingredients,
                Grams = grams,
                Price = price,
                BoxCount = currentGroup.Name.Contains("ХЛЯБ") ? 0 : 1,
                BoxPrice = currentGroup.Name.Contains("ХЛЯБ") ? 0 : .2,
            });
        }

        for (int i = 0; i < split.Length; i++)
        {
            string text = split[i].Trim();

            if (text == text.ToUpper())
            {
                if (startIndex != -1 && currentGroup != null)
                {
                    AddProduct(GetTitle(i));
                    startIndex = -1;
                }

                currentGroup = new ProductGroupDTO
                {
                    Name = text == "В ГЮВЕЧЕ"
                        ? $"🍗{text}🍗"
                        : text == "НА ТИГАН"
                        ? $"🍳{text}🍳"
                        : text,
                    Products = new List<ProductDTO>(),
                };
                if (text.Contains("ПРОМО") || text.Contains("МЕНЮ"))
                {
                    list.Insert(0, currentGroup);
                }
                else
                {
                    list.Add(currentGroup);
                }
            }
            else if (currentGroup != null)
            {
                if (moneyRegex.IsMatch(text))
                {
                    if (startIndex != -1)
                    {
                        string title = GetTitle(i);

                        if (title.StartsWith('-'))
                        {
                            ProductDTO product = currentGroup.Products[^1];

                            product.Ingredients ??= string.Empty;
                            product.Ingredients += " " + title;
                        }
                        else
                        {
                            AddProduct(title);
                        }
                    }

                    startIndex = i;
                }
            }
        }

        if (currentGroup != null && !string.IsNullOrEmpty(split[^1]))
        {
            AddProduct(GetTitle(split.Length));
        }

        list = list.Where(f => f.Products.Count > 0).ToList();

        await Http.PostAsJsonAsync("App/ReplaceMenu", list);
        URIHelper.NavigateTo("/home");
    }

    private static double ParseDouble(Match match, int index)
    {
        if (!match.Success || !double.TryParse(match.Groups[index].Value.Replace(",", "."), CultureInfo.InvariantCulture, out double val))
        {
            return 10000;
        }

        return val;
    }

    private static int? ParseInt(Match match, int index)
    {
        if (!match.Success || !int.TryParse(match.Groups[index].Value, out int val))
        {
            return null;
        }

        return val;
    }

    [GeneratedRegex("([0-9,.]+)[ ]*лв")]
    private static partial Regex MoneyRegex();

    [GeneratedRegex("([0-9]+)[ ]*гр?")]
    private static partial Regex GramRegex();

    [GeneratedRegex("\\/?([0-9]+)[ ]*бр\\/?")]
    private static partial Regex QuantityRegex();

    [GeneratedRegex("\\/.+\\/")]
    private static partial Regex IngredientsRegex();

    [GeneratedRegex("\\s+-([а-я0-9, ]+)")]
    private static partial Regex AddinsRegex();

    [GeneratedRegex(".?([а-яА-Я ]+) [>]+ [а-яА-Я ]+([0-9,]+)[ ]*лв[а-яА-Я \\(]+([0-9,.]+)[ ]*лв[ \\)]+")]
    private static partial Regex PromoRegex();
}
