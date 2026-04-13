namespace LegacyRenewalApp.Interfaces;

public interface IDiscountCalculator
{
    DiscountResult Calculate(Customer customer, SubscriptionPlan plan, int seats, bool usePoints, decimal baseAmount);
}