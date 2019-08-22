using CS_EventsServer.Server.DTO;

namespace CS_EventsServer.Server.Comunication.Commands {

	public class RequestPushEvent: CommandBase {
		public static string Name { get => typeof(RequestPushEvent).Name; }

		public RequestPushEvent() : base(Name) { }

		public RequestPushEvent(EventDTO eventDTO) {
			Params = eventDTO;
		}
	}
}