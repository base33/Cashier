using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core.Composing;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace Cashier.Stripe.Composers
{
    public class RegisterCustomRoutesComposer : ComponentComposer<RegisterCustomRoutesComponent>
    {
    }

    public class RegisterCustomRoutesComponent : IComponent
    {
        public void Initialize()
        {
            RouteTable.Routes.MapUmbracoRoute("Stripe Card Payment Page", ConfigurationManager.AppSettings["Cashier:Stripe:CardPaymentUrl"], new
            {
                controller = "StripeCardPaymentPage",
                action = "Render",
                id = UrlParameter.Optional
            }, new UmbracoVirtualNodeByIdRouteHandler(int.Parse(ConfigurationManager.AppSettings["Cashier:Stripe:CardPaymentPageVirtualNode"])));

            RouteTable.Routes.MapUmbracoRoute("Stripe Direct Debit Payment Page", ConfigurationManager.AppSettings["Cashier:Stripe:DirectDebitPaymentUrl"], new
            {
                controller = "StripeDirectDebitPaymentPage",
                action = "Render",
                id = UrlParameter.Optional
            }, new UmbracoVirtualNodeByIdRouteHandler(int.Parse(ConfigurationManager.AppSettings["Cashier:Stripe:DirectDebitPaymentPageVirtualNode"])));

            RouteTable.Routes.MapUmbracoRoute("Stripe Direct Debit Success Page","stripe-dd-success", new
            {
                controller = "StripeDirectDebitPaymentPage",
                action = "Success",
                id = UrlParameter.Optional
            }, new UmbracoVirtualNodeByIdRouteHandler(int.Parse(ConfigurationManager.AppSettings["Cashier:Stripe:DirectDebitPaymentPageVirtualNode"])));

            RouteTable.Routes.MapUmbracoRoute("Stripe Direct Debit Failure Page", "stripe-dd-failure", new
            {
                controller = "StripeDirectDebitPaymentPage",
                action = "Success",
                id = UrlParameter.Optional
            }, new UmbracoVirtualNodeByIdRouteHandler(int.Parse(ConfigurationManager.AppSettings["Cashier:Stripe:DirectDebitPaymentPageVirtualNode"])));
        }

        public void Terminate()
        {

        }
    }
}
