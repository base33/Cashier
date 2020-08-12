using Cashier.Models;
using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace Cashier.Stripe.Controllers
{
    public class StripePostbackSurfaceController : SurfaceController
    {
        protected ICashier PaymentService { get; }

        public StripePostbackSurfaceController(ICashier paymentService)
        {
            PaymentService = paymentService;
        }

        [HttpPost]
        public ActionResult HandleCardPaymentComplete(StripePayment model)
        {
            StripeConfiguration.ApiKey = ConfigurationManager.AppSettings["Cashier:Stripe:Secret"];

            var service = new PaymentIntentService();
            var stripePI = service.Get(model.StripePaymentIntentId);

            var paymentIntent = PaymentService.GetPaymentIntentByTransactionRef(model.TransactionReference);

            if (stripePI.Status == "succeeded" && stripePI.Metadata["TransactionReference"] == model.TransactionReference)
            {
                PaymentService.UpdatePaymentStatus(model.TransactionReference, model.StripePaymentIntentId, PaymentStatus.Succeeded);


                return Redirect(paymentIntent.ConfirmationPageUrl);
            }

            PaymentService.UpdatePaymentStatus(model.TransactionReference, model.StripePaymentIntentId, PaymentStatus.Failed);

            return Redirect(paymentIntent.FailurePageUrl);
        }
    }
}
