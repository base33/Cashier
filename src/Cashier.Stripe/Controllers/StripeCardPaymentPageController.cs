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

            //create the payment intent in STRIPE
            var stripePI = new PaymentIntentCreateOptions
            {
                Amount = (long)paymentIntent.Amount * 100,
                Currency = paymentIntent.Currency,
                PaymentMethodTypes = new List<string>
                {
                    "card"
                },
                Description = paymentIntent.Description
            };

            stripePI.Metadata = new Dictionary<string, string>();
            stripePI.Metadata["TransactionReference"] = paymentIntent.TransactionReference;

            var service = new PaymentIntentService();
            var response = service.Create(stripePI);

            ViewBag.TransactionReference = paymentIntent.TransactionReference;
            ViewBag.ClientSecret = response.ClientSecret;
            ViewBag.StripePaymentIntentId = response.Id;
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
