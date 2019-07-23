using CS_EventsServer.Server.Comunication.Commands;
using CS_EventsServer.Server.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace CS_EventsServer.Server.Comunication {
	class ComunicationClient: IComunicator {
		private readonly string execPath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
		private List<WebSocket> serversWS = new List<WebSocket>();
		
		public List<Uri> ServersUrls { get; set; } = new List<Uri>();

		public void ConnectSubscribers() {
			foreach(var serverUrl in ServersUrls) {
				connectToServerWS(serverUrl.ToString());
			}
		}

		public void NotifyAll(CommandBase command) {
			lock(serversWS) {
				serversWS.ForEach(
					ws => {
						if(ws.IsAlive)
							ws.SendAsync(JsonConvert.SerializeObject(command, Formatting.Indented), null);
						else
							Log.Debug("Not alive: " + ws.Url);
					});
			}
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
			Log.Trace(e.Data);
		}

		private void onError(object sender, ErrorEventArgs e) {
			Log.Warn(((WebSocket)sender).Url + ": " + e.Message + "\n" + e.Exception.ToString());
		}

		private void onClosed(object sender, CloseEventArgs e) {
			WebSocket serverWs = (WebSocket)sender;
			string serverWSUrl = serverWs.Url.ToString();

			Log.Warn("Connection with " + serverWSUrl + " was closed: " + e.Reason);

			if(disposedValue) {
				return;
			}

			Log.Debug("Reconnecting to " + serverWSUrl);
			
			serverWs.ConnectAsync();

			lock(serversWS) {
				serversWS.Remove(serverWs);
			}

			connectToServerWS(serverWSUrl);
		}

		private void onOpen(object sender, EventArgs e) {
			// send Auth request 
			// create list with auth clients
		}

		#region IDisposable Support
		private bool disposedValue = false;

		protected virtual void Dispose(bool disposing) {
			if(!disposedValue) {
				if(disposing) {
					if(serversWS != null) {
						serversWS?.Clear();
						serversWS = null;
					}
				}
				disposedValue = true;
			}
		}

		public void Dispose() {
			Dispose(true);
		}
		#endregion

	}
}
