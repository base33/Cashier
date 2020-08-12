using Cashier.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Umbraco.Web.WebApi;

namespace Cashier.Controllers
{
    public class CashierBackofficeApiController : UmbracoAuthorizedApiController
    {
        public IPaymentIntentRepository Repo { get; }

        public CashierBackofficeApiController(IPaymentIntentRepository repo)
        {
            Repo = repo;
        }

        [HttpGet]
        public IEnumerable<PaymentIntentSummary> GetPaged(int skip, int take)
        {
            return Repo.GetAll(skip, take).Select(p => new PaymentIntentSummary
            {
                TransactionReference = p.TransactionReference,
                Amount = p.Amount,
                PaymentStatus = Enum.GetName(p.PaymentStatus.GetType(), p.PaymentStatus),
                PaymentIntentType = Enum.GetName(p.PaymentIntentType.GetType(), p.PaymentIntentType),
                Created = p.Created.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }
    }

    public class PaymentIntentSummary
    {
        public string TransactionReference { get; set; }
        public double Amount { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentIntentType { get; set; }
        public string Created { get; set; }
    }
}
