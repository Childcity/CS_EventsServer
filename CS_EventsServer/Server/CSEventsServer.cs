using CS_EventsServer.Server.Services;
using System;
using System.Threading;

namespace CS_EventsServer.Server {

	public class CSEventsServer: IDisposable {
		private static readonly ServerConfiguration conf;
		private EventsWatcher eventsWatcher;

		static CSEventsServer() {
			conf = new ServerConfiguration();
		}

		public void Start(CancellationToken cancellationToken) {
			try {
				conf.Load();

				Log.Info("ConnectionString: " + conf.ConnectionString);
				Log.Info("ServersUrls Count: " + conf.ServersUrls.Count);

				eventsWatcher = new EventsWatcher(conf, cancellationToken);
			} catch (Exception e) {
				Log.Fatal("Error ocure, while server starting!\n" + e.ToString());
				return;
			}

			Log.Info("Server started!");
			cancellationToken.WaitHandle.WaitOne();
			Log.Info("Server has been stopped!");
		}

		#region IDisposable Support

		private bool disposedValue = false;

		protected virtual void Dispose(bool disposing) {
			if (!disposedValue) {
				if (disposing) {
					eventsWatcher?.Dispose();
					eventsWatcher = null;
				}
				Log.Trace($"Disposed: {ToString()}\n\n");
				disposedValue = true;
			}
		}

		public void Dispose() {
			Dispose(true);
		}

		#endregion IDisposable Support
	}
}