using CS_EventsServer.Server.Comunication.Commands;
using System;

namespace CS_EventsServer.Server.Interfaces {
	public interface ICommunicator: IDisposable {
		void ConnectSubscribers();
		void NotifyAll(CommandBase command);
	}
}
