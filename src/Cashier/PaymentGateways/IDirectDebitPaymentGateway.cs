using Cashier.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cashier.PaymentGateways
{
    public interface IDirectDebitPaymentGateway
    {
        ActionResult PayNow(PaymentIntent paymentIntent);
    }
}
