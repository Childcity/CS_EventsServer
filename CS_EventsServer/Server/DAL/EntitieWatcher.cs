using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Reflection;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base;
using TableDependency.SqlClient.Exceptions;
using TableDependency.SqlClient.Where;

namespace CS_EventsServer.Server.DAL {

	public class EntitieWatcher<EntType>: IDisposable where EntType : class, new() {
		public SqlTableDependency<EntType> Dependancy { get; private set; } = null;

		public EntitieWatcher(string connectionString, Expression<Func<EntType, bool>> filter = null) {
			Type entType = new EntType().GetType(); // temporary instance of <EntType> for getting type of EntType in runtime!

			try {
				initDependancy(connectionString, filter, entType);
			} catch(ServiceBrokerNotEnabledException servBrEx) {
				try {
					// Get database name for enabaling broker for this db
					SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
					string dbName = builder.InitialCatalog;

					Log.Warn($"Service broker not enable. Trying to activate it by executing 'ALTER DATABASE [{dbName}] SET ENABLE_BROKER WITH ROLLBACK IMMEDIATE'");
					using(SqlConnection conn = new SqlConnection(connectionString)) {
						string enableBrokerCmd = $"ALTER DATABASE [{dbName}] SET ENABLE_BROKER WITH ROLLBACK IMMEDIATE;";
						using(SqlCommand cmd = new SqlCommand(enableBrokerCmd, conn)) {
							conn.Open();
							cmd.ExecuteNonQuery();
							conn.Close();
						}
					}

					initDependancy(connectionString, filter, entType);
				} catch(Exception e) {
					throw new ArgumentException($"ServiceBroker can't be enabled. ---> [{e.Message}] ---> [{servBrEx.Message}]. See Inner Exeption!", e);
				}
			} catch(Exception e) {
				throw new ArgumentException($"EntitieWatcher<{entType.Name}> can't setup SqlTableDependency<{entType.Name}> with type <{entType.Name}>. Provide <EntType> with attributes for Entitie Framework.", e);
			}
		}

		private void initDependancy(string connectionString, Expression<Func<EntType, bool>> filter, Type entType) {

			// Get table Attribute for explore Name and Schema of table
			TableAttribute tableAttribute = entType.GetCustomAttribute<TableAttribute>(false);
			Log.Trace("Mapping entitie [" + entType.Name + "] ---> " + tableAttribute?.Name);

			// Construct mapper from attributes, that should be in Entitie class
			ModelToTableMapper<EntType> mapper = getMapperFromEntitieAtributes(ref entType);

			Dependancy = new SqlTableDependency<EntType>(
				connectionString,
				tableAttribute?.Name ?? entType.Name,
				tableAttribute?.Schema ?? "dbo",
				mapper, filter: new SqlTableDependencyFilter<EntType>(filter, mapper)
			);
		}

		public void Start() {
			Dependancy?.Start();
		}

		private ModelToTableMapper<EntType> getMapperFromEntitieAtributes(ref Type entType) {
			ModelToTableMapper<EntType> mapper = new ModelToTableMapper<EntType>();

			foreach(PropertyInfo prop in entType.GetProperties()) {
				object[] attrs = prop.GetCustomAttributes(false);
				foreach(object attr in attrs) {
					if(attr is ColumnAttribute colAttr) {
						(string propName, string colName) = (prop.Name, colAttr.Name);
						Log.Trace("\t\t" + propName + " ---> " + colName);
						mapper.AddMapping(prop, colName);
					}
				}
			}

			return mapper;
		}

		#region IDisposable Support

		private bool disposedValue = false;

		protected virtual void Dispose(bool disposing) {
			if(!disposedValue) {
				if(disposing) {
					try {
						Dependancy?.StopWithoutDisposing();
					} finally {
						Dependancy?.Dispose();
						Dependancy = null;
					}
				}
				Log.Trace($"Disposed: {GetType().Namespace}\\{GetType().Name} <{typeof(EntType).Name}>");
				disposedValue = true;
			}
		}

		public void Dispose() {
			Dispose(true);
		}

		#endregion IDisposable Support
	}
}