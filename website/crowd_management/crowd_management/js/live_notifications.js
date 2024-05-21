$(function () {
	var chat = window.$.connection.liveUpdateHub;

	chat.client.broadcastMessage = function (name, message) {
		console.log(name, message);
	};

	window.$.connection.hub.start().done(function () {
		window.$('#send').click(function () {
			chat.server.send(window.$('#name').val(), window.$('#message').val());
			window.$('#message').val('').focus();
		});
	});
});