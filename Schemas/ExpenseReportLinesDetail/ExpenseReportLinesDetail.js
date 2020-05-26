define("ExpenseReportLinesDetail", [], function() {
	return {
		entitySchemaName: "ExpenseReportLines",
		messages: {
        	//Subscribe on ExpenseReportPage
        	"somethingChanged": {
        		mode: this.Terrasoft.MessageMode.PTP,
        		direction: this.Terrasoft.MessageDirectionType.PUBLISH
        	}
        },
		details: /**SCHEMA_DETAILS*/{}/**SCHEMA_DETAILS*/,
		diff: /**SCHEMA_DIFF*/[]/**SCHEMA_DIFF*/,
		methods: {
			onDataChanged: function(){
                this.callParent(arguments);
                this.sandbox.publish("somethingChanged", "message body", ["THIS_IS_MY_TAG"]);
            }
		}
	};
});
