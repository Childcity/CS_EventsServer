using CS_EventsServer.Server.Comunication;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Globalization;

namespace CS_EventsServer.Server.DTO {

	public partial class HolderLocationDTO {
		public EventDTO HolderInfo { get; set; }

		public TimePeriodDTO TimePeriod { get; set; }

		public List<EventInfoDTO> EventsInfo { get; set; }
	}

	public partial class HolderLocationDTO {

		public static HolderLocationDTO FromObject(object obj) => FromJson(JsonConvert.SerializeObject(obj, JsonConverterSettings.Settings));

		public static HolderLocationDTO FromJson(string json) => JsonConvert.DeserializeObject<HolderLocationDTO>(json, JsonConverterSettings.Settings);
	}

	public static class SerializeHolderLocationDTO {

		public static string ToJson(this HolderLocationDTO self, bool indented = false) => JsonConvert.SerializeObject(self, indented ? Formatting.Indented : Formatting.None, JsonConverterSettings.Settings);
		public static string ToJsonTimeLocal(this HolderLocationDTO self, bool indented = false) => JsonConvert.SerializeObject(self, indented ? Formatting.Indented : Formatting.None, new JsonSerializerSettings {
			MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
			NullValueHandling = NullValueHandling.Ignore,
			DateParseHandling = DateParseHandling.None,
			Converters = {
				new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeLocal }
			},
		});
	}
}