using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cashier.Models
{
    public class PaymentIntentCreated
    {
        public ActionResult ActionToPayment { get; set; }
        public PaymentIntent PaymentIntent { get; set; }
    }
}
