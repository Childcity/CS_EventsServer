using CS_EventsServer.Server.DAL.Entities;
using System;

namespace CS_EventsServer.Server.DAL.Interfaces {

	public interface IUnitOfWork: IDisposable {
		IRepository<Event55> Events55 { get; }

		void Save();
	}
}