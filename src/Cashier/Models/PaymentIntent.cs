using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashier.Models
{
    public class PaymentIntent
    {
        public int Id { get; set; }
        public string TransactionReference { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public DateTime? DirectDebitStartDate { get; set; } = null;
        public string CustomerEmail { get; set; }
        public string CustomerAddressLines { get; set; }
        public string CustomerCity { get; set; }
        public string CustomerCountry { get; set; }
        public string CustomerPostcode { get; set; }
        public string ConfirmationPageUrl { get; set; }
        public string FailurePageUrl { get; set; }
        public string CallbackUrl { get; set; }
        public string AdditionalData { get; set; }
        public PaymentIntentType PaymentIntentType { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public string ExternalReference { get; set; }
        public DateTime Updated { get; set; } = DateTime.Now;
        public string HandShake { get; set; }
        public string CustomerUniqueReference { get; set; }
    }

    public enum PaymentIntentType 
    {
        Card = 0,
        DirectDebit = 1
    }

    public enum PaymentStatus
    {
        /// <summary>
        /// Processing means the payment or direct debit setup hasn't finished yet
        /// </summary>
        Processing = 0,
        /// <summary>
        /// Succeeded means direct debit has been set up or payment has been taken
        /// </summary>
        Succeeded = 1,
        /// <summary>
        /// Failed or cancelled
        /// </summary>
        Failed = 2,
        /// <summary>
        /// Not heard back or changed
        /// </summary>
        NothingYet = 4
    }
}
