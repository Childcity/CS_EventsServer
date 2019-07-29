using CS_EventsServer.Server.DAL.EF;
using CS_EventsServer.Server.DAL.Entities;
using CS_EventsServer.Server.DAL.Interfaces;
using System;
using System.Data.Entity;
using System.Linq;

namespace CS_EventsServer.Server.DAL.Repositories.SQLServer {

	internal class Zones55Repository: IRepository<Zone55> {
		private StopNet4Context db;

		public Zones55Repository(StopNet4Context dbContext) {
			db = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		}

		public IQueryable<Zone55> GetAll(bool asNoTracking) {
			return asNoTracking ? db.Zones55.AsNoTracking() : db.Zones55;
		}

		public Zone55 Get(int id) {
			return db.Zones55.Find(id);
		}

		public void Create(Zone55 play) {
			db.Zones55.Add(play);
		}

		public void Update(Zone55 play) {
			db.Entry(play).State = EntityState.Modified;
		}

		public void Delete(int id) {
			Zone55 play = db.Zones55.Find(id);
			if(play != null)
				db.Zones55.Remove(play);
		}
	}
}