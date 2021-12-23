var connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();
connection.start();

var pinger = new signalR.HubConnectionBuilder().withUrl("/pinghub").build();
pinger.start();

document.getElementById("btn").addEventListener("click", (e) => {
   e.preventDefault();
   let input = document.getElementById("msg");
   let msg = input.value;
   if (msg == "" || msg == " " || msg == null) {
            document.getElementById("msg").style.borderColor = "red";
            alert("Сообщение не может быть пустым");
            return;
        }
        document.getElementById("msg").style.borderColor = "gray";
        input.value = "";
        connection.invoke("NewMessage", msg);
    });
    connection.on("NewMessage", (time, sender, message) => {
        let elem = document.createElement("p");
        elem.innerText = `${time} ${sender}: ${message}`;
        let firstElem = document.getElementById("messages").firstChild;
        document.getElementById("messages").insertBefore(elem, firstElem);
    });
    connection.on("MemberJoined", username => {
        let elem = document.createElement("p");
        elem.style.backgroundColor = "lightgreen";
        elem.innerText = `Пользователь ${username} подключился к чату`;
        let firstElem = document.getElementById("messages").firstChild;
        document.getElementById("messages").insertBefore(elem, firstElem);

        let p = document.createElement("p");
        p.setAttribute("id", `${username}`);
        p.textContent = `Пользователь ${username}`;
        document.getElementById("users").appendChild(p);
    });
    connection.on("MemberLeft", username => {
        let elem = document.createElement("p");
        elem.style.backgroundColor = "red";
        elem.innerText = `Пользователь ${username} покинул чат`;
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