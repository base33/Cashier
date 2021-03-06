﻿using Cashier.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Cashier.Umbraco
{
    public class CashierComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register(typeof(IPaymentIntentRepository), typeof(PaymentIntentRepository), Lifetime.Request);
            composition.Register<ICashier>(factory => {
                return new CashierService(factory.GetInstance<IPaymentIntentRepository>(), factory.Concrete as LightInject.ServiceContainer);
                }, Lifetime.Request);

            composition.Components().Append<CashierComponent>();

            RepoDb.SqlServerBootstrap.Initialize();
        }
    }
}
