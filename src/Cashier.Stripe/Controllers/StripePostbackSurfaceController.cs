using Cashier.Models;
using Stripe;
using ExStripe = Stripe;
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

            var paymentIntent = PaymentService.GetPaymentIntentByTransactionRef(model.TransactionReference);
            var service = new PaymentIntentService();

            ExStripe.PaymentIntent stripePI = null;
            if (paymentIntent.MotoMode == true)
            {
                //if it's a moto payment, we need to create the payment intent from 
                var servicePM = new PaymentMethodService();
                var paymentMethod = servicePM.Get(model.StripePaymentIntentId);
                var piCreate = new PaymentIntentCreateOptions
                {
                    Amount = (long)paymentIntent.Amount * 100,
                    Currency = paymentIntent.Currency,
                    Description = paymentIntent.Description,
                    Confirm = true,
                    PaymentMethod = model.StripePaymentIntentId,
                    PaymentMethodOptions = new PaymentIntentPaymentMethodOptionsOptions
                    {
                        Card = new PaymentIntentPaymentMethodOptionsCardOptions
                        {
                            Moto = true
                        }
                    }
                };
                piCreate.Metadata = new Dictionary<string, string>
                {
                    { "TransactionReference", paymentIntent.TransactionReference }
                };
                stripePI = service.Create(piCreate);
                model.StripePaymentIntentId = stripePI.Id;
            }

            
            stripePI = stripePI ?? service.Get(model.StripePaymentIntentId);

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
