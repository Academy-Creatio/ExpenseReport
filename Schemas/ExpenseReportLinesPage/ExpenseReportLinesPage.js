define("ExpenseReportLinesPage", ["ServiceHelper"], function(ServiceHelper) {
	return {
		entitySchemaName: "ExpenseReportLines",
		attributes: {
			AmountFC:{
				dependencies: [
					{
						columns: ["AmountFC","Currency"],
						methodName: "recalculateHC"
					}
				]
			},
			
			DescriptionRequired:{
				"dataValueType": this.Terrasoft.DataValueType.BOOLEAN,
				value: false,
				dependencies: [
					{
						columns: ["AmountHC"],
						methodName: "IsDescriptionRequired"
					}
				]
			}
			
			
			
		},
		modules: /**SCHEMA_MODULES*/{}/**SCHEMA_MODULES*/,
		details: /**SCHEMA_DETAILS*/{}/**SCHEMA_DETAILS*/,
		businessRules: /**SCHEMA_BUSINESS_RULES*/{}/**SCHEMA_BUSINESS_RULES*/,
		methods: {
			onEntityInitialized: function(){
				this.callParent(arguments);
				this.IsDescriptionRequired();
			},
			IsDescriptionRequired: function(){
				var amountHC = this.$AmountHC;
				
				if(amountHC >=100){
					this.set("DescriptionRequired", true);
				}else{
					this.set("DescriptionRequired", false);
				}
			},
			recalculateHC: function(){
				var serviceData = {
					"request": {
						"TransactionDate": "\/Date("+this.$TransactionDate.getTime()+")\/",
						"AmountFC": this.$AmountFC,
						"CurrencyId": this.$Currency.value
					}
				};
				
				ServiceHelper.callService("PrevalidateExpenseReportLine", "Validate", function(response){
					if(response && response.ExchangeRate !== -1){
						var fxRate = response.ExchangeRate;
						var amountHC = fxRate * this.$AmountFC;
						
						this.set("FxRate", fxRate);
						this.set("AmountHC", amountHC);
					}
				}, serviceData, this);
			},
			
			asyncValidate: function(callback, scope) {
				this.callParent([function(response) {
					var checkResponse = function(context) {
						if (!context.response.success) {
							context.callback.call(context.scope, context.response);
						} else {
							context.next();
						}
					};
					var validationChain = [
						checkResponse,
						function(context) {
							context.scope.validateDescription(function(response) {
								context.response = response;
								context.next();
							}, context.scope);
						},
						checkResponse,
						function(context) {
							context.scope.validateDescriptionLength(function(response) {
								context.response = response;
								context.next();
							}, context.scope);
						},
						function(context) {
							context.callback.call(context.scope, context.response);
						}
					];
					Terrasoft.chain({
						scope: scope || this,
						response: response,
						callback: callback
					}, validationChain);
				}, this]);
			},
			
			
			validateDescription: function(callback, scope) {
				var result = {
					success: true
				};
				var amountHC = this.$AmountHC;
				var description = this.$Description;
				if (amountHC > 100 && this.Ext.isEmpty(description)) {
					result.message = "When amount exceeds 100, description must be filled in";
					result.success = false;
				}
				callback.call(scope || this, result);
			},
			validateDescriptionLength:function(callback, scope){
				var result = {
					success: true
				};

				var description = this.$Description;
				if(description.length<= 10){
					result.message = "Description must be at least 10 characters long";
					result.success = false;
				}
				
				callback.call(scope || this, result);
			},
			
			
			
			
			
		},
		dataModels: /**SCHEMA_DATA_MODELS*/{}/**SCHEMA_DATA_MODELS*/,
		diff: /**SCHEMA_DIFF*/[
			{
				"operation": "insert",
				"name": "AmountFC",
				"values": {
					"layout": {
						"colSpan": 8,
						"rowSpan": 1,
						"column": 8,
						"row": 0,
						"layoutName": "Header"
					},
					"bindTo": "AmountFC"
				},
				"parentName": "Header",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "AmountHC",
				"values": {
					"layout": {
						"colSpan": 8,
						"rowSpan": 1,
						"column": 0,
						"row": 3,
						"layoutName": "Header"
					},
					"bindTo": "AmountHC",
					"enabled": false
				},
				"parentName": "Header",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "Currency",
				"values": {
					"layout": {
						"colSpan": 8,
						"rowSpan": 1,
						"column": 16,
						"row": 0,
						"layoutName": "Header"
					},
					"bindTo": "Currency",
					"enabled": true,
					"contentType": 5
				},
				"parentName": "Header",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "Description",
				"values": {
					"layout": {
						"colSpan": 24,
						"rowSpan": 1,
						"column": 0,
						"row": 1,
						"layoutName": "Header"
					},
					"bindTo": "Description",
					"enabled": {"bindTo": "DescriptionRequired"},
					"isRequired": {"bindTo": "DescriptionRequired"},
					"visible": {"bindTo": "DescriptionRequired"},
					"contentType": 0
				},
				"parentName": "Header",
				"propertyName": "items",
				"index": 3
			},
			{
				"operation": "insert",
				"name": "FxRate",
				"values": {
					"layout": {
						"colSpan": 8,
						"rowSpan": 1,
						"column": 16,
						"row": 3,
						"layoutName": "Header"
					},
					"bindTo": "FxRate",
					"enabled": false
				},
				"parentName": "Header",
				"propertyName": "items",
				"index": 4
			},
			{
				"operation": "insert",
				"name": "TransactionDate",
				"values": {
					"layout": {
						"colSpan": 8,
						"rowSpan": 1,
						"column": 0,
						"row": 0,
						"layoutName": "Header"
					},
					"bindTo": "TransactionDate"
				},
				"parentName": "Header",
				"propertyName": "items",
				"index": 5
			}
		]/**SCHEMA_DIFF*/
	};
});
