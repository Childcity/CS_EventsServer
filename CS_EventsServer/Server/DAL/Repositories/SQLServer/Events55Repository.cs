﻿using CS_EventsServer.Server.DAL.EF;
using CS_EventsServer.Server.DAL.Entities;
using CS_EventsServer.Server.DAL.Interfaces;
using System;
using System.Data.Entity;
using System.Linq;

namespace CS_EventsServer.Server.DAL.Repositories.SQLServer {

	internal class Events55Repository: IRepository<Event55> {
		private StopNet4Context db;

		public Events55Repository(StopNet4Context dbContext) {
			db = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		}

		public IQueryable<Event55> GetAll(bool asNoTracking) {
			return asNoTracking ? db.Events55.AsNoTracking() : db.Events55;
		}

		public Event55 Get(int id) {
			return db.Events55.Find(id);
		}

		public void Create(Event55 play) {
			db.Events55.Add(play);
		}

		public void Update(Event55 play) {
			db.Entry(play).State = EntityState.Modified;
		}

		public void Delete(int id) {
			Event55 play = db.Events55.Find(id);
			if(play != null)
				db.Events55.Remove(play);
		}
	}
}