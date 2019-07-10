using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using CS_EventsServer.Server.DAL.Entities;
using CS_EventsServer.Server.DAL.Repositories.SQLServer;

namespace CS_EventsServer.Server.DAL.EF {
	class StopNet4Context: DbContext {
		public DbSet<Event55> Events55 { get; set; }

		public StopNet4Context(string connectionString)
			: base(connectionString) {

		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
			modelBuilder.Entity<Event55>().HasKey(col => new{ col.EventNumber, col.EventTime });
		}
	}
}
