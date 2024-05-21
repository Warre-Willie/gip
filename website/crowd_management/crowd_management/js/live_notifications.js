$(function () {
	const messageContainer = document.createElement('div');
	messageContainer.classList.add('message-container');
	document.body.appendChild(messageContainer);

	var chat = window.$.connection.liveUpdateHub;

	chat.client.broadcastMessage = function (name, message) {
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
		}
	};

	window.$.connection.hub.start().done(function () {
	});
});