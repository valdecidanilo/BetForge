using System;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

public class Test : MonoBehaviour
{
    public TMP_Text typeCurrency;
    public TMP_Text valueNumber;
    public enum CurrencyCode
    {
        USD,
        EUR,
        GBP,
        BRL,
        JPY,
        CNY,
        INR,
        RUB,
        KRW,
        TRY,
        MXN,
        CAD
    }
    public CurrencyCode currencyCode = CurrencyCode.USD;
    private void Start()
    {
        var api = new CurrencyApiFake();
        var jsonResponse = api.GetCurrencyFormat(currencyCode.ToString());
        var formatter = JsonConvert.DeserializeObject<CurrencyFormatter>(jsonResponse);
        
        const decimal value = 1234567.89m;
        
        typeCurrency.SetText($"Currency: {formatter.FormatCurrency(value)}");
        valueNumber.SetText(formatter.FormatNumber(value));
    }
}
