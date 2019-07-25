using CS_EventsServer.Server.Comunication.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CS_EventsServer.Server.Interfaces {

	public interface ICommunicator: IDisposable {

		void ConnectSubscribers();

		Task NotifyAll(CommandBase command, CancellationToken cancellationToken);
	}
}