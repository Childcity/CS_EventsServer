using System;

namespace CS_EventsServer.Server.Interfaces {
	interface ICardsEventsWatcherServer: IDisposable {
		void Start();
		void Stop();
	}
}
