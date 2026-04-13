namespace LegacyRenewalApp.Interfaces;

public interface IInvoiceRepository
{
    void Save(RenewalInvoice invoice);
}