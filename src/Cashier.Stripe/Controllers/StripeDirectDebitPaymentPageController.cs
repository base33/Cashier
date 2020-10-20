using Cashier.Models;
using Stripe;
using Stripe.Checkout;
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
    public class StripeDirectDebitPaymentPageController : RenderMvcController
    {
        protected ICashier PaymentService { get; }

        public StripeDirectDebitPaymentPageController(ICashier paymentService)
        {
            PaymentService = paymentService;
        }

        public ActionResult Render(string tr)
        {
            var paymentIntent = PaymentService.GetPaymentIntentByTransactionRef(tr);

            StripeConfiguration.ApiKey = ConfigurationManager.AppSettings["Cashier:Stripe:Secret"];

            string domain = $"{HttpContext.Request.Url.Scheme}://{HttpContext.Request.Url.Host}{(HttpContext.Request.Url.Port > 443 ? ":" + HttpContext.Request.Url.Port : "")}";


            var sessionService = new SessionService();
            var session = sessionService.Create(new SessionCreateOptions
            {
                SuccessUrl = domain + "/stripe-dd-success?sessionId={CHECKOUT_SESSION_ID}",
                CancelUrl = domain + "/stripe-dd-failure?sessionId={CHECKOUT_SESSION_ID}",
                Mode = "setup",
                PaymentMethodTypes = new List<string> { "bacs_debit" },
                Metadata = new Dictionary<string, string>
                {
                    { "TransactionRef", paymentIntent.TransactionReference }
                },
                //PaymentIntentData = new SessionPaymentIntentDataOptions
                //{
                //    SetupFutureUsage = "off_session",
                //    ReceiptEmail = paymentIntent.CustomerEmail
                //},
                //CustomerEmail = paymentIntent.CustomerEmail,
                BillingAddressCollection = "auto",
                //LineItems = new List<SessionLineItemOptions>
                //{
                //    new SessionLineItemOptions
                //    {
                //        Amount = (long)paymentIntent.Amount * 100,
                //        Description = paymentIntent.Description,
                //        Quantity = 1,
                //        Currency = "gbp",
                //        Name = "Direct Debit - " + paymentIntent.Amount
                //        //PriceData = new SessionLineItemPriceDataOptions
                //        //{
                //        //    UnitAmount = (long)paymentIntent.Amount * 100,
                //        //    Recurring = new SessionLineItemPriceDataRecurringOptions
                //        //    {
                //        //        Interval = "month"
                //        //    },
                //        //    Currency = "gbp",
                //        //    ProductData = new SessionLineItemPriceDataProductDataOptions
                //        //    {
                //        //        Name = "Direct Debit - " + paymentIntent.Amount,
                //        //        Description = paymentIntent.Description
                //        //    }
                //        //}
                //    }
                //}
            });


            ViewBag.StripeSessionId = session.Id;

            return View("StripeDirectDebitPayment", CurrentPage);
        }

        public ActionResult Success(string sessionId)
        {
            var paymentIntent = UpdatePaymentIntent(sessionId);

            return Redirect(paymentIntent.ConfirmationPageUrl);
        }

        public ActionResult Failure(string sessionId)
        {
            var paymentIntent = UpdatePaymentIntent(sessionId);

            return Redirect(paymentIntent.FailurePageUrl);
        }

        protected Models.PaymentIntent UpdatePaymentIntent(string sessionId)
        {
            StripeConfiguration.ApiKey = ConfigurationManager.AppSettings["Cashier:Stripe:Secret"];
            var sessionService = new SessionService();
            var session = sessionService.Get(sessionId);

            var setupIntentService = new SetupIntentService();
            var setupIntent = setupIntentService.Get(session.SetupIntentId);

            var paymentIntent = PaymentService.GetPaymentIntentByTransactionRef(session.Metadata["TransactionRef"]);

            if (string.IsNullOrEmpty(setupIntent.PaymentMethodId))
            {
                paymentIntent.PaymentStatus = PaymentStatus.Failed;
                PaymentService.UpdatePaymentStatus(paymentIntent.TransactionReference, session.PaymentIntentId, PaymentStatus.Failed);
                return paymentIntent;
            }

            var ddPriceName = $"Direct Debit - {paymentIntent.DirectDebitFrequencyInterval} {Enum.GetName(typeof(PaymentFrequencyUnit), paymentIntent.DirectDebitFrequencyUnit)}{(paymentIntent.DirectDebitFrequencyInterval > 1 ? "s" : "")} - {paymentIntent.Amount}";

            var productService = new ProductService();
            var product = productService.List().FirstOrDefault(p => p.Description == "Direct Debit");

            if (product == null)
            {
                product = productService.Create(new ProductCreateOptions
                {
                    Name = ddPriceName,
                    Type = "service"
                });

            }

            var priceService = new PriceService();
            var price = priceService.List().FirstOrDefault(p => p.Nickname == ddPriceName);
            if (price == null)
            {
                price = priceService.Create(new PriceCreateOptions
                {
                    Nickname = ddPriceName,
                    Product = product.Id,
                    UnitAmount = (long)paymentIntent.Amount * 100,
                    Currency = paymentIntent.Currency,
                    Recurring = new PriceRecurringOptions
                    {
                        Interval = Enum.GetName(typeof(PaymentFrequencyUnit), paymentIntent.DirectDebitFrequencyUnit).ToLower(),
                        IntervalCount = paymentIntent.DirectDebitFrequencyInterval,
                        UsageType = "licensed"
                    }
                });
            }

            var customerService = new CustomerService();
            var customer = customerService.List().FirstOrDefault(c => c.Name == paymentIntent.CustomerUniqueReference);
            if (customer == null)
                customer = customerService.Create(new CustomerCreateOptions
                {
                    Name = paymentIntent.CustomerUniqueReference,
                    Description = paymentIntent.CustomerUniqueReference,
                    PaymentMethod = setupIntent.PaymentMethodId,
                    Email = paymentIntent.CustomerEmail,
                    Address = new AddressOptions
                    {
                        Line1 = paymentIntent.CustomerAddressLines,
                        City = paymentIntent.CustomerCity,
                        Country = paymentIntent.CustomerCountry,
                        PostalCode = paymentIntent.CustomerPostcode
                    }
                });
            else
            {
                var paymentMethodService = new PaymentMethodService();
                paymentMethodService.Attach(setupIntent.PaymentMethodId, new PaymentMethodAttachOptions
                {
                    Customer = customer.Id
                });
            }

            var subscriptionService = new SubscriptionService();
            var subscriptionCreate = new SubscriptionCreateOptions
            {
                Customer = customer.Id,
                DefaultPaymentMethod = setupIntent.PaymentMethodId,
                ProrationBehavior = "none",
                BillingCycleAnchor = paymentIntent.DirectDebitStartDate,
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                        Price = price.Id
                    }
                }
            };
            if(paymentIntent.DirectDebitTrialDateEnd.HasValue)
            {
                subscriptionCreate.TrialEnd = paymentIntent.DirectDebitTrialDateEnd;
            }
            var subscription = subscriptionService.Create(subscriptionCreate);

            paymentIntent.PaymentStatus = PaymentStatus.Succeeded;
            PaymentService.UpdatePaymentStatus(paymentIntent.TransactionReference, subscription.Id, PaymentStatus.Succeeded);

            return paymentIntent;
        }
    }
}

/*
 * var ddPriceName = "Direct Debit - " + paymentIntent.Amount;

            var productService = new ProductService();
            var product = productService.List().FirstOrDefault(p => p.Description == "Direct Debit");

            if(product == null)
            {
                product = productService.Create(new ProductCreateOptions
                {
                    Name = ddPriceName,
                    Type = "service"
                });

            }

            var priceService = new PriceService();
            var price = priceService.List().FirstOrDefault(p => p.Nickname == ddPriceName);
            if (price == null)
            {
                price = priceService.Create(new PriceCreateOptions
                {
                    Nickname = ddPriceName,
                    Product = product.Id,
                    UnitAmount = (long)paymentIntent.Amount * 100,
                    Currency = "gbp",
                    Recurring = new PriceRecurringOptions
                    {
                        Interval = "month",
                        UsageType = "licensed"
                    }
                });
            }
*/