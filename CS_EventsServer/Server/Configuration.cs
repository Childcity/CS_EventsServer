using CS_EventsServer.Server.Interfaces;
using System;
using System.Configuration;
using System.Linq;

namespace CS_EventsServer.Server {

	internal class ServerConfiguration: AConfiguration {

		public override void Load() {
			ConnectionString = ConfigurationManager.ConnectionStrings["SqlServ"]?.ConnectionString;
			string ServersUrlsString = ConfigurationManager.AppSettings["ServersUrls"];
			string[] clientsUrlsStr = ServersUrlsString?.Split(';').Select(s => s.Trim()).ToArray();

			if(clientsUrlsStr != null)
				foreach(var client in clientsUrlsStr) {
					try {
						Uri clientUri = new Uri(client);
						ServersUrls.Add(clientUri);
					} catch(UriFormatException ex) {
						throw new ArgumentException($"Incorrect format of URL has been  added to settings file! ({ex.Message})");
					}
				}
		}
	}
}