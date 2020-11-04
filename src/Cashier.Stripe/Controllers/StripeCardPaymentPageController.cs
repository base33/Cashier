using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace Cashier.Stripe.Controllers
{
    public class StripeCardPaymentPageController : RenderMvcController
    {
        protected ICashier PaymentService { get; }

        public StripeCardPaymentPageController(ICashier paymentService)
        {
            PaymentService = paymentService;
        }

        public ActionResult Render(string tr)
        {
            var paymentIntent = PaymentService.GetPaymentIntentByTransactionRef(tr);

            StripeConfiguration.ApiKey = ConfigurationManager.AppSettings["Cashier:Stripe:Secret"];

            string intentId = "";
            string intentClientSecret = "";

            if (paymentIntent.MotoMode == true)
            {
                var serviceSI = new SetupIntentService();
                var setup = serviceSI.Create(new SetupIntentCreateOptions
                {
                    PaymentMethodTypes = new List<string>
                    {
                        "card"
                    },
                    Metadata = new Dictionary<string, string>
                    {
                        { "TransactionReference", paymentIntent.TransactionReference }
                    }
                });
                intentId = setup.Id;
                intentClientSecret = setup.ClientSecret;
            }
            else
            {
                var servicePI = new PaymentIntentService();
                var createPI = new PaymentIntentCreateOptions
                {
                    Amount = (long)paymentIntent.Amount * 100,
                    Currency = paymentIntent.Currency,
                    PaymentMethodTypes = new List<string>
                    {
                        "card"
                    },
                    Description = paymentIntent.Description
                };
                createPI.Metadata = new Dictionary<string, string>
                {
                    { "TransactionReference", paymentIntent.TransactionReference }
                };
                var responsePI = servicePI.Create(createPI);
                intentId = responsePI.Id;
                intentClientSecret = responsePI.ClientSecret;
            }

            ViewBag.PaymentIntent = paymentIntent;
            ViewBag.TransactionReference = paymentIntent.TransactionReference;
            ViewBag.ClientSecret = intentClientSecret;
            ViewBag.StripePaymentIntentId = intentId;
            ViewBag.TestMode = ConfigurationManager.AppSettings["Cashier:Stripe:LiveMode"].ToLower() == "false";

            return View("StripeCardPayment", CurrentPage);
        }
    }

    public class StripePayment
    {
        public string StripePaymentIntentId { get; set; }
        public string TransactionReference { get; set; }
    }
}
