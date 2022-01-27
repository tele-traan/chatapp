var connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();

document.getElementById("btnsendmsg").addEventListener("click", (e) => {
   e.preventDefault();
   let input = document.getElementById("msg");
   let msg = input.value;
   if (msg == "" || msg == " " || msg == null) return;
   document.getElementById("msg").style.borderColor = "gray";
   input.value = "";
   connection.invoke("NewMessage", msg);
});
connection.on("ThisAccOnNewTab", () => {
    alert("Один аккаунт может находиться в чате только в одной вкладке");
    window.location.href = "/Home/Index";
});
connection.on("NewMessage", (time, sender, message) => {
    let elem = document.createElement("p");
    elem.innerText = `${time} ${sender}: ${message}`;
    let firstElem = document.getElementById("messages").firstChild;
    document.getElementById("messages").insertBefore(elem, firstElem);
});
connection.on("SystemMessage", (message,color) => {
    let elem = document.createElement("p");
    elem.innerText = message;
    elem.style.color = color;
    let div = document.getElementById("messages");
    div.insertBefore(elem, div.firstChild);
});
connection.on("MemberJoined", (username, time) => {
    let elem = document.createElement("p");
    elem.style.backgroundColor = "lightgreen";
    elem.innerText = `${time} Пользователь ${username} подключился к чату`;
    let firstElem = document.getElementById("messages").firstChild;
    document.getElementById("messages").insertBefore(elem, firstElem);

    let p = document.createElement("p");
    p.setAttribute("id", `${username}`);
    p.textContent = `Пользователь ${username}`;
    document.getElementById("users").appendChild(p);
});
connection.on("MemberLeft", (username,time) => {
    let elem = document.createElement("p");
    elem.style.backgroundColor = "red";
    elem.innerText = `${time} Пользователь ${username} покинул чат`;
    let firstElem = document.getElementById("messages").firstChild;
    document.getElementById("messages").insertBefore(elem, firstElem);

    let p = document.getElementById(`${username}`);
    p.parentNode.removeChild(p);
});
document.addEventListener("keypress", e => {
    if (e.key == "Enter") {
        e.stopPropagation();
        var input = document.getElementById("msg");
        var msg = document.getElementById("msg").value.trim();
        if (msg != "" && msg != null && msg != " ") {
            input.value = "";
            connection.invoke("NewMessage", msg);
        }
    }
});

connection.start();
