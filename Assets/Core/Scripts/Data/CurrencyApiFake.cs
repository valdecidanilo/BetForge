using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class CurrencyApiFake
{
    private readonly Dictionary<string, CurrencyFormatter> _currencyFormats = new()
    {
        {
            "BRL", new CurrencyFormatter
            {
                Currency = "BRL",
                Decimals = 2,
                DecimalSeparator = ",",
                GroupSeparator = ".",
                GroupSizes = new[] { 3 }
            }
        },
        {
            "USD", new CurrencyFormatter
            {
                Currency = "USD",
                Decimals = 2,
                DecimalSeparator = ".",
                GroupSeparator = ",",
                GroupSizes = new[] { 3 }
            }
        },
        {
            "JPY", new CurrencyFormatter
            {
                Currency = "JPY",
                Decimals = 0,
                DecimalSeparator = ".",
                GroupSeparator = ",",
                GroupSizes = new[] { 3 }
            }
        },
        {
            "INR", new CurrencyFormatter
            {
                Currency = "INR",
                Decimals = 2,
                DecimalSeparator = ".",
                GroupSeparator = ",",
                GroupSizes = new[] { 3, 2 }
            }
        }
    };

    public string GetCurrencyFormat(string currencyCode)
    {
        if (_currencyFormats.TryGetValue(currencyCode, out var formatter))
        {
            return JsonConvert.SerializeObject(formatter);
        }

        var defaultFormat = new CurrencyFormatter
        {
            Currency = currencyCode,
            Decimals = 2,
            DecimalSeparator = ".",
            GroupSeparator = ",",
            GroupSizes = new[] { 3 }
        };

        return JsonConvert.SerializeObject(defaultFormat);
    }
}