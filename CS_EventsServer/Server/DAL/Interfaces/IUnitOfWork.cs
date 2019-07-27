using CS_EventsServer.Server.DAL.Entities;
using System;

namespace CS_EventsServer.Server.DAL.Interfaces {

	public interface IUnitOfWork: IDisposable {
		IRepository<Event55> Events55 { get; }
		IRepository<Zone55> Zones55 { get; }
		IRepository<Employe> Employees { get; }
		IRepository<Visitor> Visitors { get; }
		IRepository<Vehicle> Vehicles { get; }
		IRepository<Account55> Accounts55 { get; }

		IRepository<ControlPoint> ControlPoints { get; }

		void Save();
	}
}