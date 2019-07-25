using CS_EventsServer.Server;
using CS_EventsServer.Server.Interfaces;
using NLog;
using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace CS_EventsServer {

	public partial class EventsWatcherSvc: ServiceBase {
		private readonly CancellationTokenSource watcherCancTokenSource;

		public EventsWatcherSvc() {
			watcherCancTokenSource = new CancellationTokenSource();
			InitializeComponent();

			CanPauseAndContinue = true;
			CanShutdown = true;
			CanStop = true;
			AutoLog = false;
		}

		protected override void OnStart(string[] args) { restart(); base.OnStart(args); }

		protected override void OnContinue() { restart(); base.OnContinue(); }

		protected override void OnPause() { stop(); base.OnPause(); }

		protected override void OnShutdown() { stop(); base.OnShutdown(); }

		protected override void OnStop() { stop(); base.OnStop(); }

		private void restart() {
			try {
				var watcher = new CSEventsServer();

				// Run watcher in new Task, which will be started in new separae thread 
				Task.Factory.StartNew(() => watcher.Start(watcherCancTokenSource.Token),
					watcherCancTokenSource.Token,
					TaskCreationOptions.LongRunning,
					TaskScheduler.Default)
				//.Unwrap()
				.ContinueWith(task => {
					// if there are uncatched exceptions -> print them
					if(task.Status == TaskStatus.Faulted)
						Log.Fatal(task.Exception.ToString());
					watcher.Dispose();
				}, TaskContinuationOptions.ExecuteSynchronously);

			} catch(Exception e) {
				Log.Fatal(e.ToString());
			}
		}

		private void stop() {
			watcherCancTokenSource.Cancel();
		}

		#region IDisposable Support

		/// <summary>
		/// Освободить все используемые ресурсы.
		/// </summary>
		/// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
				watcherCancTokenSource?.Dispose();
			}
			base.Dispose(disposing);
		}

		#endregion IDisposable Support
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