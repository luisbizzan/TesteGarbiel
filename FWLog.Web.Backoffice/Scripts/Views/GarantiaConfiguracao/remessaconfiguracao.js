
/* gravar no banco */
function GravarRemessaConfiguracao() {
    RegistroInclusao.Inclusao = [];

    var registro = new Object();
    registro.Cod_Fornecedor = $("#spanIdFornecedor").html() == "" ? 0 : $("#spanIdFornecedor").html();
    registro.Automatica = $("#chkRCAutomatica")[0].checked ? 1 : 0;
    registro.Vlr_Minimo = $("#txtRCMinimoEnvio").val() == "" ? 0 : $("#txtRCMinimoEnvio").val().replace(".", "").replace(",", ".");
    registro.Total = $("#chkRCTotal")[0].checked ? 1 : 0;
    RegistroInclusao.Inclusao.push(JSON.stringify(registro));
    CancelarRemessaConfiguracao();
    RegistroIncluir();    
}

/* cancelar */
function CancelarRemessaConfiguracao() {
    _listaAutoComplete = [];
    $("#spanIdFilial").html("");
    $("#spanFilial").html("");
    $("#spanFornecedor").html("");
    $("#chkRCAutomatica")[0].checked = false;
    $("#txtRCMinimoEnvio").val("");
    $("#txtRCTotal").val("");
}

/* autocomplete - FORNECEDOR */
$("#txtRCFornecedor").autocomplete({
    lookup: function (query, done) {

        var AutoComplete = new Object();
        AutoComplete.tag = TagPadrao;
        AutoComplete.tagAutoComplete = "Fornecedor";
        AutoComplete.palavra = $("#txtRCFornecedor").val();

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

    onSelect: function (remessaConfig) {
        $("#spanIdFornecedor").html(remessaConfig.data);
        $("#spanFornecedor").html(remessaConfig.value);
        $("#txtRCFornecedor").val("");
    }
});

/* autocomplete - FILIAL */
$("#txtRCFilial").autocomplete({
    lookup: function (query, done) {

        var AutoComplete = new Object();
        AutoComplete.tag = TagPadrao;
        AutoComplete.tagAutoComplete = "Filial";
        AutoComplete.palavra = $("#txtRCFilial").val();

        $.ajax({
            url: "/GarantiaConfiguracao/AutoComplete",
            global: false,
            method: "POST",
            data: { autocomplete: AutoComplete },
            success: function (s) {
                if (s.Success) {
                    _listaAutoComplete = [];
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

    onSelect: function (remessaConfig) {
        $("#spanIdFilial").html(remessaConfig.data);
        $("#spanFilial").html(remessaConfig.value);
        $("#txtRCFilial").val("");
    }
});