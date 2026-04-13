namespace LegacyRenewalApp.Interfaces;

public interface INotificationService
{
    void Send(Customer customer, RenewalInvoice invoice);
}