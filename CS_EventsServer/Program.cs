﻿using System.ServiceProcess;

namespace CS_EventsServer {

	internal static class Program {

		/// <summary>
		/// Главная точка входа для приложения.
		/// </summary>
		private static void Main() {
			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[]
			{
				new EventsWatcherSvc()
			};
			ServiceBase.Run(ServicesToRun);
		}
	}
}