namespace LegacyRenewalApp.Interfaces;

public interface ITaxCalculator
{
    decimal Calculate(string country, decimal amount);
}