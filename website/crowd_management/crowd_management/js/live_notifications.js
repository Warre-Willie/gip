$(function () {
	console.log($.connection);
	var chat = $.connection.chatHub;

	chat.client.broadcastMessage = function (name, message) {
		console.log(name, message);
	};

	$.connection.hub.start().done(function () {
		$('#send').click(function () {
			chat.server.send($('#name').val(), $('#message').val());
			$('#message').val('').focus();
		});
	});
});