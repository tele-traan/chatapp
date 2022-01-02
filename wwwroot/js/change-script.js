var connection = new signalR.HubConnectionBuilder().withUrl("/authhub");
connection.start();

let nameInput = document.getElementById("UserName");
let passInput = document.getElementById("Password");

connection.on("Result", response => {
    if (response == "ok") {
        nameInput.style.borderColor = "lawngreen";
        nameInput.style.borderRadius = "1px";
    }
    else if (response == "same") {
        nameInput.style.borderColor = "red";
        nameInput.style.borderRadius = "1px";
        nameInput.value = "";
        alert("Новый и предыдущий ники не могут совпадать");
    } else {
        nameInput.style.borderColor = "red";
        nameInput.style.borderRadius = "1px";
        nameInput.value = "";
    }
});

nameInput.addEventListener("change", () => {
    if (nameInput.value = "") return;
    connection.invoke("CheckChange", nameInput);
});