using Cashier.Models;
using Cashier.PaymentGateways;
using Cashier.PaymentGateways.Models;
using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cashier.Stripe
{
    public class StripeDirectDebitPaymentGateway : IDirectDebitPaymentGateway
    {
        public System.Web.Mvc.ActionResult PayNow(Cashier.Models.PaymentIntent paymentIntent)
        {
            string url = "/" + ConfigurationManager.AppSettings["Cashier:Stripe:DirectDebitPaymentUrl"] + "/?tr=" + paymentIntent.TransactionReference;

            return new RedirectResult(url);
        }
    }
}
