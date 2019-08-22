using CS_EventsServer.Server.DTO;

namespace CS_EventsServer.Server.Comunication.Commands {

	public class RequestHolderLocation: CommandBase {
		public static string Name { get => typeof(RequestHolderLocation).Name; }

		public RequestHolderLocation() : base(Name) { }

		public RequestHolderLocation(HolderLocationPeriodDTO holderLocationPeriod) {
			Params = holderLocationPeriod;
		}
	}
}