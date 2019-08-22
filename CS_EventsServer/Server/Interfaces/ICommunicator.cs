using CS_EventsServer.Server.Comunication.Commands;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CS_EventsServer.Server.Interfaces {

	public interface ICommunicator: IDisposable {

		// server urls to connect
		List<Uri> ServersUrls { get; set; }

		// on request from server
		event EventHandler<CommandBase> OnRequest;

		// connect servers, which integrated
		void ConnectSubscribers();

		// send command to all subscribers
		Task NotifyAll(CommandBase command, CancellationToken cancellationToken);
	}
}