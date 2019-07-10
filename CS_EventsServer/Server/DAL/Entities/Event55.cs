using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CS_EventsServer.Server.DAL.Entities {
	[Table("tblEvents_55", Schema = "dbo")]
	public class Event55 {
		[Column("colEventNumber"), Required]
		public decimal EventNumber { get; set; }

		[Column("colEventCode"), Required]
		public int EventCode { get; set; }

		[Column("colGeneratorID"), Required]
		public int GeneratorID { get; set; }

		[Column("colInitiatorID"), Required]
		public int InitiatorID { get; set; }

		[Column("colAccountID")]
		public int? AccountID { get; set; }

		[Column("colHolderID")]
		public int? HolderID { get; set; }

		[Column("colControlPointID")]
		public int? ControlPointID { get; set; }

		[Column("colStartZoneID")]
		public int? StartZoneID { get; set; }
		
		[Column("colTargetZoneID")]
		public int? TargetZoneID { get; set; }

		[Column("colDirection")]
		public byte? Direction { get; set; }
		
		[Column("colCardNumber")]
		public decimal? CardNumber { get; set; }

		[Column("colGroupID")]
		public int? GroupID { get; set; }

		[Column("colEventTime"), Required]
		public DateTime EventTime { get; set; }
	}
}