var connection = new signalR.HubConnectionBuilder().withUrl("/roomhub").build();
connection.start();

var pinger = new signalR.HubConnectionBuilder().withUrl("/pinghub").build();
pinger.start();

connection.on("ErrorLogging", msg => {
    alert(msg);
    setTimeout(() => {
        window.location.href =
            `/Room/RoomIndex?type=connect`;
    }, 3500);
});
connection.on("NewMessage", (time, sender, msg) => {
    let p = document.createElement("p");
    p.innerText = `${time} ${sender}: ${msg}`;
    let firstElem = document.getElementById("messages").firstChild;
    document.getElementById("messages").insertBefore(p, firstElem);

});

connection.on("MemberJoined", memberName => {
    let elem = document.createElement("p");
    elem.style.backgroundColor = "lightgreen";
    elem.innerText = `Пользователь ${memberName} подключился к комнате`;
    let firstElem = document.getElementById("messages").firstChild;
    document.getElementById("messages").insertBefore(elem, firstElem);

    let p = document.createElement("p");
    p.setAttribute("id", `${memberName}`);
    document.getElementById("users").appendChild(p);
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