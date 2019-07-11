using CS_EventsServer.Server;
using CS_EventsServer.Server.Interfaces;
using NLog;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.ServiceProcess;
using System.Threading;

namespace CS_EventsServer {

	public partial class CardsEventsWatcherSvc: ServiceBase {
		private ICardsEventsWatcherServer server;
		private Thread serverThread;

		public CardsEventsWatcherSvc() {
			InitializeComponent();
			CanStop = true;
			CanPauseAndContinue = true;
			AutoLog = false;
		}

		protected override void OnStart(string[] args) {
			try {
				server = new CardsEventsWatcherServer();
				serverThread = new Thread(new ThreadStart(server.Start));
				serverThread.Start();
			} catch(System.Exception e) {
				Log.Fatal(e.Message + "\n" + e.StackTrace);
			}
		}

		protected override void OnStop() {
			server.Stop();
			serverThread.Join(2000);
			server.Dispose();
		}
	}

	public static class Log {
		private static Logger log = LogManager.GetCurrentClassLogger();

		private enum Type { Trace, Debug, Info, Warn, Error, Fatal }

		public static void Trace(string message,
		[CallerFilePath] string filePath = "",
		[CallerLineNumber] int lineNumber = 0) {
			LoggMsg(Type.Trace, message, filePath, lineNumber);
		}

		public static void Debug(string message,
		[CallerFilePath] string filePath = "",
		[CallerLineNumber] int lineNumber = 0) {
			LoggMsg(Type.Debug, message, filePath, lineNumber);
		}

		public static void Info(string message,
		[CallerFilePath] string filePath = "",
		[CallerLineNumber] int lineNumber = 0) {
			LoggMsg(Type.Info, message, filePath, lineNumber);
		}

		public static void Warn(string message,
		[CallerFilePath] string filePath = "",
		[CallerLineNumber] int lineNumber = 0) {
			LoggMsg(Type.Warn, message, filePath, lineNumber);
		}

		public static void Fatal(string message,
		[CallerFilePath] string filePath = "",
		[CallerLineNumber] int lineNumber = 0) {
			LoggMsg(Type.Fatal, message, filePath, lineNumber);
		}

		private static void LoggMsg(Type logType, string message, string filePath, int lineNumber) {
			message = $"[{filePath.Substring(filePath.LastIndexOf('\\'))} {lineNumber}] {message}";
			switch(logType) {
				case Type.Trace:
					log.Trace(message);
					break;

				case Type.Debug:
					log.Debug(message);
					break;

				case Type.Info:
					log.Info(message);
					break;

				case Type.Warn:
					log.Warn(message);
					break;

				case Type.Error:
					log.Error(message);
					break;

				case Type.Fatal:
					log.Error(message);
					break;
			}
		}
	}
}