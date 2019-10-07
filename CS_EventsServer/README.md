# CS_EventsServer
Client, that can connect to [Server](https://github.com/Childcity/CS_EventsNotifierSlackBot) by WebSocket and send events, that occurred on DB.

- It Up [SqlTableDependency](https://www.nuget.org/packages/SqlTableDependency/) and listen table changes. If there are changes, it react on them, filter needed information and send it to [Server](https://github.com/Childcity/CS_EventsNotifierSlackBot).

- Can receive commands from Server.

## How it works
![scheme](./doc/Scheme.png)