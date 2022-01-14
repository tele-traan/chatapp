const form = document.forms['delaccform'];

form.elements['delaccsubmit'].addEventListener('click', e => {
    e.preventDefault();
    document.getElementById('delaccconfirmspan')
        .innerHTML = '<p>Вы уверены, что хотите удалить свой аккаунт навсегда без возможности восстановления?</p>'
        + '<button id="delacconfirmbtn" class="btn btn-outline-danger">Уверен</button>';
    document.getElementById('delaccconfirmbtn').addEventListener('click', e => form.submit());
});