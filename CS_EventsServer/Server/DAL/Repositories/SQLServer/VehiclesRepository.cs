using CS_EventsServer.Server.DAL.EF;
using CS_EventsServer.Server.DAL.Entities;
using CS_EventsServer.Server.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace CS_EventsServer.Server.DAL.Repositories.SQLServer {

	internal class VehiclesRepository: IRepository<Vehicle> {
		private StopNet4Context db;

		public VehiclesRepository(StopNet4Context dbContext) {
			db = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		}

		public IEnumerable<Vehicle> GetAll(bool asNoTracking) {
			return asNoTracking ? db.Vehicles.AsNoTracking() : db.Vehicles;
		}

		public Vehicle Get(int id) {
			return db.Vehicles.Find(id);
		}

		public void Create(Vehicle play) {
			db.Vehicles.Add(play);
		}

		public void Update(Vehicle play) {
			db.Entry(play).State = EntityState.Modified;
		}

		public void Delete(int id) {
			Vehicle play = db.Vehicles.Find(id);
			if(play != null)
				db.Vehicles.Remove(play);
		}
	}
}