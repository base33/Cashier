using Cashier.Models;
using RepoDb;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashier.Data
{
    public class PaymentIntentRepository : IPaymentIntentRepository
    {
        protected static List<PaymentIntent> cache { get; set; } = new List<PaymentIntent>();

        public void EnsureCreated()
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["CashierDb"].ConnectionString).EnsureOpen())
            {
                connection.ExecuteNonQuery(@"
                    IF NOT (EXISTS (SELECT * 
                             FROM INFORMATION_SCHEMA.TABLES 
                             WHERE TABLE_NAME = 'PaymentIntents'))
                        BEGIN
                            CREATE TABLE [dbo].[PaymentIntents](
		                        [Id] [int] IDENTITY(1,1) NOT NULL,
		                        [TransactionReference] [nvarchar](max) NOT NULL,
		                        [Description] [nvarchar](max) NULL,
		                        [Amount] [decimal](18, 2) NOT NULL,
                                [Currency] [nvarchar](max) NULL,
		                        [DirectDebitStartDate] [datetime2](7) NULL,
		                        [CustomerEmail] [nvarchar](max) NULL,
		                        [CustomerAddressLines] [nvarchar](max) NULL,
		                        [CustomerCity] [nvarchar](max) NULL,
		                        [CustomerCountry] [nvarchar](max) NULL,
		                        [CustomerPostcode] [nvarchar](max) NULL,
		                        [ConfirmationPageUrl] [nvarchar](max) NULL,
		                        [FailurePageUrl] [nvarchar](max) NULL,
		                        [CallbackUrl] [nvarchar](max) NULL,
		                        [AdditionalData] [nvarchar](max) NULL,
		                        [PaymentIntentType] [nvarchar](max) NULL,
		                        [PaymentStatus] [nvarchar](max) NULL,
		                        [Created] [datetime2](7) NULL,
		                        [ExternalReference] [nvarchar](max) NULL,
		                        [Updated] [datetime2](7) NULL,
		                        [HandShake] [nvarchar](max) NULL,
		                        [CustomerUniqueReference] [nvarchar](max) NULL
	                        ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
                        END

                  /*  IF EXISTS (
                      SELECT * 
                      FROM   sys.columns 
                      WHERE  object_id = OBJECT_ID(N'[dbo].[PaymentIntents]') 
                             AND name = 'DirectDebitFrequencyMonths'
                    )
                        BEGIN
                            ALTER TABLE PaymentIntents
                            DROP COLUMN [DirectDebitFrequencyMonths]
                        END */

                    IF NOT EXISTS (
                      SELECT * 
                      FROM   sys.columns 
                      WHERE  object_id = OBJECT_ID(N'[dbo].[PaymentIntents]') 
                             AND name = 'DirectDebitFrequencyUnit'
                    )
                        BEGIN
                            ALTER TABLE PaymentIntents
                            ADD [DirectDebitFrequencyUnit] INT
                        END

                    IF NOT EXISTS (
                      SELECT * 
                      FROM   sys.columns 
                      WHERE  object_id = OBJECT_ID(N'[dbo].[PaymentIntents]') 
                             AND name = 'DirectDebitFrequencyInterval'
                    )
                        BEGIN
                            ALTER TABLE PaymentIntents
                            ADD [DirectDebitFrequencyInterval] INT
                        END

                    IF NOT EXISTS (
                      SELECT * 
                      FROM   sys.columns 
                      WHERE  object_id = OBJECT_ID(N'[dbo].[PaymentIntents]') 
                             AND name = 'Currency'
                    )
                        BEGIN
                            ALTER TABLE PaymentIntents
                            ADD [Currency] nvarchar(50)
                        END

                    IF NOT EXISTS (
                      SELECT * 
                      FROM   sys.columns 
                      WHERE  object_id = OBJECT_ID(N'[dbo].[PaymentIntents]') 
                             AND name = 'DirectDebitTrialDateEnd'
                    )
                        BEGIN
                            ALTER TABLE PaymentIntents
                            ADD [DirectDebitTrialDateEnd] [datetime2](7) NULL
                        END

                    IF NOT EXISTS (
                      SELECT * 
                      FROM   sys.columns 
                      WHERE  object_id = OBJECT_ID(N'[dbo].[PaymentIntents]') 
                             AND name = 'MotoMode'
                    )
                        BEGIN
                            ALTER TABLE PaymentIntents
                            ADD [MotoMode] [bit] NULL
                        END

                    ");
            }
        }

        public PaymentIntent GetPaymentIntent(int id)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["CashierDb"].ConnectionString).EnsureOpen())
            {
                return connection.ExecuteQuery<PaymentIntent>("SELECT * FROM PaymentIntents WHERE Id = @id", new { id = id }).First();
            }
        }

        public PaymentIntent GetPaymentIntent(string transactionReference)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["CashierDb"].ConnectionString).EnsureOpen())
            {
                return connection.ExecuteQuery<PaymentIntent>("SELECT * FROM PaymentIntents WHERE TransactionReference = @transactionReference", new { transactionReference }).First();
            }
        }

        public PaymentIntent SavePaymentIntent(PaymentIntent paymentIntent)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["CashierDb"].ConnectionString).EnsureOpen())
            {
                if (paymentIntent.Id == 0)
                {
                    paymentIntent.Id = (int)connection.Insert("PaymentIntents", paymentIntent);
                }
                else
                {
                    connection.Update("PaymentIntents", paymentIntent);
                }
            }
                

            return paymentIntent;
        }

        public IEnumerable<PaymentIntent> GetAll(int skip, int take)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["CashierDb"].ConnectionString).EnsureOpen())
            {
                return connection.ExecuteQuery<PaymentIntent>("SELECT * FROM PaymentIntents").OrderByDescending(p => p.Created).Skip(skip).Take(take).ToList();
            }
        }
    }
}
