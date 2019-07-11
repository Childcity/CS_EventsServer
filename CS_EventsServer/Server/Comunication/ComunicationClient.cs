using CS_EventsServer.Server.Comunication.Commands;
using CS_EventsServer.Server.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace CS_EventsServer.Server.Comunication {
	class ComunicationClient: IComunicator {
		private List<WebSocket> serversWS = new List<WebSocket>();
		
		public List<Uri> ServersUrls { get; set; } = new List<Uri>();

		private WebSocket findSocketByUrl(Uri url) {
			return serversWS.Single(ws => ws.Url == url);
			//return null;
		}

		public void ConnectSubscribers() {
			foreach(var serverUrl in ServersUrls) {
				var ws = new WebSocket(serverUrl.ToString());
				serversWS.Add(ws);

				ws.Log.Level = LogLevel.Fatal;

				ws.OnOpen += onOpen;
				ws.OnMessage += onMessage;
				ws.OnError += onError;
				ws.OnClose += onClosed;
				
				ws.ConnectAsync();
			}
		}

		public void NotifyAll(CommandBase command) {
			serversWS.ForEach(
				ws => {
					if(ws.IsAlive)
						ws.SendAsync(JsonConvert.SerializeObject(command, Formatting.Indented), null);
				});
		}

		private void onMessage(object sender, MessageEventArgs e) {
			Log.Trace(e.Data);
		}

		private void onError(object sender, ErrorEventArgs e) {
			Log.Warn(e.Message + "\n" + e.Exception.ToString());
		}

		private void onClosed(object sender, CloseEventArgs e) {
			Log.Warn("Connection with " + ((WebSocket)sender).Url + " was closed: " + e.Reason);
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
						foreach(var ws in serversWS) {
							ws?.Close();
						}
						serversWS.Clear();
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
