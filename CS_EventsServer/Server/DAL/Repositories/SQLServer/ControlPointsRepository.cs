using CS_EventsServer.Server.DAL.EF;
using CS_EventsServer.Server.DAL.Entities;
using CS_EventsServer.Server.DAL.Interfaces;
using System;
using System.Data.Entity;
using System.Linq;

namespace CS_EventsServer.Server.DAL.Repositories.SQLServer {

	internal class ControlPointsRepository: IRepository<ControlPoint> {
		private StopNet4Context db;

		public ControlPointsRepository(StopNet4Context dbContext) {
			db = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		}

		public IQueryable<ControlPoint> GetAll(bool asNoTracking) {
			return asNoTracking ? db.GetControlPoints().AsNoTracking() : db.GetControlPoints();
		}

		public void Create(ControlPoint item) {
			throw new NotSupportedException();
		}

		public void Delete(int id) {
			throw new NotSupportedException();
		}

		public ControlPoint Get(int id) {
			throw new NotSupportedException();
		}

		public void Update(ControlPoint item) {
			throw new NotSupportedException();
		}
	}
}