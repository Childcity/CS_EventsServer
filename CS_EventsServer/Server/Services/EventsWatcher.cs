using CS_EventsServer.Server.BLL.Services;
using CS_EventsServer.Server.Comunication;
using CS_EventsServer.Server.Comunication.Commands;
using CS_EventsServer.Server.DAL;
using CS_EventsServer.Server.DAL.Entities;
using CS_EventsServer.Server.DTO;
using CS_EventsServer.Server.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using TableDependency.SqlClient.Base.Delegates;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;

namespace CS_EventsServer.Server.Services {

	public class EventsWatcher: IDisposable {
		private static readonly ComunicationClient comunicator;
		private CancellationTokenSource tokenSource;
		private EntitieWatcher<Event55> event55Wtch;
		private EventService eventService;

		private ChangedEventHandler<Event55> onChangedEvent55EventHandler;

		static EventsWatcher() {
			comunicator = new ComunicationClient();
		}

		public EventsWatcher(AConfiguration conf) {
			tokenSource = new CancellationTokenSource();
			eventService = new EventService(conf.ConnectionString, tokenSource.Token);

			// setup comunication with bot servers
			comunicator.ServersUrls = conf.ServersUrls;
			comunicator.ConnectSubscribers();

			event55Wtch = new EntitieWatcher<Event55>(conf.ConnectionString, null);
			onChangedEvent55EventHandler = new ChangedEventHandler<Event55>(async (s, e) => await onChanged(s, e));

			event55Wtch.Dependancy.OnChanged += onChangedEvent55EventHandler;
			event55Wtch.Dependancy.OnStatusChanged += onStatusChanged;
			event55Wtch.Dependancy.OnError += onError;

			event55Wtch.Start();
		}

		private void onError(object sender, ErrorEventArgs e) {
			Log.Debug("SqlTableDependency onError: " + e.Error?.Message);
		}

		private void onStatusChanged(object sender, StatusChangedEventArgs e) {
			Log.Debug("SqlTableDependency onStatusChanged: " + e.Status.ToString());
		}

		private async Task onChanged(object sender, object evArgs) {
			try { 
				Log.Debug("SqlTableDependency onChanged");

				if(evArgs is RecordChangedEventArgs<Event55>) {
					var concreteEvArgs = evArgs as RecordChangedEventArgs<Event55>;

					if(concreteEvArgs.ChangeType == ChangeType.None)
						return;

					var event55 = concreteEvArgs.Entity;
					EventDTO eventDTO = await eventService.GetEventInfo(event55);

					await comunicator.NotifyAll(new RequestPushEvent(eventDTO), tokenSource.Token);
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
					try {
						tokenSource?.Cancel();
					} finally {
						comunicator?.Dispose();

						if(event55Wtch != null && event55Wtch.Dependancy != null) {
							event55Wtch.Dependancy.OnChanged -= onChangedEvent55EventHandler;
							event55Wtch.Dependancy.OnStatusChanged -= onStatusChanged;
							event55Wtch.Dependancy.OnError -= onError;
							event55Wtch.Dispose();
							event55Wtch = null;
						}

						eventService?.Dispose();
						tokenSource?.Dispose(); // should be disposed last!
						eventService = null;
						onChangedEvent55EventHandler = null;
						tokenSource = null;
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