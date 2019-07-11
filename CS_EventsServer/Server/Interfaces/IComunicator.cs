using CS_EventsServer.Server.Comunication.Commands;
using System;

namespace CS_EventsServer.Server.Interfaces {
	public interface IComunicator: IDisposable {
		void ConnectSubscribers();
		void NotifyAll(CommandBase command);
	}
}
