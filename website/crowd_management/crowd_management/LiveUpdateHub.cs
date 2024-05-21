using Microsoft.AspNet.SignalR;

namespace crowd_management;

public class LiveUpdateHub : Hub
{
	// ReSharper disable once UnusedMember.Global
	public void Send(string name, string message)
	{
		// Call the broadcastMessage method to update clients.
		Clients.All.broadcastMessage(name, message);
	}
}