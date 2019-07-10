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

	public class CardsEventsWatcherServer: ICardsEventsWatcherServer {
		private bool isRunning;
		private IUnitOfWork unitOfWork;
		private readonly Configuration conf;
		private static readonly HttpClient httpClient = new HttpClient();

		private DateTime lastNotifiedDateTime;

		public CardsEventsWatcherServer() {
			conf = new Configuration();
		}

		public void Start() {
			try {
				isRunning = true;
				conf.Load();

				Log.Info("ConnectionString: " + conf.ConnectionString);
				unitOfWork = new EFUnitOfWork(conf.ConnectionString);

				Log.Info("Trying to get last event from dbo.Event_55");
				lastNotifiedDateTime = getLastDateTime();

				setupNetConfig();
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
							foreach(var endPoint in conf.ClientUrls) {
								var command = new RequestPushEvent(
									new EventDTO() {
										CardNumber = event55.CardNumber,
										EventTime = event55.EventTime
									});

								Log.Trace(JsonConvert.SerializeObject(command, Formatting.Indented));

								string event55Json = JsonConvert.ToString(JsonConvert.SerializeObject(event55, Formatting.Indented));
								Log.Debug(event55Json);
								string eventJson = @"{""text"":" + event55Json + "}";

								notifierTasks.Add(
									httpClient.PostAsync(
										endPoint,
										new StringContent(eventJson, System.Text.Encoding.UTF8, "application/json")));
							}

							if(!isRunning)
								break;
						}

						Task.WaitAll(notifierTasks.ToArray());


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

		private void setupNetConfig() {
			ServicePointManager.DefaultConnectionLimit = 20;
			foreach(var endPoint in conf.ClientUrls) {
				var habrServicePoint = ServicePointManager.FindServicePoint(endPoint);
				habrServicePoint.MaxIdleTime = 100000;
				habrServicePoint.ConnectionLeaseTimeout = 60000;
			}
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
					unitOfWork.Dispose();
					httpClient.Dispose();
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