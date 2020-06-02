document.addEventListener("DOMContentLoaded", function (event) {
    validarRemessaAutomatica();
});

function validarRemessaAutomatica() {
    $.ajax({
        url: "/Garantia/RemessaAutomaticaProcessarUsr",
        method: "POST",
        success: function (result) {
            if (result.Success) {
            } else {
                $.confirm({
                    type: 'red',
                    theme: 'material',
                    closeIcon: function () {
                        return false;
                    },
                    title: 'Remessa Automática',
                    content: result.Message,
                    typeAnimated: true,

                    buttons: {
                        confirmar: {
                            text: 'Conferir',
                            btnClass: 'btn-red',
                            action: function () {
                                window.location.href = "/Garantia/Remessa";
                            }
                        }
                    }
                });
            }
        },
        error: function (request, status, error) {
            PNotify.warning({ text: result.Message });
        }
    });
}