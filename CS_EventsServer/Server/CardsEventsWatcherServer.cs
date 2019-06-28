using CS_EventsServer.Entities;
using CS_EventsServer.Server.Interfaces;
using System;
using System.Threading;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;

namespace CS_EventsServer.Server {

	public class CardsEventsWatcherServer: ICardsEventsWatcherServer {
		private bool isRunning;
		private readonly Configuration conf;
		private SqlTableDependency<Events55> dependency;

		public CardsEventsWatcherServer() {
			conf = new Configuration();
		}

		public void Start() {
			try {
				isRunning = true;
				Log.Info("Server started!");
				conf.Load();

				var events55Mapper = new ModelToTableMapper<Events55>();
				events55Mapper.AddMapping(model => model.CardNumber, "colEventTime");
				events55Mapper.AddMapping(model => model.CardNumber, "colCardNumber");
				events55Mapper.AddMapping(model => model.colHolderID, "colHolderID");
				events55Mapper.AddMapping(model => model.Direction, "colDirection");

				dependency = new SqlTableDependency<Events55>(conf.ConnectionString, "tblEvents_55", "", events55Mapper);
				dependency.OnChanged += onEvent55Changed;
				dependency.Start();

				while(isRunning) {
					Thread.Sleep(1000);
				}
			} catch(Exception e) {
				Log.Warn(e.Message + e.StackTrace);
			}
		}

		public void Stop() {
			Log.Trace("Server has been stopped!");
			isRunning = false;
		}

		private void onEvent55Changed(object sender, RecordChangedEventArgs<Events55> e) {
			if(e.ChangeType == ChangeType.Insert) {
				Log.Info($"Inserted Event with CardNumber: {e.Entity.CardNumber}");
			}
		}
	}
}