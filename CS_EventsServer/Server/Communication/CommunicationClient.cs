using CS_EventsServer.Server.Comunication.Commands;
using CS_EventsServer.Server.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using WebSocketSharp;

namespace CS_EventsServer.Server.Comunication {

	internal class ComunicationClient: ICommunicator {
		private readonly string execPath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
		private List<WebSocket> serversWS = new List<WebSocket>();

		// server ping timer
		private System.Timers.Timer heartBeatTimer;

		public List<Uri> ServersUrls { get; set; } = new List<Uri>();

		public event EventHandler<CommandBase> OnRequest;

		public void ConnectSubscribers() {
			foreach(var serverUrl in ServersUrls) {
				connectToServerWS(serverUrl.ToString());
			}
			
			heartBeatTimer = new System.Timers.Timer(1000);
			heartBeatTimer.Elapsed += onTimerElapsed;
			heartBeatTimer.Start();	
		}

		public Task NotifyAll(CommandBase command, CancellationToken cancellationToken) {
			return Task.Run(() => {
				lock(serversWS) {
					serversWS.ForEach(
						ws => {
							if(cancellationToken.IsCancellationRequested)
								return;

							if(ws.IsAlive)
								ws.SendAsync(JsonConvert.SerializeObject(command, Formatting.Indented), null);
							else
								Log.Trace("Server doesn't alive: " + ws.Url);

							Log.Trace("Sending to server: " + JsonConvert.SerializeObject(command, Formatting.Indented));
						});
				}
			}, cancellationToken);
		}

		private void connectToServerWS(string serverUrl) {
			var ws = new WebSocket(serverUrl);

			ws.Log.Level = LogLevel.Warn;
			ws.Log.File = execPath + @"\logs\ws_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";

			ws.OnOpen += onOpen;
			ws.OnMessage += onMessage;
			ws.OnError += onError;
			ws.OnClose += onClosed;

			lock(serversWS) {
				serversWS.Add(ws);
			}

			ws.ConnectAsync();
		}

		private void onMessage(object sender, MessageEventArgs e) {
			Log.Trace("onMessage: " + e.Data);
			if(e.IsText) {
				var command = CommandBase.FromJson(e.Data);
				var receivers = OnRequest.GetInvocationList();
				foreach(EventHandler<CommandBase> receiver in receivers) {
					OnRequest?.BeginInvoke(this, command, null, null);
				}
			}
		}

		private void onError(object sender, ErrorEventArgs e) {
			Log.Warn("onError: " + ((WebSocket)sender).Url + ": " + e.Message + "\n" + e.Exception.ToString());
		}

		private void onClosed(object sender, CloseEventArgs e) {
			WebSocket serverWs = (WebSocket)sender;
			string serverWSUrl = serverWs.Url.ToString();

			//Log.Warn("Connection with " + serverWSUrl + " was closed: " + e.Reason);

			serverWs.OnOpen -= onOpen;
			serverWs.OnMessage -= onMessage;
			serverWs.OnError -= onError;
			serverWs.OnClose -= onClosed;

			if(disposedValue) {
				return;
			}

			lock(serversWS) {
				serversWS.Remove(serverWs);
			}

			//Log.Debug("Reconnecting to " + serverWSUrl);

			connectToServerWS(serverWSUrl);
		}

		private void onOpen(object sender, EventArgs e) {
			Log.Trace("WS connection opened");
		}

		private void onTimerElapsed(object source, ElapsedEventArgs e) {
			lock(serversWS) {
				serversWS.ForEach(ws => {
					bool ping = ws.Ping();
					//Log.Trace("Server " + ws.Url + " Ping: " + ping);
				});
			}
		}

		#region IDisposable Support

		private bool disposedValue = false;

		protected virtual void Dispose(bool disposing) {
			if(!disposedValue) {
				if(disposing) {
					if(serversWS != null) {
						serversWS.ForEach(ws => {
							ws.OnOpen -= onOpen;
							ws.OnMessage -= onMessage;
							ws.OnError -= onError;
							ws.OnClose -= onClosed;
							if(ws.ReadyState != WebSocketState.Closed) {
								try { ws.CloseAsync(CloseStatusCode.Normal); } finally { ws = null; }
							}
						});

						serversWS.Clear();
						serversWS = null;
					}

					try { heartBeatTimer.Stop(); } finally {
						heartBeatTimer.Elapsed -= onTimerElapsed;
						heartBeatTimer.Dispose();
						heartBeatTimer = null;
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