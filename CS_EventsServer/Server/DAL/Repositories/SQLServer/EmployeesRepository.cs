using CS_EventsServer.Server.DAL.EF;
using CS_EventsServer.Server.DAL.Entities;
using CS_EventsServer.Server.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace CS_EventsServer.Server.DAL.Repositories.SQLServer {

	internal class EmployeesRepository: IRepository<Employe> {
		private StopNet4Context db;

		public EmployeesRepository(StopNet4Context dbContext) {
			db = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		}

		public IEnumerable<Employe> GetAll(bool asNoTracking) {
			return asNoTracking ? db.Employees.AsNoTracking() : db.Employees;
		}

		public Employe Get(int id) {
			return db.Employees.Find(id);
		}

		public void Create(Employe play) {
			db.Employees.Add(play);
		}

		public void Update(Employe play) {
			db.Entry(play).State = EntityState.Modified;
		}

		public void Delete(int id) {
			Employe play = db.Employees.Find(id);
			if(play != null)
				db.Employees.Remove(play);
		}
	}
}