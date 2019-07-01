using TableDependency.SqlClient.Base;

namespace CS_EventsServer.Entities {
	class Events55 {
		public int EventTime { get; set; }
		public int colHolderID { get; set; }
		public int CardNumber { get; set; }
		public int Direction { get; set; }
	}

	static class Events55Mapper {
		public static ModelToTableMapper<Events55> Get() {
			var mapper = new ModelToTableMapper<Events55>();
			mapper.AddMapping(model => model.CardNumber, "colEventTime");
			mapper.AddMapping(model => model.CardNumber, "colCardNumber");
			mapper.AddMapping(model => model.colHolderID, "colHolderID");
			mapper.AddMapping(model => model.Direction, "colDirection");
			return mapper;
		}
	}
}
