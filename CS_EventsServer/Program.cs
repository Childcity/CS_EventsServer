using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace CS_EventsServer {
	static class Program {
		/// <summary>
		/// Главная точка входа для приложения.
		/// </summary>
		static void Main() {
			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[]
			{
				new EventsWatcherSvc()
			};
			ServiceBase.Run(ServicesToRun);
		}
	}
}
