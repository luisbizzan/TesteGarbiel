
/* variáveis */
var _remessaUsuarioGravar = [];
var liRemessaUsuario = '<li id="{data}"><p><a class="btn btn-link" onclick="RemessaUsuarioRemoverLista(*{data}*);" data-toggle="tooltip" data-placement="top" data-original-title="Remover"><b><i class="fa fa-trash-o text-danger"></i></b></a>  <b> {value}</b></p></li>';

/* adicionar na lista */
function RemessaUsuarioAdicionarLista(usuario) {
    if (jQuery.inArray(usuario.data, _remessaUsuarioGravar) == -1) {
        _remessaUsuarioGravar.push(usuario.data);
        $("#listaRemessaUsuarios").append(liRemessaUsuario.replace("{data}", usuario.data).replace("{data}", usuario.data).replace("{value}", usuario.value).replace("*", "'").replace("*", "'"));
    }
    $("#txtRemessaUsuario").val("");
}

/* remover da lista */
function RemessaUsuarioRemoverLista(IdUsuario) {
    _remessaUsuarioGravar = jQuery.grep(_remessaUsuarioGravar, function (value) {
        return value != IdUsuario;
    });
    $("#" + IdUsuario).remove();
}

/* gravar no banco */
function GravarRemessaUsuario() {
    RegistroInclusao.Inclusao = [];

    $.each(_remessaUsuarioGravar, function (i, item) {
        var registro = new Object();
        registro.Id_Usr = item;
        registro.Id_Empresa = $('#ddlEmpresa option:selected')[0].value;

        RegistroInclusao.Inclusao.push(JSON.stringify(registro));
    });

    CancelarRemessaUsuario();
    RegistroIncluir();
}

/* cancelar */
function CancelarRemessaUsuario() {
    _listaAutoComplete = [];
    _remessaUsuarioGravar = [];
    $("#listaRemessaUsuarios").html("");
}

/* autocomplete */
$("#txtRemessaUsuario").autocomplete({
    lookup: function (query, done) {

        var AutoComplete = new Object();
        AutoComplete.tag = TagPadrao;
        AutoComplete.palavra = $("#txtRemessaUsuario").val();

        $.ajax({
            url: "/GarantiaConfiguracao/AutoComplete",
            global: false,
            method: "POST",
            data: { autocomplete: AutoComplete },
            success: function (s) {
                _listaAutoComplete = [];
                if (s.Success) {
                    $.each(s.Lista, function (e, item) {
                        var registro = new Object();
                        registro.data = item.Data;
                        registro.value = item.Value;
                        _listaAutoComplete.push(registro);
                    });
                }
                else {
                    Mensagem(s.Success, s.Message);
                }
            },
            error: function (e) {
                console.log(e);
            }
        });

        var result = {
            suggestions: _listaAutoComplete
        };

        _listaAutoComplete = [];
        done(result);
    },

    onSelect: function (item) {
        RemessaUsuarioAdicionarLista(item);
        $("#txtRemessaUsuario").val("");
    }
});