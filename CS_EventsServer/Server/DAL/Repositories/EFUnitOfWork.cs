using CS_EventsServer.Server.DAL.EF;
using CS_EventsServer.Server.DAL.Entities;
using CS_EventsServer.Server.DAL.Interfaces;
using CS_EventsServer.Server.DAL.Repositories.SQLServer;
using System;

namespace NLayerApp.DAL.Repositories {
	public class EFUnitOfWork :IUnitOfWork {
		private StopNet4Context db;
		private Event55Repository event55Repository;

		public IRepository<Event55> Events55 {
			get => event55Repository ?? (event55Repository = new Event55Repository(db));
		}

		public EFUnitOfWork(string connectionString) => db = new StopNet4Context(connectionString);

		public void Save() {
			db.SaveChanges();
		}

		#region IDisposable Support
		private bool disposedValue = false; // Для определения избыточных вызовов

		protected virtual void Dispose(bool disposing) {
			if(!disposedValue) {
				if(disposing) {
					db.Dispose();
				}
				disposedValue = true;
			}
		}

		// Этот код добавлен для правильной реализации шаблона высвобождаемого класса.
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
