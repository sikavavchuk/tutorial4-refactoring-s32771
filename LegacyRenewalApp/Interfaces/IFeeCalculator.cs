namespace LegacyRenewalApp.Interfaces;

public interface IFeeCalculator
{
    (decimal supportFee, decimal paymentFee, string notes) Calculate(string plan, string payment, decimal subtotal, bool support);
}