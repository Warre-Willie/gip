using Owin;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(crowd_management.Startup))]

namespace crowd_management;

public class Startup
{
	// ReSharper disable once UnusedMember.Global
	public void Configuration(IAppBuilder app)
	{
		// Any connection or hub wire up and configuration should go here
		app.MapSignalR();
	}
}