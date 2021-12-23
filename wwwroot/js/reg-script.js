var connection = new signalR.HubConnectionBuilder().withUrl("/authhub").build();
connection.start();

let input = document.getElementById("UserName");
let submitBtn = document.getElementById("btnreg");
let handler = submitBtn.onclick;
connection.on("Result", result => {
    if (result == "available") {

        input.style.border = "1px solid lawngreen";
        submitBtn.setAttribute("type", "submit");
        submitBtn.onclick = handler;
    }
    else
    {
        input.style.borderColor = "red";
        submitBtn.setAttribute("type", "");
        handler = submitBtn.onclick;
        alert("Это имя пользователя уже занято");
    }
});
let btn = document.getElementById("btnreg");

    btn.addEventListener("click", e => {
        let condition = document.getElementById("Password").value != ""
            && document.getElementById("Password").value == document.getElementById("password-repeat").value;
        if (!condition) {
            e.preventDefault();
            alert("Пароли не совпадают!");
            document.getElementById("Password").value = "";
            document.getElementById("password-repeat").value = "";
        }

    });
input.addEventListener("change", () => {

    let userName = input.value;
    if (userName == "") return;
    connection.invoke("Check", userName);

});