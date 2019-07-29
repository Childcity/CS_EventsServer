using CS_EventsServer.Server.DAL.EF;
using CS_EventsServer.Server.DAL.Entities;
using CS_EventsServer.Server.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace CS_EventsServer.Server.DAL.Repositories.SQLServer {

	internal class VisitorsRepository: IRepository<Visitor> {
		private StopNet4Context db;

		public VisitorsRepository(StopNet4Context dbContext) {
			db = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		}

		public IEnumerable<Visitor> GetAll(bool asNoTracking) {
			return asNoTracking ? db.Visitors.AsNoTracking() : db.Visitors;
		}

		public Visitor Get(int id) {
			return db.Visitors.Find(id);
		}

		public void Create(Visitor play) {
			db.Visitors.Add(play);
		}

		public void Update(Visitor play) {
			db.Entry(play).State = EntityState.Modified;
		}

		public void Delete(int id) {
			Visitor play = db.Visitors.Find(id);
			if(play != null)
				db.Visitors.Remove(play);
		}
	}
}