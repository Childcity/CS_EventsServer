using CodeFirstStoreFunctions;
using CS_EventsServer.Server.DAL.Entities;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace CS_EventsServer.Server.DAL.EF {

	internal class StopNet4Context: DbContext {
		public DbSet<Event55> Events55 { get; set; }
		public DbSet<Zone55> Zones55 { get; set; }
		public DbSet<Employe> Employees { get; set; }
		public DbSet<Visitor> Visitors { get; set; }
		public DbSet<Vehicle> Vehicles { get; set; }
		public DbSet<Account55> Accounts55 { get; set; }

		public DbSet<ControlPoint> GetControlPointsResult { get; set; }

		[DbFunction("StopNet4Context", "fnGetControlPoints")]
		public virtual System.Linq.IQueryable<ControlPoint> GetControlPoints() {
			return (this as IObjectContextAdapter).ObjectContext.CreateQuery<ControlPoint>("StopNet4Context.fnGetControlPoints()");
		}

		public StopNet4Context(string connectionString)
			: base(connectionString) {
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder) {
			modelBuilder.Entity<Event55>().HasKey(col => new { col.EventNumber, col.EventTime });
			modelBuilder.Conventions.Add(new FunctionsConvention<StopNet4Context>("dbo"));
		}
	}
}