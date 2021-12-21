var connection = new signalR.HubConnectionBuilder().withUrl("/authhub").build();
connection.start();

let input = document.getElementById("UserName");

connection.on("Result", result => {
    let submitBtn = document.getElementById("btnreg");
    if (result == "available") {

        input.style.border = "1px solid lawngreen";
        submitBtn.setAttribute("type", "submit");

    }
    else
    {
        input.style.borderColor = "red";
        submitBtn.setAttribute("type", "");
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