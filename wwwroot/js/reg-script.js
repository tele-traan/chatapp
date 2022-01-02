/*
let input = document.getElementById("UserName");
let submitBtn = document.getElementById("btnreg");
let handler = submitBtn.onclick;
connection.on("Result", result => {
    if (result == "available") {
        input.style.border = "1px solid lawngreen";
        input.setAttribute("placeholder", "Ваш ник");
        submitBtn.onclick = handler;
    }
    else
    {
        input.style.border = "1px solid red";
        submitBtn.onclick = null;
        input.value = "";
        input.setAttribute("placeholder", "Этот никнейм занят");
    }
});

input.addEventListener("change", () => {

    let userName = input.value;
    if (userName == "") return;
    connection.invoke("Check", userName);

});*/

$(function () {
    var connection = new signalR.HubConnectionBuilder().withUrl("/authhub").build();
    connection.start();
    let inp = $('input[name="UserName"]');
    connection.on("Result", result => {
        if (result == "available") {
            inp.css('border', '1px solid lawngreen');
            inp.attr("placeholder", "Ваш ник");
        }
        
        else {
            inp.css('border', '1px solid red');
            inp.val("")
            inp.attr("placeholder", "Этот никнейм занят");
        }
    });

    $('input[name="show-pass"]').on("click", function () {
        if ($(this).prop("checked")) {
            $('input[name="Password"').attr('type', 'text');
        } else $('input[name=Password').attr('type', 'password');
    });

    $('input[name="show-pass-confirm"]').on("click", function () {
        if ($(this).prop("checked")) {
            $('input[name="ConfirmPassword"').attr('type', 'text');
        } else $('input[name=ConfirmPassword').attr('type', 'password');
    });
});