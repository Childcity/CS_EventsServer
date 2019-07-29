using CS_EventsServer.Server.DAL.EF;
using CS_EventsServer.Server.DAL.Entities;
using CS_EventsServer.Server.DAL.Interfaces;
using System;
using System.Data.Entity;
using System.Linq;

namespace CS_EventsServer.Server.DAL.Repositories.SQLServer {

	internal class Accounts55Repository: IRepository<Account55> {
		private StopNet4Context db;

		public Accounts55Repository(StopNet4Context dbContext) {
			db = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		}

		public IQueryable<Account55> GetAll(bool asNoTracking) {
			return asNoTracking ? db.Accounts55.AsNoTracking() : db.Accounts55;
		}

		public Account55 Get(int id) {
			return db.Accounts55.Find(id);
		}

		public void Create(Account55 play) {
			db.Accounts55.Add(play);
		}

		public void Update(Account55 play) {
			db.Entry(play).State = EntityState.Modified;
		}

		public void Delete(int id) {
			Account55 play = db.Accounts55.Find(id);
			if(play != null)
				db.Accounts55.Remove(play);
		}
	}
}