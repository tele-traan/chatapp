var connection = new signalR.HubConnectionBuilder().withUrl("/authhub").build();

let inp = $("#UserName");

connection.start();