using System;

namespace CS_EventsServer.Server.Interfaces {
	interface IWatcherService: IDisposable {
		void Start();
		void Stop();
	}
}
