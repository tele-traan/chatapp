var connection = new signalR.HubConnectionBuilder().withUrl("/roomhub").build();

connection.on("ErrorLogging", msg => {
    alert(msg);
    window.location.href = `/Home/Index`;
});

connection.on("NewMessage", (time, sender, msg) => {
    let p = document.createElement("p");
    p.innerText = `${time} ${sender}: ${msg}`;
    let firstElem = document.getElementById("messages").firstChild;
    document.getElementById("messages").insertBefore(p, firstElem);
});
connection.on("SystemMessage", (color, message) => {
    let elem = document.createElement("p");
    elem.innerText = message;
    elem.style.color = color;
    let div = document.getElementById("messages");
    div.insertBefore(elem, div.firstChild);
});
connection.on("ThisAccOnNewTab", () => {
    alert("Один аккаунт может находиться в чате только в одной вкладке");
    window.location.href = "/Home/Index";
});
connection.on("MemberJoined", (memberName, isAdmin, time) => {
    let elem = document.createElement("p");
    elem.style.backgroundColor = "lightgreen";
    if (isAdmin) {
        elem.textContent = `${time} Админ ${memberName} подключился к комнате`;
    } else elem.textContent = `${time} Пользователь ${memberName} подключился к комнате`;
    let firstElem = document.getElementById("messages").firstChild;
    document.getElementById("messages").insertBefore(elem, firstElem);

    let p = document.createElement("p");
    p.setAttribute("id", `${memberName}`);
    if (isAdmin) {
        p.textContent = `Админ ${memberName}`;
        p.style.color = "crimson";
    } else p.textContent = `${memberName}`;
    document.getElementById("users").appendChild(p);
});
connection.on("MemberLeft", (memberName, isAdmin, time) => {
    let p = document.createElement("p");
    p.style.backgroundColor = "crimson";
    if (isAdmin) {
        p.innerText = `${time} Админ ${memberName} покинул комнату`;
    } else p.innerText = `${time} Пользователь ${memberName} покинул комнату`;
    let firstElem = document.getElementById("messages").firstChild;
    document.getElementById("messages").insertBefore(p, firstElem);

    let elem = document.getElementById(`${memberName}`);
    elem.parentNode.removeChild(elem);
});

connection.on("UserKicked", admin => {
    $('#render').fadeOut();
    connection.stop();
    alert(`Администратор ${admin} кикнул вас из комнаты. Переадресация через 5 секунд...`);
    setTimeout(() => window.location.href = "/Home/Index", 5000);
});

connection.on("UserBanned", (admin, reason, time) => {
    $('#render').fadeOut();
    connection.stop();
    alert(`Администратор ${admin} забанил вас в этой комнате до ${time} по причине: "${reason}". Переадресация через 5 секунд...`);
    window.location.href = "/Home/Index";
});

connection.on("UserOpped", creator => {
    alert(`Создатель комнаты ${creator} назначил вас администратором`);
});

connection.on("UserDeopped", creator => {
    alert(`Создатель комнаты ${creator} отнял у вас права администратора`);
});

document.getElementById("btnsendmsg").addEventListener("click", e => {
    e.preventDefault();
    let msg = document.getElementById("msg").value;
    if (msg != null && msg != "" && msg != " ") {
        connection.invoke("NewMessage", msg);
        document.getElementById("msg").value = "";
    } else {
        alert("Сообщение не может быть пустым");
        return;
    }
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

connection.on("RoomDeleted", () => {
    alert("Эта комната удалена её создателем.");
    window.location.href = "/Home/Index";
});

connection.start();