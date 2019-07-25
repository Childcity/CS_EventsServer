﻿using CS_EventsServer.Server.DTO;

namespace CS_EventsServer.Server.Comunication.Commands {

	public class RequestPushEvent: CommandBase {
		public RequestPushEvent() : base("PushEventToRemoteServer") { }

		public RequestPushEvent(EventDTO eventDTO) {
			Params = eventDTO;
		}
	}
}