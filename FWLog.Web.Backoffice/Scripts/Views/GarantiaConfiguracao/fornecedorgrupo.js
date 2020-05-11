
/* gravar banco dados */
function GravarFornecedorGrupo() {
    RegistroInclusao.Inclusao = [];

    var registro = new Object();
    registro.Cod_Forn_Pai = $("#spanIdFornecedorPai").html() == "" ? 0 : $("#spanIdFornecedorPai").html();
    registro.Cod_Forn_Filho = $("#spanIdFornecedorFilho").html() == "" ? 0 : $("#spanIdFornecedorFilho").html();

    RegistroInclusao.Inclusao.push(JSON.stringify(registro));
    CancelarFornecedorGrupo();
    RegistroIncluir();    
}

/* cancelar */
function CancelarFornecedorGrupo() {
    $("#txtFornecedorPai").val("");
    $("#spanIdFornecedorPai").html("");
    $("#spanFornecedorPai").html("");
    $("#txtFornecedorFilho").val("");
    $("#spanIdFornecedorFilho").html("");
    $("#spanFornecedorFilho").html("");
}

/* autocomplete PAI */
$("#txtFornecedorPai").autocomplete({
    lookup: function (query, done) {
        var AutoComplete = new Object();
        AutoComplete.tag = TagPadrao;
        AutoComplete.tagAutoComplete = "Fornecedor";
        AutoComplete.palavra = $("#txtFornecedorPai").val();

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

    onSelect: function (retorno) {
        $("#spanIdFornecedorPai").html(retorno.data);
        $("#spanFornecedorPai").html(retorno.value);
        $("#txtFornecedorPai").val("");
    }
});

/* autocomplete FILHO */
$("#txtFornecedorFilho").autocomplete({
    lookup: function (query, done) {
        var AutoComplete = new Object();
        AutoComplete.tag = TagPadrao;
        AutoComplete.tagAutoComplete = "Fornecedor";
        AutoComplete.palavra = $("#txtFornecedorFilho").val();

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

    onSelect: function (retorno) {
        $("#spanIdFornecedorFilho").html(retorno.data);
        $("#spanFornecedorFilho").html(retorno.value);
        $("#txtFornecedorFilho").val("");
    }
});