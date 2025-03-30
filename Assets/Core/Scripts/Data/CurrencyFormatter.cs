using System;
using System.Globalization;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
public class CurrencyFormatter
{
    [JsonProperty("currency")] 
    public string Currency { get; set; }

    [JsonProperty("decimals")] 
    public int Decimals { get; set; } = 2; // Valor padrão

    [JsonProperty("decimal_separator")] 
    public string DecimalSeparator { get; set; } = "."; // Valor padrão

    [JsonProperty("group_separator")] 
    public string GroupSeparator { get; set; } = ","; // Valor padrão

    [JsonProperty("group_sizes")] 
    public int[] GroupSizes { get; set; } = { 3 }; // Valor padrão

    private static readonly Dictionary<string, string> CurrencySymbols = new()
    {
        { "USD", "$" }, { "EUR", "€" }, { "GBP", "£" }, { "BRL", "R$" },
        { "JPY", "¥" }, { "CNY", "¥" }, { "INR", "₹" }, { "RUB", "₽" },
        { "KRW", "₩" }, { "TRY", "₺" }, { "MXN", "$" }, { "CAD", "$" }
    };

    public string FormatCurrency(decimal value)
    {
        var culture = CreateCustomCulture();
        return value.ToString("C", culture);
    }

    public string FormatNumber(decimal value)
    {
        var culture = CreateCustomCulture();
        return value.ToString("N", culture);
    }

    private CultureInfo CreateCustomCulture()
    {
        return new CultureInfo("en-US")
        {
            NumberFormat = 
            {
                CurrencySymbol = GetCurrencySymbol(Currency),
                CurrencyDecimalDigits = Decimals,
                CurrencyDecimalSeparator = DecimalSeparator,
                CurrencyGroupSeparator = GroupSeparator,
                CurrencyGroupSizes = GroupSizes,
                
                // Configurações para formatação numérica geral
                NumberDecimalDigits = Decimals,
                NumberDecimalSeparator = DecimalSeparator,
                NumberGroupSeparator = GroupSeparator,
                NumberGroupSizes = GroupSizes
            }
        };
    }

    private static string GetCurrencySymbol(string currencyCode)
    {
        return CurrencySymbols.GetValueOrDefault(currencyCode, currencyCode);
    }

    public static CurrencyFormatter FromJson(string json)
    {
        return JsonConvert.DeserializeObject<CurrencyFormatter>(json);
    }
}