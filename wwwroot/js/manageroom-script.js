$('body').fadeOut(1);
var connection = new signalR.HubConnectionBuilder().withUrl("/manageroomhub").build();

let forms = document.forms;

for (let f of forms) {

    let username = f.name;
    let summary = document.getElementById(`summary-${username}`);
    let actions = ["Op", "Deop", "Kick", "Unban"];
    for (let a of actions) {
        if (f.elements[a]!==undefined) {
            f.elements[a].addEventListener("click", function (e) {
                summary.innerText = "";
                e.preventDefault();
                connection.invoke(a, username);
            });
        }
    }
    if (f.elements["Ban"] !== undefined) {
        f.elements["Ban"].addEventListener("click", function (e) {
            e.preventDefault();
            summary.innerText = "";
            let reason = f.elements["reason"].value;
            let days = parseInt(f.elements["days"].value);
            if (reason == "") {
                summary.innerText = "Вы не ввели причину бана";
                return;
            }
            connection.invoke("Ban", username, reason, days)
        });
    }
}

connection.on("OpResult", (response, username) => {
    let p = $(`#summary-${username}`);
    if (response == "success") {
        p.text(`Пользователю ${username} выданы права администратора`);
        p.css("color", "#28a745");
    } else {
        p.text("Ошибка. Попробуйте снова");
        p.css("color", "red");
    }
});
connection.on("DeopResult", (response, username) => {
    let p = $(`#summary-${username}`);
    if (response == "success") {
        p.text(`У пользователя ${username} отняты права администратора`);
        p.css("color", "#28a745");
    } else {
        p.text("Ошибка. Попробуйте снова");
        p.css("color", "red");
    }
});
connection.on("KickResult", (response, username) => {
    let p = $(`#summary-${username}`);
    if (response == "success") {
        let form = document.forms[username];
        form.parentNode.removeChild(form);
        alert(`Пользователь ${username} кикнут из комнаты`);
    } else {
        p.text("Ошибка. Попробуйте снова");
        p.css("color", "red");
    }
});
connection.on("BanResult", (response, username) => {
    let p = $(`#summary-${username}`);
    if (response == "success") {
        let form = document.forms[username];
        form.parentNode.removeChild(form);
        alert(`Пользователь ${username} забанен`);

        let newForm = document.createElement("form");
        newForm.name = username;
        newForm.innerHTML = `<h3 style="display:inline-block;">Пользователь ${username}</h3>` +
            '<input type="button" name="Unban" class="btn btn-outline-success" value="Разбанить" />'
        document.getElementById("bannedusers").appendChild(newForm);
        newForm = document.forms[newForm.name];
        newForm.elements["Unban"].addEventListener("click", function (e) {
            e.preventDefault();
            connection.invoke("Unban", username);
        })
    } else {
        p.text("Ошибка. Попробуйте снова");
        p.css("color", "red");
    }
});
connection.on("UnbanResult", (response, username) => {
    if (response == "success") {
        let form = document.forms[username];
        form.parentNode.removeChild(form);
        alert(`Пользователь ${username} разбанен`);
    } else {
        alert(`Ошибка. Попробуйте ещё раз`);
    }
});

connection.start().then(()=>$('body').fadeIn());