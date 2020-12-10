using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashier.StripePaymentIntentFix
{
    class Program
    {
        static void Main(string[] args)
        {
            RepoDb.SqlServerBootstrap.Initialize();

            var repo = new PaymentIntentRepository();

            var data = repo.GetAll(0, 3000);

            StripeConfiguration.ApiKey = ConfigurationManager.AppSettings["Cashier:Stripe:Secret"];

            var chargeApi = new Stripe.ChargeService();
            var charges = chargeApi.List(new ChargeListOptions
            {
                Limit = 5000
            });
            
            foreach (var item in data)
            {
                var paymentMethodApi = new Stripe.PaymentMethodService();
                var paymentIntentApi = new Stripe.PaymentIntentService();
                var invoiceApi = new Stripe.InvoiceService();
                if (item.ExternalReference?.StartsWith("pi_") ?? true)
                    continue;

                var payment = charges.FirstOrDefault(p => p.PaymentMethod == item.ExternalReference);

                if (payment != null && !string.IsNullOrEmpty(payment.PaymentIntentId))
                {
                    item.ExternalReference = payment.PaymentIntentId;
                    repo.SavePaymentIntent(item);
                }
                    
            }

            
        }
    }
}
