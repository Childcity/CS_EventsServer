using System;
using System.Collections.Generic;

namespace CS_EventsServer.Server.Interfaces {

	public abstract class AConfiguration: IConfiguration {
		public List<Uri> ServersUrls { get; protected set; } = new List<Uri>();
		public string ConnectionString = null;

		public abstract void Load();
	}
}