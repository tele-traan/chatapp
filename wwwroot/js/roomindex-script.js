let form = document.forms['createroomform'];
if (form !== undefined) {
    let roomPassword = $('input[name="RoomPassword"]');
    roomPassword.fadeOut(1);
    $('input[name="IsPrivate"]').on('change', function (e) {
        if ($(this).prop('checked')) {
            roomPassword.fadeIn(1);
        } else roomPassword.fadeOut(1);
    });
}