using System;
using System.Collections.Generic;
using LegacyRenewalApp.Interfaces;

namespace LegacyRenewalApp;

public class DiscountCalculator : IDiscountCalculator
{
    public DiscountResult Calculate(Customer c, SubscriptionPlan p, int seats, bool usePoints, decimal baseAmount)
    {
        decimal discount = 0;
        var notes = new List<string>();

        discount += Segment(c, p, baseAmount, notes);
        discount += Loyalty(c, baseAmount, notes);
        discount += Seats(seats, baseAmount, notes);
        discount += Points(c, usePoints, notes);

        return new DiscountResult
        {
            Amount = discount,
            Notes = string.Join("; ", notes)
        };
    }

    private decimal Segment(Customer c, SubscriptionPlan p, decimal baseAmount, List<string> notes)
    {
        var map = new Dictionary<string, decimal>
        {
            ["Silver"] = 0.05m,
            ["Gold"] = 0.10m,
            ["Platinum"] = 0.15m
        };

        if (c.Segment == "Education" && p.IsEducationEligible)
        {
            notes.Add("education discount");
            return baseAmount * 0.20m;
        }

        if (map.TryGetValue(c.Segment, out var rate))
        {
            notes.Add($"{c.Segment.ToLower()} discount");
            return baseAmount * rate;
        }

        return 0;
    }

    private decimal Loyalty(Customer c, decimal baseAmount, List<string> notes)
    {
        if (c.YearsWithCompany >= 5)
        {
            notes.Add("long-term loyalty discount");
            return baseAmount * 0.07m;
        }
        if (c.YearsWithCompany >= 2)
        {
            notes.Add("basic loyalty discount");
            return baseAmount * 0.03m;
        }
        return 0;
    }

    private decimal Seats(int seats, decimal baseAmount, List<string> notes)
    {
        if (seats >= 50) return Add(baseAmount, 0.12m, "large team discount", notes);
        if (seats >= 20) return Add(baseAmount, 0.08m, "medium team discount", notes);
        if (seats >= 10) return Add(baseAmount, 0.04m, "small team discount", notes);
        return 0;
    }

    private decimal Points(Customer c, bool usePoints, List<string> notes)
    {
        if (!usePoints || c.LoyaltyPoints <= 0) return 0;

        int used = Math.Min(200, c.LoyaltyPoints);
        notes.Add($"loyalty points used: {used}");
        return used;
    }

    private decimal Add(decimal baseAmount, decimal rate, string note, List<string> notes)
    {
        notes.Add(note);
        return baseAmount * rate;
    }
}