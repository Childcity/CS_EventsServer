@echo off

::Stop server
sc stop CS_EventsServerSvc
sc delete CS_EventsServerSvc
