using ForeignExchange;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using Terrasoft.Core;
using Terrasoft.Core.DB;
using Terrasoft.Web.Common;
namespace ExpenseReportStart
{
	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class PrevalidateExpenseReportLine : BaseService
	{
		
		private SystemUserConnection _systemUserConnection;
		private SystemUserConnection SystemUserConnection
		{
			get
			{
				return _systemUserConnection ?? (_systemUserConnection = (SystemUserConnection)AppConnection.SystemUserConnection);
			}
		}
		private UserConnection userConnection {get; set;}

		[OperationContract]
		[WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		public BankResult Validate(PrevalidateExpenseReportLineDataContract request){
			//Call: [APP]/0/rest/PrevalidateExpenseReportLine/Validate
			userConnection = UserConnection ?? SystemUserConnection;
			
			IBank Bank = BankFactory.GetBank(BankFactory.SupportedBanks.BOC);
			BankResult result = new BankResult();
			string shortCurrency = FindValueById<string>("Currency", request.CurrencyId, "ShortName");
			
			System.Threading.Tasks.Task.Run(async () =>
			{
				IBankResult response = await Bank.GetRateAsync(shortCurrency, request.TransactionDate);
				result = (BankResult)response;
			}).Wait();
			return result;
		}
		
		
		private T FindValueById<T>(string RootSchemName, Guid Id, string SearchColumn)
		{
			Select select = new Select(userConnection)
				.Top(1)
				.Column(SearchColumn)
				.From(RootSchemName)
				.Where("Id").IsEqual(Column.Parameter(Id)) as Select;
			var result = select.ExecuteScalar<T>();
			return result;
		}
		
	}

	
	[Serializable]
	[DataContract]
	public class PrevalidateExpenseReportLineDataContract
	{
		[DataMember(Name = "TransactionDate")]
		public DateTime TransactionDate { get; set; }

		[DataMember(Name = "AmountFC")]
		public decimal AmountFC { get; set; }
		
		[DataMember(Name = "CurrencyId")]
		public Guid CurrencyId { get; set; }
	}
	
}