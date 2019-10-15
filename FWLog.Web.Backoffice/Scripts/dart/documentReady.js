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