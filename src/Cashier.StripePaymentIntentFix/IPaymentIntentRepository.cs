using System.Collections.Generic;

namespace Cashier.StripePaymentIntentFix
{
    public interface IPaymentIntentRepository
    {
        void EnsureCreated();
        PaymentIntent GetPaymentIntent(int id);
        PaymentIntent GetPaymentIntent(string transactionReference);
        PaymentIntent SavePaymentIntent(PaymentIntent paymentIntent);
        IEnumerable<PaymentIntent> GetAll(int skip, int take);
    }
}