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
		private readonly CancellationToken cancellationToken;
		private EntitieWatcher<Event55> event55Wtch;
		private EventService eventService;

		private ChangedEventHandler<Event55> onChangedEvent55EventHandler;

		static EventsWatcher() {
			comunicator = new ComunicationClient();
		}

		public EventsWatcher(AConfiguration conf, CancellationToken token) {
			cancellationToken = token;

			// setup comunication with bot servers
			comunicator.ServersUrls = conf.ServersUrls;
			comunicator.ConnectSubscribers();

			eventService = new EventService(conf.ConnectionString, cancellationToken);

			event55Wtch = new EntitieWatcher<Event55>(conf.ConnectionString, filter: event55 => event55.EventCode == 105);
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

					await comunicator.NotifyAll(new RequestPushEvent(eventDTO), cancellationToken);
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
						eventService = null;
						onChangedEvent55EventHandler = null;
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