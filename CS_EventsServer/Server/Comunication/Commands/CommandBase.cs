using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_EventsServer.Server.Comunication.Commands {
	public abstract class CommandBase {
		public readonly string Command;
		public readonly Guid CommandId;
		public readonly DateTime TimeStamp;

		public object Params;

		public CommandBase() {
			Command = GetType().Name;
			CommandId = Guid.NewGuid();
			TimeStamp = DateTime.Now;
		}
	}
}
