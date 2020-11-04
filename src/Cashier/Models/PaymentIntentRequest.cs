using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashier.Models
{
    public class PaymentIntentRequest
    {
        public string TransactionReference { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public string ConfirmationPageUrl { get; set; }
        public string FailurePageUrl { get; set; }
        public string CallbackUrl { get; set; }
        public string AdditionalData { get; set; }
        public DateTime DirectDebitStartDate { get; set; }
        public DateTime? DirectDebitTrialDateEnd { get; set; }
        public PaymentFrequencyUnit DirectDebitFrequencyUnit { get; set; } = PaymentFrequencyUnit.Month;
        public int DirectDebitFrequencyInterval { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerUniqueReference { get; set; }
        public CustomerAddress CustomerAddress { get; set; }
        public bool MotoMode { get; set; }

        public PaymentIntentType PaymentIntentType { get; set; }

        public static PaymentIntentRequest CreateCardPayment(string transactionReference, string description, double amount, string currency, string customerEmail, string confirmationPageUrl, string failurePageUrl, string callbackUrl, string additionalData, bool motoMode = false)
        {
            var pi = new PaymentIntentRequest();
            pi.PaymentIntentType = PaymentIntentType.Card;
            pi.Amount = amount;
            pi.Currency = currency;
            pi.TransactionReference = transactionReference;
            pi.Description = description;
            pi.CustomerEmail = customerEmail;
            pi.ConfirmationPageUrl = confirmationPageUrl;
            pi.FailurePageUrl = failurePageUrl;
            pi.CallbackUrl = callbackUrl;
            pi.AdditionalData = additionalData;
            pi.MotoMode = motoMode;
            return pi;
        }

        public static PaymentIntentRequest CreateDirectDebit(string transactionReference, string description, double amount, string currency, DateTime directDebitStartDate, PaymentFrequencyUnit directDebitFrequencyUnit, int directDebitFrequencyInterval, string customerEmail, string customerUniqueReference, CustomerAddress customerAddress, string confirmationPageUrl, string failurePageUrl, string callbackUrl, string additionalData, DateTime? directDebitTrialDateEnd = null)
        {
            var pi = new PaymentIntentRequest();
            pi.PaymentIntentType = PaymentIntentType.DirectDebit;
            pi.Amount = amount;
            pi.Currency = currency;
            pi.TransactionReference = transactionReference;
            pi.Description = description;
            pi.DirectDebitStartDate = directDebitStartDate;
            pi.DirectDebitFrequencyUnit = directDebitFrequencyUnit;
            pi.DirectDebitFrequencyInterval = directDebitFrequencyInterval;
            pi.CustomerEmail = customerEmail;
            pi.CustomerUniqueReference = customerUniqueReference;
            pi.CustomerAddress = customerAddress;
            pi.ConfirmationPageUrl = confirmationPageUrl;
            pi.FailurePageUrl = failurePageUrl;
            pi.CallbackUrl = callbackUrl;
            pi.AdditionalData = additionalData;
            pi.DirectDebitTrialDateEnd = directDebitTrialDateEnd;
            return pi;
        }

        protected PaymentIntentRequest()
        {

        }

        
    }
}
