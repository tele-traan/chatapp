var connection = new signalR.HubConnectionBuilder().withUrl("/roomhub").build();
connection.start();

connection.on("ErrorLogging", msg => {
    alert(msg);
    setTimeout(() => {
        window.location.href =
            `${window.location.protocol}://${window.location.host}`;
    }, 3500);
});
connection.on("NewMessage", (time, sender, msg) => {
    let p = document.createElement("p");
    p.innerText = `${time} ${sender}: ${msg}`;
    let firstElem = document.getElementById("messages").firstChild;
    document.getElementById("messages").insertBefore(p, firstElem);

});

connection.on("MemberJoined", memberName => {
    let p = document.createElement("p");
    p.style.backgroundColor = "lightgreen";
    p.innerText = `Пользователь ${memberName} подключился к комнате`;
    let firstElem = document.getElementById("messages").firstChild;
    document.getElementById("messages").insertBefore(p, firstElem);
});

connection.on("MemberLeft", memberName => {
    let p = document.createElement("p");
    p.style.backgroundColor = "red";
    p.innerText = `Пользователь ${memberName} покинул комнату`;
    let firstElem = document.getElementById("messages").firstChild;
    document.getElementById("messages").insertBefore(p, firstElem);
});

document.getElementById("btnsendmsg").addEventListener("click", e => {
    let msg = document.getElementById("msg").value;
    if (msg != null && msg != "" && msg != " ") {
        connection.invoke("NewMessage", msg);
        document.getElementById("msg").value = "";
    } else {
        alert("Сообщение не может быть пустым");
        return;
    }
});