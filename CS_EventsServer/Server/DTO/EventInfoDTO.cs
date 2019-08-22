using CS_EventsServer.Server.Comunication;
using Newtonsoft.Json;
using System;

namespace CS_EventsServer.Server.DTO {

	public partial class EventInfoDTO {
		
		public int? EventCode { get; set; }

		public byte? Direction { get; set; }

		public DateTime? EventTime { get; set; }
		
		//

		public string StartAreaName { get; set; }

		public string TargetAreaName { get; set; }

		//

		public string ObjectType { get; set; }

		public string ObjectName { get; set; }
	}

	public partial class EventInfoDTO {

		public static EventInfoDTO FromObject(object obj) => FromJson(JsonConvert.SerializeObject(obj, JsonConverterSettings.Settings));

		public static EventInfoDTO FromJson(string json) => JsonConvert.DeserializeObject<EventInfoDTO>(json, JsonConverterSettings.Settings);
	}

	public static class SerializeLocationInfoDTO {

		public static string ToJson(this EventDTO self) => JsonConvert.SerializeObject(self, JsonConverterSettings.Settings);
	}
}