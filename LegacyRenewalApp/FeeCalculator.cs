using System;
using LegacyRenewalApp.Interfaces;

namespace LegacyRenewalApp;

public class FeeCalculator : IFeeCalculator
{
    public (decimal, decimal, string) Calculate(string plan, string payment, decimal subtotal, bool support)
    {
        decimal supportFee = support ? GetSupport(plan) : 0;
        decimal paymentFee = GetPayment(payment, subtotal + supportFee);

        return (supportFee, paymentFee, "");
    }

    private decimal GetSupport(string plan) => plan switch
    {
        "START" => 250,
        "PRO" => 400,
        "ENTERPRISE" => 700,
        _ => 0
    };

    private decimal GetPayment(string method, decimal amount) => method switch
    {
        "CARD" => amount * 0.02m,
        "BANK_TRANSFER" => amount * 0.01m,
        "PAYPAL" => amount * 0.035m,
        "INVOICE" => 0,
        _ => throw new ArgumentException("Unsupported payment")
    };
}