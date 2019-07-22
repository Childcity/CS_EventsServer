using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_EventsServer.Server {
	class Configuration {
		public List<Uri> ServersUrls { get; private set; } = new List<Uri>();
		public string ConnectionString = default;

		public void Load() {
			ConnectionString = ConfigurationManager.ConnectionStrings["SqlServ"]?.ConnectionString;
			string clientDomeins = ConfigurationManager.AppSettings["ServersUrls"];
			string[] clientsUrlsStr = clientDomeins?.Split(';').Select(s => s.Trim()).ToArray();

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
