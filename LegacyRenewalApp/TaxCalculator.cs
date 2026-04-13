using System.Collections.Generic;
using LegacyRenewalApp.Interfaces;

namespace LegacyRenewalApp;

public class TaxCalculator : ITaxCalculator
{
    private static readonly Dictionary<string, decimal> Rates = new()
    {
        ["Poland"] = 0.23m,
        ["Germany"] = 0.19m,
        ["Czech Republic"] = 0.21m,
        ["Norway"] = 0.25m
    };

    public decimal Calculate(string country, decimal amount)
    {
        var rate = Rates.TryGetValue(country, out var r) ? r : 0.20m;
        return amount * rate;
    }
}