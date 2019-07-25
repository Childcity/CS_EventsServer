using CS_EventsServer.Server.DAL.Entities;
using System.Data.Entity;

namespace CS_EventsServer.Server.DAL.EF {

	internal class StopNet4Context: DbContext {
		public DbSet<Event55> Events55 { get; set; }

		public StopNet4Context(string connectionString)
			: base(connectionString) {
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder) {
			modelBuilder.Entity<Event55>().HasKey(col => new { col.EventNumber, col.EventTime });
		}
	}
}