using CS_EventsServer.Server.Comunication;
using CS_EventsServer.Server.Comunication.Commands;
using CS_EventsServer.Server.DAL;
using CS_EventsServer.Server.DAL.Entities;
using CS_EventsServer.Server.DTO;
using CS_EventsServer.Server.Interfaces;
using System;
using System.Threading;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;

namespace CS_EventsServer.Server.Services {

	public class EventsWatcher: IDisposable {
		private static readonly ComunicationClient comunicator;
		private EntitieWatcher<Event55> event55Wtch;

		private DateTime lastNotifiedDateTime;

		static EventsWatcher() {
			comunicator = new ComunicationClient();
		}

		public EventsWatcher(AConfiguration conf) {
			// setup comunication with bot servers
			comunicator.ServersUrls = conf.ServersUrls;
			comunicator.ConnectSubscribers();

			event55Wtch = new EntitieWatcher<Event55>(conf.ConnectionString, null);
			event55Wtch.Dependancy.OnChanged += onChanged;
			event55Wtch.Dependancy.OnStatusChanged += onStatusChanged;
			event55Wtch.Dependancy.OnError += onError;

			event55Wtch.Start();
		}

		private void onError(object sender, ErrorEventArgs e) {
			Log.Trace("SqlTableDependency Error: " + e.Error?.Message);
		}

		private void onStatusChanged(object sender, StatusChangedEventArgs e) {
			Log.Trace("SqlTableDependency Status: " + e.Status.ToString());
		}

		private void onChanged(object sender, object evArgs) {
			try {
				Log.Trace("onChanged");

				if(evArgs is RecordChangedEventArgs<Event55>) {
					var concreteEvArgs = evArgs as RecordChangedEventArgs<Event55>;

					if(concreteEvArgs.ChangeType == ChangeType.None)
						return;

					var event55 = concreteEvArgs.Entity;

					comunicator.NotifyAll(new RequestPushEvent(new EventDTO() {
						EventNumber = event55.EventNumber,
						EventTime = event55.EventTime
					}), CancellationToken.None);
				}
			} catch(Exception e) {
				Log.Trace(e.Message + "\n" + e.StackTrace);
			}
		}


		#region IDisposable Support

		private bool disposedValue = false;

		protected virtual void Dispose(bool disposing) {
			if(!disposedValue) {
				if(disposing) {
					try { } finally {
						comunicator?.Dispose();

						if(event55Wtch != null && event55Wtch.Dependancy != null) {
							event55Wtch.Dependancy.OnChanged -= onChanged;
							event55Wtch.Dependancy.OnStatusChanged -= onStatusChanged;
							event55Wtch.Dependancy.OnError -= onError;
							event55Wtch.Dispose();
							event55Wtch = null;
						}
					}
				}
				Log.Trace($"Disposed: {ToString()}");
				disposedValue = true;
			}
		}

		public void Dispose() {
			Dispose(true);
		}

		#endregion IDisposable Support
	}
}