﻿using Cashier.Models;

namespace Cashier.Data
{
    public interface IPaymentIntentRepository
    {
        void EnsureCreated();
        PaymentIntent GetPaymentIntent(int id);
        PaymentIntent GetPaymentIntent(string transactionReference);
        PaymentIntent SavePaymentIntent(PaymentIntent paymentIntent);
    }
}