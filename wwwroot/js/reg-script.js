var connection = new signalR.HubConnectionBuilder().withUrl("/authhub").build();

let inp = $('#UserName');
inp.on('change', function () {

    if ($(this).val() !== "") connection.invoke('CheckAvailability', $(this).val());
    else return;

});
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

connection.start();