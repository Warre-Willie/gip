/*
 * File: live_update.js
 * Author: Warre Willeme & Jesse UijtdeHaag
 * Date: May 12, 2024
 * Description: This file contains the JavaScript code for the live update of the heatmap on the crowd management page.
 */

$(function () {
	var chat = window.$.connection.liveUpdateHub;

	const messageContainer = document.createElement('div');
	messageContainer.classList.add('message-container');
	document.body.appendChild(messageContainer);

	chat.client.broadcastMessage = function (name, message) {
		console.log(name, message);
		if (name === "HeatMap") {
			var data = JSON.parse(message);

			var colorClasses = ["is-success", "is-warning", "is-danger"]; // Add all possible color classes

			for (var id in data) {
				if (Object.prototype.hasOwnProperty.call(data, id)) {
					window.$("#tagZoneName" + id).text(data[id].Name);
					if (data[id].Percentage !== -1) {
						window.$("#tagZoneColor" + id).removeClass(colorClasses.join(" ")).addClass('is-' + data[id].Color);
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
		if (name === "Notification") {
			const messageElement = document.createElement('article');
			messageElement.classList.add('message', 'is-danger');

			const messageBody = document.createElement('div');
			messageBody.classList.add('message-body');
			messageBody.innerHTML = message;

			const progressBar = document.createElement('div');
			progressBar.classList.add('progress-bar');
			const progressBarInner = document.createElement('div');
			progressBarInner.classList.add('progress-bar-inner');

			progressBar.appendChild(progressBarInner);
			messageElement.appendChild(messageBody);
			messageElement.appendChild(progressBar);
			messageContainer.appendChild(messageElement);
			messageElement.getBoundingClientRect();

			messageElement.classList.add('show');

			progressBarInner.style.animationDuration = 5000 + 'ms';

			// Hide the message after the specified duration
			setTimeout(() => {
				messageElement.classList.remove('show');
				messageElement.addEventListener('transitionend', () => {
					messageContainer.removeChild(messageElement);
				});
			}, 5000);
		};
	};

	window.$.connection.hub.start().done(function () {
	});
});