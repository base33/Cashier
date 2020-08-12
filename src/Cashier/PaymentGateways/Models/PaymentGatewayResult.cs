using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashier.PaymentGateways.Models
{
    public class PaymentGatewayResult
    {
        public int? PaymentIntentId { get; set; }
        public bool Successful { get; set; }
    }
}
