using System;
using LegacyRenewalApp;
using LegacyRenewalApp.Interfaces;

public class SubscriptionRenewalService
{
    private readonly ICustomerRepository _customers;
    private readonly ISubscriptionPlanRepository _plans;
    private readonly IDiscountCalculator _discounts;
    private readonly IFeeCalculator _fees;
    private readonly ITaxCalculator _tax;
    private readonly IInvoiceRepository _invoices;
    private readonly INotificationService _notifications;
    
    public SubscriptionRenewalService()
        : this(
            new CustomerRepository(),
            new SubscriptionPlanRepository(),
            new DiscountCalculator(),
            new FeeCalculator(),
            new TaxCalculator(),
            new InvoiceRepository(),
            new NotificationService())
    {
    }

    public SubscriptionRenewalService(
        ICustomerRepository customers,
        ISubscriptionPlanRepository plans,
        IDiscountCalculator discounts,
        IFeeCalculator fees,
        ITaxCalculator tax,
        IInvoiceRepository invoices,
        INotificationService notifications)
    {
        _customers = customers;
        _plans = plans;
        _discounts = discounts;
        _fees = fees;
        _tax = tax;
        _invoices = invoices;
        _notifications = notifications;
    }

    // ✅ MATCHES Program call exactly
    public RenewalInvoice CreateRenewalInvoice(
        int customerId,
        string planCode,
        int seatCount,
        string paymentMethod,
        bool includePremiumSupport,
        bool useLoyaltyPoints)
    {
        var customer = _customers.GetById(customerId);
        var plan = _plans.GetByCode(planCode);

        if (!customer.IsActive)
            throw new InvalidOperationException("Inactive customer");

        decimal baseAmount = (plan.MonthlyPricePerSeat * seatCount * 12) + plan.SetupFee;

        var discount = _discounts.Calculate(
            customer,
            plan,
            seatCount,
            useLoyaltyPoints,
            baseAmount);

        decimal subtotal = Math.Max(300, baseAmount - discount.Amount);

        var (supportFee, paymentFee, _) = _fees.Calculate(
            plan.Code,
            paymentMethod,
            subtotal,
            includePremiumSupport);

        decimal tax = _tax.Calculate(
            customer.Country,
            subtotal + supportFee + paymentFee);

        decimal final = Math.Max(
            500,
            subtotal + supportFee + paymentFee + tax);

        var invoice = new RenewalInvoice
        {
            InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{customerId}-{plan.Code}",
            CustomerName = customer.FullName,
            PlanCode = plan.Code,
            PaymentMethod = paymentMethod,
            SeatCount = seatCount,
            BaseAmount = baseAmount,
            DiscountAmount = discount.Amount,
            SupportFee = supportFee,
            PaymentFee = paymentFee,
            TaxAmount = tax,
            FinalAmount = final,
            Notes = discount.Notes,
            GeneratedAt = DateTime.UtcNow
        };

        _invoices.Save(invoice);
        _notifications.Send(customer, invoice);

        return invoice;
    }
}