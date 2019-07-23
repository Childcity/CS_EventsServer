using CS_EventsServer.Server.Comunication;
using CS_EventsServer.Server.Comunication.Commands;
using CS_EventsServer.Server.DAL.Entities;
using CS_EventsServer.Server.DAL.Interfaces;
using CS_EventsServer.Server.DAL.Repositories;
using CS_EventsServer.Server.DTO;
using CS_EventsServer.Server.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CS_EventsServer.Server {

	public class EventsWatcher: IWatcherService {
		private bool isRunning;
		private IUnitOfWork unitOfWork;
		private readonly Configuration conf;
		private static readonly ComunicationClient comunicator;

		private DateTime lastNotifiedDateTime;

		static EventsWatcher() {
			comunicator = new ComunicationClient();
		}

		public EventsWatcher() {
			conf = new Configuration();
		}

		public void Start() {
			try {
				isRunning = true;
				conf.Load();

				// setup comunication with bot servers
				comunicator.ServersUrls = conf.ServersUrls;
				comunicator.ConnectSubscribers();

				Log.Info("ConnectionString: " + conf.ConnectionString);
				unitOfWork = new EFUnitOfWork(conf.ConnectionString);

				Log.Info("Trying to get last event from dbo.Event_55");
				lastNotifiedDateTime = getLastDateTime();
				
			} catch(Exception e) {
				Log.Fatal("Error ocure, while server starting!\n" + e.ToString());
				Stop();
				return;
			}

			Log.Info("Server started!");

			doWatching();
		}

		private void doWatching() {
			var notifierTasks = new List<Task>();

			while(isRunning) {
				try {
					var lastDateTime = getLastDateTime();

					//Log.Debug("lastNotifiedDateTime: " + lastNotifiedDateTime.ToString());
					//Log.Debug("lastDateTime:         " + lastDateTime.ToString());

					if(lastNotifiedDateTime < lastDateTime) {
						notifierTasks.Clear();

						// for each new event we create new request
						foreach(var event55 in getEvents(lastNotifiedDateTime, lastDateTime)) {
							Log.Debug($"Should notify: {event55.EventNumber.ToString()}");

							// for each client endpoint we create new notification request
							// and add it to notifierTasks List
							foreach(var endPoint in conf.ServersUrls) {
								var command = new RequestPushEvent(
									new EventDTO() {
										CardNumber = event55.CardNumber,
										EventTime = event55.EventTime
									});

								comunicator.NotifyAll(command);
							}

							if(!isRunning)
								break;
						}

						lastNotifiedDateTime = lastDateTime;
					}
				} catch(Exception e) {
					Log.Warn("Error ocure, while watching events\n" + e.ToString());
				}

				Thread.Sleep(500);
			}
		}

		public void Stop() {
			Log.Info("Server has been stopped!");
			isRunning = false;
		}

		private List<Event55> getEvents(DateTime from, DateTime to) {
			return unitOfWork.Events55.GetAll(true)
				.Where(item => item.EventTime > from && item.EventTime <= to)
				.ToList();
		}

		private DateTime getLastDateTime() {
			return unitOfWork.Events55.GetAll(true)
				.Max(item => item.EventTime);
		}

		#region IDisposable Support

		private bool disposedValue = false;

		protected virtual void Dispose(bool disposing) {
			if(!disposedValue) {
				if(disposing) {
					unitOfWork?.Dispose();
					comunicator?.Dispose();
				}

				disposedValue = true;
			}
		}

		public void Dispose() {
			Dispose(true);
		}

		#endregion IDisposable Support
	}
}