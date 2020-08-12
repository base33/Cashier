using Cashier.PaymentGateways;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Composing;

namespace Cashier.Umbraco
{
    public static class UmbracoCompositionExtensions
    {
        public static void RegisterDirectDebitPaymentGateway<T>(this Composition composition) where T : IDirectDebitPaymentGateway
        {
            composition.Register(typeof(IDirectDebitPaymentGateway), typeof(T), Lifetime.Request);
        }

        public static void RegisterCardPaymentGateway<T>(this Composition composition) where T : ICardPaymentGateway
        {
            composition.Register(typeof(ICardPaymentGateway), typeof(T), Lifetime.Request);
        }
    }
}
