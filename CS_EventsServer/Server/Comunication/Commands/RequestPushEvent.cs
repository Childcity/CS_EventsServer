using CS_EventsServer.Server.DTO;

namespace CS_EventsServer.Server.Comunication.Commands {
	public class RequestPushEvent: CommandBase<EventDTO> {
		public RequestPushEvent(EventDTO eventDTO) {
			Params = eventDTO;
		}
	}
}
