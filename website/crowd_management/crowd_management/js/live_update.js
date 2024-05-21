/*
 * File: live_update.js
 * Author: Warre Willeme & Jesse UijtdeHaag
 * Date: May 12, 2024
 * Description: This file contains the JavaScript code for the live update of the heatmap on the crowd management page.
 */

$(function () {
	var chat = window.$.connection.liveUpdateHub;

	chat.client.broadcastMessage = function (name, message) {
		if (name === "HeatMap") {
			var data = JSON.parse(message);

			var colorClasses = ["is-success", "is-warning", "is-danger"]; // Add all possible color classes

			for (var id in data) {
				if (Object.prototype.hasOwnProperty.call(data, id)) {
					window.$("#tagZoneName" + id).text(data[id].Name);
					if (data[id].Percentage === -1) {
						window.$("#tagZoneColor" + id).removeClass(colorClasses.join(" ")).addClass('is-' + data[id].Color);
					} else {
						window.$("#tagZonePercentage" + id).text(data[id].Percentage + "%");
					}

					if (data[id].Lockdown) {
						window.$("#zoneLockdown" + id).removeClass('fa-lock-open').addClass('fa-lock').show();
					} else {
						window.$("#zoneLockdown" + id).removeClass('fa-lock').addClass('fa-lock-open').show();
					}
				}
			};
		};
	};

	window.$.connection.hub.start().done(function () {
	});
});