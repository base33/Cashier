using Cashier.Models;
using Cashier.PaymentGateways.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cashier.PaymentGateways
{
    public interface ICardPaymentGateway
    {
        ActionResult PayNow(PaymentIntent paymentIntent);
    }
}
