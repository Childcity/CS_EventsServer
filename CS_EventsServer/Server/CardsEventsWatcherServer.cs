using CS_EventsServer.Entities;
using CS_EventsServer.Server.Interfaces;
using System;
using System.Data.SqlClient;
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
		private readonly Configuration conf;
		private SqlTableDependency<Events55> dependency;

		public CardsEventsWatcherServer() {
			conf = new Configuration();
		}

		public void Start() {
			try {
				isRunning = true;
				conf.Load();

				Log.Info("ConnectionString: " + conf.ConnectionString);

				try {
					dependency = new SqlTableDependency<Events55>(conf.ConnectionString, "tblEvents_55", "dbo", Events55Mapper.Get());
				} catch(ServiceBrokerNotEnabledException) {
					Log.Warn($"Service broker not enable. Trying to activate it by executing 'ALTER DATABASE [{dbName}] SET ENABLE_BROKER'");
					using(SqlConnection conn = new SqlConnection(conf.ConnectionString)) {
						string enableBrokerCmd = $"ALTER DATABASE [{dbName}] SET ENABLE_BROKER;";
						using(SqlCommand cmd = new SqlCommand(enableBrokerCmd, conn)) {
							conn.Open();
							cmd.ExecuteNonQuery();
							conn.Close();
						}
					}
					Log.Info("Please, restart server!");
					Stop();
					return;
				}

				dependency.OnChanged += onEvent55Changed;
				dependency.Start();

				Log.Info("Server started!");

				while(isRunning) {
					Thread.Sleep(1000);
				}
			} catch(Exception e) {
				Log.Fatal("Error ocure, while server starting!\n" + e.ToString());
				dependency?.Stop();
			}
		}

		public void Stop() {
			Log.Info("Server has been stopped!");
			isRunning = false;
			dependency?.Stop();
		}

		private void onEvent55Changed(object sender, RecordChangedEventArgs<Events55> e) {
			Log.Trace($"Inserted Event with CardNumber: {e.ToString()}");
			if(e.ChangeType == ChangeType.Insert) {
				Log.Trace($"Inserted Event with CardNumber: {e.Entity.CardNumber}");
			}else if(e.ChangeType == ChangeType.Update) {
				Log.Trace($"Update Event with CardNumber: {e.Entity.CardNumber}");
			}
		}
	}
}