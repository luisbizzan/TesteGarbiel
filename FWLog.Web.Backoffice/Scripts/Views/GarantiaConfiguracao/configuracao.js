
/* gravar no banco */
function GravarConfiguracao() {
    RegistroInclusao.Inclusao = [];

    var registro = new Object();
    registro.Id_Filial_Sankhya = $("#spanConfigIdFilial").html() == "" ? 0 : $("#spanConfigIdFilial").html();
    registro.Filial = $("#spanConfigFilial").html();
    registro.Pct_Estorno_Frete = $("#txtConfigEstFrete").val() == "" ? 0 : $("#txtConfigEstFrete").val();
    registro.Pct_Desvalorizacao = $("#txtConfigDesvalorizacao").val() == "" ? 0 : $("#txtConfigDesvalorizacao").val();
    registro.Vlr_Minimo_Envio = $("#txtConfigMinimo").val() == "" ? 0 : $("#txtConfigMinimo").val().replace(".", "").replace(",", ".");
    registro.Prazo_Envio_Automatico = $("#txtConfigPrazoEnvio").val() == "" ? 0 : $("#txtConfigPrazoEnvio").val();
    registro.Prazo_Descarte = $("#txtConfigPrazoDescarte").val() == "" ? 0 : $("#txtConfigPrazoDescarte").val();

    RegistroInclusao.Inclusao.push(JSON.stringify(registro));

    RegistroIncluir();
    CancelarConfiguracao();
}

/* cancelar */
function CancelarConfiguracao() {
    RegistroInclusao.Inclusao = [];
    _listaAutoComplete = [];
    $("#spanConfigIdFilial").html("");
    $("#spanConfigFilial").html("");
    $("#txtConfigEstFrete").val("");
    $("#txtConfigDesvalorizacao").val("");
    $("#txtConfigMinimo").val("");
    $("#txtConfigPrazoEnvio").val("");
    $("#txtConfigPrazoDescarte").val("");
}

/* autocomplete - FILIAL */
$("#txtConfigFilial").autocomplete({
    lookup: function (query, done) {
        var AutoComplete = new Object();
        AutoComplete.tag = TagPadrao;
        AutoComplete.tagAutoComplete = "Filial";
        AutoComplete.palavra = $("#txtConfigFilial").val();

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
                        registro.registro = item;
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

    onSelect: function (config) {
        CancelarTudo();
        $("#spanConfigIdFilial").html(config.data);
        $("#spanConfigFilial").html(config.value);
        $("#txtConfigFilial").val("");

        if (config.registro.RegistroCadastrado) {
            $("#txtConfigEstFrete").val(config.registro.Pct_Estorno_Frete);
            $("#txtConfigDesvalorizacao").val(config.registro.Pct_Desvalorizacao);
            $("#txtConfigMinimo").val(config.registro.Vlr_Minimo_EnvioView);
            $("#txtConfigPrazoEnvio").val(config.registro.Prazo_Envio_Automatico);
            $("#txtConfigPrazoDescarte").val(config.registro.Prazo_Descarte);
        }
    }
});