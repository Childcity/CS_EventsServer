using CS_EventsServer.Server.DAL.Entities;
using CS_EventsServer.Server.DAL.Interfaces;
using CS_EventsServer.Server.Interfaces;
using NLayerApp.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;
using TableDependency.SqlClient.Exceptions;

namespace CS_EventsServer.Server {

	public class CardsEventsWatcherServer: ICardsEventsWatcherServer {
		private static readonly string dbName = "StopNet4";
		private bool isRunning;
		private IUnitOfWork unitOfWork;
		private readonly Configuration conf;

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

				lastNotifiedDateTime = getLastDateTime();

				Log.Info("Server started!");

				while(isRunning) {

					var lastDateTime = getLastDateTime();

					Log.Trace("lastNotifiedDateTime" + lastNotifiedDateTime.ToString());
					Log.Trace("lastDateTime" + lastDateTime.ToString());

					if(lastNotifiedDateTime < lastDateTime) {
						foreach(var event55 in getEvents(lastNotifiedDateTime, lastDateTime)) {
							Log.Trace($"Should notify: {event55.CardNumber.ToString()}");
						}
					}

					Thread.Sleep(4000);
				}
			} catch(Exception e) {
				Log.Fatal("Error ocure, while server starting!\n" + e.ToString());
			}
		}

		private List<Event55> getEvents(DateTime from, DateTime to) {
			var events = new List<Event55>(0);
			try {
				using(SqlConnection conn = new SqlConnection(conf.ConnectionString)) {
					SqlCommand command = new SqlCommand($"SELECT * FROM [StopNet4].[dbo].[tblEvents_55] WHERE colEventTime > '{from.ToString("yyyy-MM-ddTHH:mm:ss.fff")}' ORDER BY [colEventTime] ASC;", conn);
					Log.Trace(command.CommandText);
					conn.Open();
					using(SqlDataReader reader = command.ExecuteReader()) {
						if(reader.HasRows) {
							while(reader.Read()) {
								Event55 event55 = new Event55 {
									EventNumber = reader.GetSqlDecimal(reader.GetOrdinal("colEventNumber")).Value,
									EventCode = reader.GetSqlInt32(reader.GetOrdinal("colEventCode")).Value,
									//HolderID = reader.GetSqlInt32(reader.GetOrdinal("colHolderID")).Value,
									//Direction = reader.GetSqlInt32(reader.GetOrdinal("colDirection")).Value,
									CardNumber = reader.GetSqlDecimal(reader.GetOrdinal("colCardNumber")).Value,
									EventTime = reader.GetSqlDateTime(reader.GetOrdinal("colEventTime")).Value
								};
							}
						}
					}
				}
			} catch(Exception e) {
				Log.Warn("Cannot select last 'colEventTime' of Event\n" + e.ToString());
			}

			return events;
		}

		private DateTime getLastDateTime() {
			return unitOfWork.Events55.GetAll().OrderBy(item => item.EventTime).FirstOrDefault().EventTime;
		}

		public void Stop() {
			Log.Info("Server has been stopped!");
			isRunning = false;
		}
	
	}
}