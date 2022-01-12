var connection = new signalR.HubConnectionBuilder().withUrl("/authhub").build();

let changeNameInput = $('input[name="username"]');
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


changeNameInput.on("change", function (e) {
    if (changeNameInput.val() === "") {
        nameInputValidation.html('<p style="color:red;">Введите новый никнейм</p>');
    }
    if (changeNamePass.val() === "") {
        passInputValidation.html('<p style="color:red;">Подтвердите пароль</p>');
    }
    if ($(this).val() !== "") connection.invoke("CheckChange", $(this).val());
});


changeNameBtn.on("click", function (e) {
    e.preventDefault();
    changeNameSummarize.html("");

    let newUsername = changeNameInput.val();
    let password = changeNamePass.val();

    if (newUsername != "" && password != "") {
        connection.invoke("ChangeUsername", newUsername, password);
    } else if (newUsername === "") {
        nameInputValidation.text("Введите никнейм");
        return;
    } else {
        passInputValidation.text("Введите пароль");
        return;
    }

    changeNameInput.val("");
    changeNamePass.val("");
});

changePassBtn.on("click", function (e) {
    e.preventDefault();
    changePassSummarize.html("");

    let oldPass = changePassOld.val();
    let newPass = changePassNew.val();

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

    changePassOld.val("");
    changePassNew.val("");
});

connection.start();