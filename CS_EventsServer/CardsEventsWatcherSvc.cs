using CS_EventsServer.Server;
using CS_EventsServer.Server.Interfaces;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CS_EventsServer {
	public partial class CardsEventsWatcherSvc: ServiceBase {
		private static Logger log = LogManager.GetCurrentClassLogger();
		ICardsEventsWatcherServer server;
		Thread serverThread;

		public CardsEventsWatcherSvc() {
			InitializeComponent();
			CanStop = true;
			CanPauseAndContinue = true;
			AutoLog = false;
		}

		protected override void OnStart(string[] args) {
			server = new CardsEventsWatcherServer();
			serverThread = new Thread(new ThreadStart(server.Start));
			serverThread.Start();
		}

		protected override void OnStop() {
			server.Stop();
			serverThread.Join(1000);
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
				case Type.Trace: log.Trace(message); break;
				case Type.Debug: log.Debug(message); break;
				case Type.Info: log.Info(message); break;
				case Type.Warn: log.Warn(message); break;
				case Type.Error: log.Error(message); break;
				case Type.Fatal: log.Error(message); break;
			}
		}
	}
}
