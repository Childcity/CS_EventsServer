@echo off

::Start server as service
sc create CS_EventsServerSvc binpath=%~dp0CS_EventsServer.exe
sc description CS_EventsServerSvc "Service for CS_EventsServerSvc"

sc start CS_EventsServerSvc
::pause