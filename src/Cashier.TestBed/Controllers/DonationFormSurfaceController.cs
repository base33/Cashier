using Cashier.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace Cashier.TestBed.Controllers
{
    public class DonationFormSurfaceController : SurfaceController
    {
        public ICashier PaymentService { get; }

        public DonationFormSurfaceController(ICashier paymentService)
        {
            PaymentService = paymentService;
        }

        [ChildActionOnly]
        public ActionResult Render(string tr, string hs)
        {
            var hasCompleteDonation = HasCompleteDonation(tr, hs);

            return !hasCompleteDonation 
                ? PartialView("donate", new DonationFormModel())
                : PartialView("donate_complete", new DonationFormModel());
        }

        [HttpPost]
        public ActionResult HandleDonationForm(DonationFormModel model)
        {
            if (!ModelState.IsValid)
                return CurrentUmbracoPage();

            //store the donation as JSON (possible solution)
            string jsonDonation = JsonConvert.SerializeObject(model);

            PaymentIntentRequest paymentIntentRequest;
            model.DonationType = "Regular";

            if (model.DonationType == "Single")
                paymentIntentRequest = PaymentIntentRequest.CreateCardPayment(
                    Guid.NewGuid().ToString(),
                    "a constituent ID",
                    model.Amount,
                    "gbp",
                    "craig@mentordigital.co.uk",
                    CurrentPage.Url,
                     Umbraco.Content(1148).Url,
                    "/umbraco/surface/DonationFormSurface/HandleCallback",
                    jsonDonation
                );
            else
                paymentIntentRequest = PaymentIntentRequest.CreateDirectDebit(
                    Guid.NewGuid().ToString(),
                    "a constituent ID",
                    model.Amount,
                    "gbp",
                    DateTime.UtcNow.AddHours(3),
                    3, //three months
                    "craig@mentordigital.co.uk",
                    "craig@mentordigital.co.uk",
                    new CustomerAddress { AddressLines = "Street", City= "Bristol", Country = "United Kingdom", Postcode = "BS13 2xs" },
                    CurrentPage.Url,
                     Umbraco.Content(1148).Url,
                    "/umbraco/surface/DonationFormSurface/HandleCallback",
                    jsonDonation
                );

            var creationResult = PaymentService.CreateNewPaymentIntent(paymentIntentRequest);

            return creationResult.ActionToPayment;
        }

        [HttpGet]
        public ActionResult HandleCallback(string tr, string hs)
        {
            var paymentIntent = PaymentService.GetPaymentIntentByTransactionRef(tr);
            
            //validate the handshake matches
            if(paymentIntent.HandShake != hs)
            {
                return new HttpStatusCodeResult(400);
            }

            if (paymentIntent.PaymentIntentType == PaymentIntentType.Card && paymentIntent.PaymentStatus == PaymentStatus.Succeeded)
            {
                //do something now that payment was successful
            }
            else
            {
                //payment wasn't taken and failure page was met
            }


            return new HttpStatusCodeResult(200);
        }

        public bool HasCompleteDonation(string tr, string hs)
        {
            if(!string.IsNullOrEmpty(tr) && !string.IsNullOrEmpty(hs))
            {
                var paymentIntent = PaymentService.GetPaymentIntentByTransactionRef(tr);

                return paymentIntent.HandShake == hs;
            }

            return false;
        }
    }

    public class DonationFormModel
    {
        [Required]
        [Display(Name = "Please enter how much you wish to donate")]
        public double Amount { get; set; }

        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public string DonationType { get; set; }
    }
}