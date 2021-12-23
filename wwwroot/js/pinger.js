var pinger = new signalR.HubConnectionBuilder().withUrl("/pinghub").build();
pinger.start();