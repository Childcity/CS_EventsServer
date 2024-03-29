﻿using CS_EventsServer.Server.DAL.Entities;
using CS_EventsServer.Server.DAL.Interfaces;
using CS_EventsServer.Server.DAL.Repositories;
using CS_EventsServer.Server.DTO;
using System;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CS_EventsServer.Server.BLL.Services {
	public class EventService: IDisposable {
		private CancellationToken cancellationToken;
		private IUnitOfWork unitOfWork;

		public EventService(string connectionString, CancellationToken token) {
			cancellationToken = token;
			unitOfWork = new EFUnitOfWork(connectionString);
		}

		public async Task<EventDTO> GetEventInfo(Event55 event55) {
			
			var query = from ev in unitOfWork.Events55.GetAll(true)
							join startZone55 in unitOfWork.Zones55.GetAll(true) on ev.StartZoneID equals startZone55.colID into startZone55_temp
							join targetZone55 in unitOfWork.Zones55.GetAll(true) on ev.StartZoneID equals targetZone55.colID into targetZone55_temp
							join account55 in unitOfWork.Accounts55.GetAll(true) on ev.AccountID equals account55.colAccountID into account55_temp
							join controlPoint in unitOfWork.ControlPoints.GetAll(true) on ev.ControlPointID equals controlPoint.colID into controlPoint_temp
							join holder in 
									(from eml in unitOfWork.Employees.GetAll(true) select new {
										Id = eml.colID, Surname = eml.colSurname, Name = eml.colName, Middlename = eml.colMiddlename,
										StateCode = "", DepartmentID = eml.colDepartmentID, Department = eml.colDepartment,
										TabNumber = eml.colTabNumber, Photo = eml.colPhoto, HolderType = "Employe"
									}).Concat
									(from vis in unitOfWork.Visitors.GetAll(true) select new {
										Id = vis.colID, Surname = vis.colSurname, Name = vis.colName, Middlename = vis.colMiddlename,
										StateCode = "", DepartmentID = (int?)null, Department = "",
										TabNumber = "", Photo = vis.colPhoto, HolderType = "Visitor" }
									).Concat
									(from veh in unitOfWork.Vehicles.GetAll(true) select new { 
										Id = veh.colID, Surname = "", Name = "", Middlename = "",
										StateCode = veh.colStateCode, DepartmentID = (int?)null, Department = "",
										TabNumber = "", Photo = veh.colPhoto, HolderType = "Vehicle"
									})
								on ev.HolderID equals holder.Id into holder_temp

						from startZone55 in startZone55_temp.DefaultIfEmpty()
						from targetZone55 in targetZone55_temp.DefaultIfEmpty()
						from account55 in account55_temp.DefaultIfEmpty()
						from controlPoint in controlPoint_temp.DefaultIfEmpty()
						from holder in holder_temp.DefaultIfEmpty()

						where ev.EventNumber == event55.EventNumber

						select new {
							Event55 = ev,
							StartZone = startZone55,
							TargetZone = targetZone55,
							Account55 = account55,
							ControlPoint = controlPoint,
							Holder = holder
						};

			var eventInfo = (await query.ToListAsync(cancellationToken)).FirstOrDefault();

			return eventInfo == null 
				? new EventDTO()
				: new EventDTO() {
					EventCode = eventInfo?.Event55?.EventCode,
					Direction = eventInfo?.Event55?.Direction,
					CardNumber = eventInfo?.Event55?.CardNumber,
					EventTime = eventInfo?.Event55?.EventTime.ToUniversalTime(),

					AccountNumber = eventInfo?.Account55?.colAccountNumber,

					StartAreaName = eventInfo?.StartZone?.colName,
					TargetAreaName = eventInfo?.TargetZone?.colName,

					ObjectType = eventInfo?.ControlPoint?.colType,
					ObjectName = eventInfo?.ControlPoint?.colName,

					HolderType = eventInfo?.Holder?.HolderType,
					HolderSurname = eventInfo?.Holder?.Surname,
					HolderName = eventInfo?.Holder?.Name,
					HolderMiddlename = eventInfo?.Holder?.Middlename,
					HolderDepartment = eventInfo?.Holder?.Department,
					HolderTabNumber = eventInfo?.Holder?.TabNumber,
					HolderPhoto = eventInfo?.Holder?.Photo
				};
		}

		public async Task<HolderLocationDTO> GetHolderLocations(HolderLocationPeriodDTO locationPeriod) {
			DateTime startTime = locationPeriod.TimePeriod.StartTime.Value.LocalDateTime;
			DateTime endTime = locationPeriod.TimePeriod.EndTime.Value.LocalDateTime;

			Log.Trace("from: " + startTime.ToString() + "    to: " + endTime.ToString());

			var query = from ev in unitOfWork.Events55.GetAll(true)
						join startZone55 in unitOfWork.Zones55.GetAll(true) on ev.StartZoneID equals startZone55.colID into startZone55_temp
						join targetZone55 in unitOfWork.Zones55.GetAll(true) on ev.StartZoneID equals targetZone55.colID into targetZone55_temp
						join controlPoint in unitOfWork.ControlPoints.GetAll(true) on ev.ControlPointID equals controlPoint.colID into controlPoint_temp
						join holder in 
									(from eml in unitOfWork.Employees.GetAll(true) select new {
										Id = eml.colID, Surname = eml.colSurname, Name = eml.colName, Middlename = eml.colMiddlename,
										StateCode = "", DepartmentID = eml.colDepartmentID, Department = eml.colDepartment,
										TabNumber = eml.colTabNumber, Photo = eml.colPhoto, HolderType = "Employe"
									}).Concat
									(from vis in unitOfWork.Visitors.GetAll(true) select new {
										Id = vis.colID, Surname = vis.colSurname, Name = vis.colName, Middlename = vis.colMiddlename,
										StateCode = "", DepartmentID = (int?)null, Department = "",
										TabNumber = "", Photo = vis.colPhoto, HolderType = "Visitor" }
									)
								on ev.HolderID equals holder.Id into holder_temp
								
						from startZone55 in startZone55_temp.DefaultIfEmpty()
						from targetZone55 in targetZone55_temp.DefaultIfEmpty()
						from controlPoint in controlPoint_temp.DefaultIfEmpty()
						from holder in holder_temp.DefaultIfEmpty()

						where	((
									holder.Name.ToLower().Contains(locationPeriod.HolderName.ToLower()) 
									&& holder.Middlename.ToLower().Contains(locationPeriod.HolderMiddlename.ToLower())
									&& holder.Surname.ToLower().Contains(locationPeriod.HolderSurname.ToLower())
								) ||
								(
									holder.Name.ToLower().Contains(locationPeriod.HolderName.ToLower())
									&& holder.Surname.ToLower().Contains(locationPeriod.HolderSurname.ToLower())
								))
								&& ev.EventTime >= startTime
								&& ev.EventTime <= endTime

						orderby ev.EventTime descending 

						select new {
							Event55 = ev,
							StartZone = startZone55,
							TargetZone = targetZone55,
							ControlPoint = controlPoint,
							Holder = holder
						};

			var holderEvents = await query.Take(2000).ToListAsync(cancellationToken);

			// create and fill events information
			var eventsInfos = new List<EventInfoDTO>();
			foreach(var eventInfo in holderEvents) {
				eventsInfos.Add(new EventInfoDTO() {
					Direction = eventInfo?.Event55?.Direction,
					EventCode = eventInfo?.Event55?.EventCode,
					EventTime = eventInfo?.Event55?.EventTime.ToUniversalTime(),

					StartAreaName = eventInfo?.StartZone?.colName,
					TargetAreaName = eventInfo?.TargetZone?.colName,

					ObjectType = eventInfo?.ControlPoint?.colType,
					ObjectName = eventInfo?.ControlPoint?.colName
				});
			}
			
			return new HolderLocationDTO() {
				TimePeriod = locationPeriod.TimePeriod,
				EventsInfo = eventsInfos,
				HolderInfo = (holderEvents?.Count == 0) // fill Info only about holder. Other fields is null
								? new EventDTO() 
								: new EventDTO() {
									CardNumber = holderEvents[0]?.Event55?.CardNumber,
									HolderType = holderEvents[0]?.Holder?.HolderType,
									HolderName = holderEvents[0]?.Holder?.Name,
									HolderMiddlename = holderEvents[0]?.Holder?.Middlename,
									HolderSurname = holderEvents[0]?.Holder?.Surname,
									HolderDepartment = holderEvents[0]?.Holder?.Department,
									HolderTabNumber = holderEvents[0]?.Holder?.TabNumber,
									HolderPhoto = holderEvents[0]?.Holder?.Photo
								}
				};
		}

		#region IDisposable Support

		private bool disposedValue = false;

		protected virtual void Dispose(bool disposing) {
			if(!disposedValue) {
				if(disposing) {
					unitOfWork?.Dispose();
					unitOfWork = null;
				}
				Log.Trace($"Disposed: {ToString()}");
				disposedValue = true;
			}
		}

		public void Dispose() {
			Dispose(true);
		}

		#endregion IDisposable Support
	}
}
