using Microsoft.Owin;
using Cashier.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Composing;
using Umbraco.Web.Models.ContentEditing;

namespace Cashier.Umbraco
{
    public class CashierComponent : IComponent
    {
        protected IPaymentIntentRepository Repository { get; }

        public CashierComponent(IPaymentIntentRepository repository)
        {
            Repository = repository;
        }

        public void Initialize()
        {
            Repository.EnsureCreated();
        }

        public void Terminate()
        {
            
        }
    }
}
