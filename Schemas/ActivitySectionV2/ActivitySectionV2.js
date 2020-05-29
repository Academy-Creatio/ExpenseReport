define("ActivitySectionV2", [],
	function() {
		return {
			entitySchemaName: "Activity",
			methods: {
				init: function(callback, scope) {
					this.callParent(arguments);
					Terrasoft.ServerChannel.on(Terrasoft.EventName.ON_MESSAGE, this.onMessageReceived, this);
				},
				onMessageReceived: function(scope, message) {
					if (!message || message.Header.Sender !== "ActivityListener") {
						return;
					}
					this.showInformationDialog(message.Body);
				}
				
			},
		};
	}
);
