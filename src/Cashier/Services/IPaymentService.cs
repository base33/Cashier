using Cashier.Models;

namespace Cashier.Services
{
    public interface IPaymentService
    {
        PaymentIntentCreated CreateNewPaymentIntent(PaymentIntentRequest request);
        PaymentIntent GetPaymentIntentByTransactionRef(string transactionReference);
        void UpdatePaymentStatus(string transactionReference, string externalReference, PaymentStatus paymentStatus);
        bool ValidateHandshakeByTransactionReference(string transactionReference, string handshake);
        PaymentIntent SavePaymentIntent(PaymentIntent paymentIntent);
    }
}