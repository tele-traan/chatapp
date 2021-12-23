var connection = new signalR.HubConnectionBuilder().withUrl("/authhub").build();
connection.start();

let input = document.getElementById("UserName");
let submitBtn = document.getElementById("btnreg");
let handler = submitBtn.onclick;
connection.on("Result", result => {
    if (result == "available") {
        input.style.border = "1px solid lawngreen";
        input.style.borderRadius = "1px";
        submitBtn.onclick = handler;
    }
    else
    {
        input.style.borderColor = "red";
        submitBtn.onclick = null;
        alert("Это имя пользователя уже занято");
    }
});

submitBtn.addEventListener("click", e => {

    let password = document.getElementById("Password").value;
        let condition = password != ""
            && document.getElementById("Password").value == document.getElementById("password-repeat").value;
        if (!condition) {
            e.preventDefault();
            alert("Пароли не совпадают!");
            document.getElementById("Password").value = "";
            document.getElementById("password-repeat").value = "";
            return;
        }
    /*let secondCondition = password.match(/^.*(?=.{7,})(?=.*[a-zA-Z][а-яА-Я])(?=.*\d).*$/ig).length != 0;
    if (!secondCondition) {
        e.preventDefault();
        document.getElementById("Password").value = "";
        document.getElementById("password-repeat").value = "";
        alert("Пароль должен содержать от 7 символов - букв и цифр");
        return;
    }*/

    });
input.addEventListener("change", () => {

    let userName = input.value;
    if (userName == "") return;
    connection.invoke("Check", userName);

});