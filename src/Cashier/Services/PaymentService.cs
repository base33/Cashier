using LightInject;
using Cashier.Data;
using Cashier.Models;
using Cashier.PaymentGateways;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Cashier.Services
{
    public class PaymentService : IPaymentService
    {
        protected IPaymentIntentRepository Repository { get; }
        protected ICardPaymentGateway CardPaymentGateway { get; }
        protected IDirectDebitPaymentGateway DirectDebitPaymentGateway { get; }

        public PaymentService(IPaymentIntentRepository repository, ServiceContainer serviceContainer)
        {
            Repository = repository;

            CardPaymentGateway = serviceContainer.TryGetInstance<ICardPaymentGateway>(); ;
            DirectDebitPaymentGateway = serviceContainer.TryGetInstance<IDirectDebitPaymentGateway>();
        }

        public PaymentIntentCreated CreateNewPaymentIntent(PaymentIntentRequest request)
        {
            var paymentIntent = new PaymentIntent
            {
                TransactionReference = request.TransactionReference,
                Description = request.Description,
                Amount = request.Amount,
                CustomerEmail = request.CustomerEmail,
                CallbackUrl = request.CallbackUrl,
                ConfirmationPageUrl = request.ConfirmationPageUrl,
                FailurePageUrl = request.FailurePageUrl,
                AdditionalData = request.AdditionalData,
                HandShake = Guid.NewGuid().ToString(),
                PaymentStatus = PaymentStatus.NothingYet,
                PaymentIntentType = request.PaymentIntentType
            };

            if (request.PaymentIntentType == PaymentIntentType.DirectDebit)
            {
                paymentIntent.DirectDebitStartDate = request.DirectDebitStartDate;
                paymentIntent.CustomerAddressLines = request.CustomerAddress.AddressLines;
                paymentIntent.CustomerCity = request.CustomerAddress.City;
                paymentIntent.CustomerCountry = request.CustomerAddress.Country;
                paymentIntent.CustomerPostcode = request.CustomerAddress.Postcode;
                paymentIntent.CustomerUniqueReference = request.CustomerUniqueReference;
            }

            var paymentGatewayAction = paymentIntent.PaymentIntentType == PaymentIntentType.Card ? CardPaymentGateway.PayNow(paymentIntent) : DirectDebitPaymentGateway.PayNow(paymentIntent);

            paymentIntent.CallbackUrl = paymentIntent.CallbackUrl + (paymentIntent.CallbackUrl.Contains("?") ? "&" : "?") + $"tr={paymentIntent.TransactionReference}&hs={paymentIntent.HandShake}";
            paymentIntent.ConfirmationPageUrl = paymentIntent.ConfirmationPageUrl + (paymentIntent.ConfirmationPageUrl.Contains("?") ? "&" : "?") + $"tr={paymentIntent.TransactionReference}&hs={paymentIntent.HandShake}";

            paymentIntent = Repository.SavePaymentIntent(paymentIntent);

            return new PaymentIntentCreated
            {
                ActionToPayment = paymentGatewayAction,
                PaymentIntent = paymentIntent
            };
        }

        public PaymentIntent SavePaymentIntent(PaymentIntent paymentIntent)
        {
            return Repository.SavePaymentIntent(paymentIntent);
        }

        public void UpdatePaymentStatus(string transactionReference, string externalReference, PaymentStatus paymentStatus)
        {
            //call callback url
            var paymentIntent = Repository.GetPaymentIntent(transactionReference);
            paymentIntent.PaymentStatus = paymentStatus;
            paymentIntent.Updated = DateTime.Now;
            paymentIntent.ExternalReference = externalReference;
            Repository.SavePaymentIntent(paymentIntent);

            var uri = paymentIntent.CallbackUrl.StartsWith("/") ? new Uri(HttpContext.Current.Request.Url, paymentIntent.CallbackUrl) : new Uri(paymentIntent.CallbackUrl);

            var request = WebRequest.Create(uri);
            request.Method = "GET";

            using (var response = request.GetResponse())
            {

            }
        }

        public PaymentIntent GetPaymentIntentByTransactionRef(string transactionReference)
        {
            return Repository.GetPaymentIntent(transactionReference);
        }

        public bool ValidateHandshakeByTransactionReference(string transactionReference, string handshake)
        {
            var paymentIntent = Repository.GetPaymentIntent(transactionReference);

            return paymentIntent == null || paymentIntent.HandShake != handshake;
        }
    }
}
