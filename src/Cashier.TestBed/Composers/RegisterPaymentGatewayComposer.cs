using Cashier.PaymentGateways;
using Cashier.Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Composing;
using Umbraco.Web;

namespace Cashier.TestBed.Composers
{
    public class RegisterPaymentGatewayComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register(typeof(ICardPaymentGateway), typeof(StripeCardPaymentGateway), Lifetime.Request);
            composition.Register(typeof(IDirectDebitPaymentGateway), typeof(StripeDirectDebitPaymentGateway), Lifetime.Request);
        }
    }
}