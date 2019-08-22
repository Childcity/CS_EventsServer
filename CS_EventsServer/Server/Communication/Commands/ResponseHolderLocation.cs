using CS_EventsServer.Server.DTO;

namespace CS_EventsServer.Server.Comunication.Commands {

	public class ResponseHolderLocation: CommandBase {
		public static string Name { get => typeof(ResponseHolderLocation).Name; }

		public ResponseHolderLocation() : base(Name) {}

		public ResponseHolderLocation(HolderLocationDTO coworkerLocation) {
			Params = coworkerLocation;
		}
	}
}