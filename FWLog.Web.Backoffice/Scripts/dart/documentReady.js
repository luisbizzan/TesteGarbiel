//prevent submit on enter 
$(document).ready(function () {
    $('form').keydown(function (event) {
        if (event.keyCode === 13) {
            let target = $(event.target);
            if (target) {
                if (!target.is("textarea")) {
                    event.preventDefault();
                    return false;
                } else {
                    return true;
                }
            }
            event.preventDefault();
            return false;
        }
    });
});

$(document).ajaxStart(function () {
    waitingDialog.show("Carregando dados...");
});

$(document).ajaxStop(function () {
    waitingDialog.hide();
});

$(document).ajaxError(function (event, jqxhr, settings, thrownError) {
    new PNotify({
        title: 'Erro',
        text: 'Chamada Ajax com problema',
        type: 'error'
    });
    let http = new HttpService();
    let urlLog = HOST_URL + "ApplicationLog/LogBadAjaxCall";
    let data = { url: settings.url };
    http.post(urlLog, data, false)
        .then(dados => console.log("Criado log de erro para chamada ajax!"))
        .catch(err => console.log("Erro ao tentar gerar log de erros! "));
});
