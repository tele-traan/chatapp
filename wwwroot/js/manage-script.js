var connection = new signalR.HubConnectionBuilder().withUrl("/authhub");
connection.start();

let changeNameInput = $('input[name="userName"]');
let nameInputValidation = $('#changeusername-validation-username');
let changeNamePass = $('input[name="password"]')
let passInputValidation = $('#changeusername-validation-password');

let changeNameBtn = $('#changeusername-button');
let changeNameSummarize = $('#changeusername-summarize');

let changePassOld = $('input[name="oldPassword"]');
let oldPassValidation = $('#changepassword-validation-oldpassword');
let changePassNew = $('input[name="newPassword"]');
let newPassValidation = $('#changepassword-validation-newpassword');

let changePassBtn = $('#changepassword-button');
let changePassSummarize = $('#changepassword-summarize');

changeNameBtn.on("click", function (e) {
    e.preventDefault();
    changeNameSummarize.text("");

    let newUsername = changeNameInput.val();
    let password = changeNamePass.val();

    changeNameInput.val("");
    changeNamePass.val("");

    if (newUsername !== "" && password !== "") {
        connection.invoke("ChangeUsername", newUsername, password);
    } else if (newUsername === "") {
        nameInputValidation.text("Введите никнейм");
    } else {
        passInputValidation.text("Введите пароль");
    }
});

changePassBtn.on("click", function (e) {
    e.preventDefault();
    changePassSummarize.html("");

    let oldPass = changePassOld.val();
    let newPass = changePassNew.val();

    changePassOld.val("");
    changePassNew.val("");

    if (newPass === oldPass) {
        changePassSummarize.html('<p style="color:red;">Новый и старый пароли не должны совпадать</p>');
        return;
    }
    if (oldPass === "") {
        oldPassValidation.text("Вы не ввели пароль");
        return;
    } if (newPass === "") {
        newPassValidation.text("Вы не ввели пароль");
        return;
    }
    connection.invoke("ChangePassword", oldPass, newPass);
});

connection.on("ChangeUsernameResult", response => {
    if (response == "success") changeNameSummarize.html('<p style="color:lawngreen;">Имя успешно изменено</p>');
    else if (response == "failure") changeNameSummarize.html('<p style="color:red;">Произошла ошибка. Попробуйте ещё раз</p>');
    else alert("Ошибка");
});

connection.on("ChangePasswordResult", response => {
    if (response == "success") changePassSummarize.html('<p style="color:lawngreen;">Пароль успешно изменён</p>');
    else if (response == "failure") changePassSummarize.html('<p style="color:red;">Произошла ошибка. Попробуйте ещё раз</p>');
    else alert("Ошибка");
});