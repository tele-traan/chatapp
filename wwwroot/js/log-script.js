var connection = new signalR.HubConnectionBuilder().withUrl("/authhub").build();
connection.start();

let inp = $("#UserName");