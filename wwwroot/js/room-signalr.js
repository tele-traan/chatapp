var connection = new signalR.HubConnectionBuilder().withUrl("/roomhub").build();

connection.on("ErrorLogging", msg => {
    alert(msg);
    window.location.href = `/Room/Index?type=connect`;
});

connection.on("NewMessage", (time, sender, msg) => {
    let p = document.createElement("p");
    p.innerText = `${time} ${sender}: ${msg}`;
    let firstElem = document.getElementById("messages").firstChild;
    document.getElementById("messages").insertBefore(p, firstElem);
});
connection.on("ThisAccOnNewTab", () => {
    alert("Один аккаунт может находиться в чате только в одной вкладке");
    window.location.href = "/Home/Index";
});
connection.on("MemberJoined", (memberName,isAdmin) => {
    let elem = document.createElement("p");
    elem.style.backgroundColor = "lightgreen";
    if (isAdmin) {
        elem.textContent = `Админ ${memberName} подключился к комнате`;
    } else elem.textContent = `Пользователь ${memberName} подключился к комнате`;
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

connection.on("MemberLeft", (memberName, isAdmin) => {
    let p = document.createElement("p");
    p.style.backgroundColor = "crimson";
    if (isAdmin) {
        p.innerText = `Админ ${memberName} покинул комнату`;
    } else p.innerText = `Пользователь ${memberName} покинул комнату`;
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

connection.on("RoomDeleted", () => {
    alert("Эта комната удалена её создателем.");
    window.location.href = "/Home/Index";
});

connection.start();