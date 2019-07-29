using System.Collections.Generic;
using System.Linq;

namespace CS_EventsServer.Server.DAL.Interfaces {

	public interface IRepository<T>
		where T : class {

		IQueryable<T> GetAll(bool asNoTracking = false);

		T Get(int id);

		void Create(T item);

		void Update(T item);

		void Delete(int id);
	}
}