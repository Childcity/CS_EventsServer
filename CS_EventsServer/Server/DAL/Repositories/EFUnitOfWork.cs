using CS_EventsServer.Server.DAL.EF;
using CS_EventsServer.Server.DAL.Entities;
using CS_EventsServer.Server.DAL.Interfaces;
using CS_EventsServer.Server.DAL.Repositories.SQLServer;

namespace CS_EventsServer.Server.DAL.Repositories {

	public class EFUnitOfWork: IUnitOfWork {
		private readonly StopNet4Context db;

		private IRepository<Event55> event55Repository;
		private IRepository<Zone55> zones55Repository;
		private IRepository<Employe> emploeesRepository;
		private IRepository<Visitor> visitorsRepository;
		private IRepository<Vehicle> vehiclesRepository;
		private IRepository<Account55> accountsRepository;

		private IRepository<ControlPoint> controlPointsRepository;

		public IRepository<Event55> Events55 {
			get => event55Repository ?? (event55Repository = new Events55Repository(db));
		}

		public IRepository<Zone55> Zones55 {
			get => zones55Repository ?? (zones55Repository = new Zones55Repository(db));
		}

		public IRepository<Employe> Employees {
			get => emploeesRepository ?? (emploeesRepository = new EmployeesRepository(db));
		}

		public IRepository<Visitor> Visitors {
			get => visitorsRepository ?? (visitorsRepository = new VisitorsRepository(db));
		}

		public IRepository<Vehicle> Vehicles {
			get => vehiclesRepository ?? (vehiclesRepository = new VehiclesRepository(db));
		}

		public IRepository<Account55> Accounts55 {
			get => accountsRepository ?? (accountsRepository = new Accounts55Repository(db));
		}

		public IRepository<ControlPoint> ControlPoints {
			get => controlPointsRepository ?? (controlPointsRepository = new ControlPointsRepository(db));
		}

		public EFUnitOfWork(string connectionString) => db = new StopNet4Context(connectionString);

		public void Save() {
			db.SaveChanges();
		}

		#region IDisposable Support

		private bool disposedValue = false;

		protected virtual void Dispose(bool disposing) {
			if(!disposedValue) {
				if(disposing) {
					db.Dispose();
				}
				disposedValue = true;
			}
		}

		public void Dispose() {
			Dispose(true);
		}

		#endregion IDisposable Support
	}
}