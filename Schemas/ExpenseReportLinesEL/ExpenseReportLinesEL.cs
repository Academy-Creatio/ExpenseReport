using Common.Logging;
using ForeignExchange;
using System;
using System.Threading.Tasks;
using Terrasoft.Core;
using Terrasoft.Core.Entities;
using Terrasoft.Core.Entities.Events;


namespace ExpenseReportStart
{
    [EntityEventListener(SchemaName = "ExpenseReportLines")]
    class ExpenseReportLinesEL : BaseEntityEventListener
    {
        //private UserConnection UserConnection;
        private static readonly ILog _log = LogManager.GetLogger("TrainingLogger");

        public override void OnSaving(object sender, EntityBeforeEventArgs e)
        {
            base.OnSaving(sender, e);
            Entity entity = (Entity)sender;
            UserConnection UserConnection = entity.UserConnection;

            var descOld = entity.GetTypedOldColumnValue<string>("Description");
            var descNew = entity.GetTypedColumnValue<string>("Description");
            _log.Info($"Old Value:{descOld}\tNew Value:{descNew}");
        }

        public override void OnSaved(object sender, EntityAfterEventArgs e)
        {

            base.OnSaved(sender, e);
            Entity entity = (Entity)sender;
            //UserConnection = entity.UserConnection;

            var amountFC = entity.GetTypedColumnValue<decimal>("AmountFC");
            var date = entity.GetTypedColumnValue<DateTime>("TransactionDate");
            var currencyId = entity.GetTypedColumnValue<Guid>("CurrencyId");
            string shortCurrency = entity.FindValueById<string>("Currency", currencyId, "ShortName");

            IBank ibank = BankFactory.GetBank(BankFactory.SupportedBanks.BOC);
            IBankResult result = null;

            Task.Run(async () =>
            {
                result = await ibank.GetRateAsync(shortCurrency, date);
            }).Wait();


            entity.SetColumnValue("FxRate", result.ExchangeRate);
            entity.SetColumnValue("AmountHC", amountFC* result.ExchangeRate);
            entity.Save();
            _log.Info($"rate:{result.ExchangeRate} on {result.RateDate:dd-MMM-yyyy}");
        }
    }
}
        