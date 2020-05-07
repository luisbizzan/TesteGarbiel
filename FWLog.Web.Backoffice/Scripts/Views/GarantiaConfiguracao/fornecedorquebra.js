
/* variaveis */
var _fornecedoresGravar = [];
var liFornecedorQuebra = '<li id="{id}"><p><a onclick="FornecedorQuebraRemoverLista({data});" class="btn btn-link"><b><i class="fa fa-trash-o text-danger"></i></b></a><span class="label label-primary"> {value}</span></p></li>';

/* autocomplete */
$("#Cod_Fornecedor").autocomplete({
    lookup: function (query, done) {
        var AutoComplete = new Object();
        AutoComplete.tag = TagPadrao;
        AutoComplete.palavra = $("#Cod_Fornecedor").val();

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

    onSelect: function (item) {
        FornecedorQuebraAdicionarLista(item);
        $("#Cod_Fornecedor").val("");
    }
});

/* adicionar na lista */
function FornecedorQuebraAdicionarLista(fornecedor) {
    if (jQuery.inArray(fornecedor.data, _fornecedoresGravar) == -1) {
        _fornecedoresGravar.push(fornecedor.data);
        $("#listaFornecedor").append(liFornecedorQuebra.replace("{value}", fornecedor.value).replace("{id}", fornecedor.data).replace("{data}", fornecedor.data));
    }

    $("#controleFornecedor").attr("style", (_fornecedoresGravar.length > 0) ? "display:normal;" : "display:none;");
}

/* remover da lista */
function FornecedorQuebraRemoverLista(cnpj) {
    _fornecedoresGravar = jQuery.grep(_fornecedoresGravar, function (value) {
        return value != cnpj;
    });
    $("#" + cnpj).remove();

    $("#controleFornecedor").attr("style", (_fornecedoresGravar.length > 0) ? "display:normal;" : "display:none;");
}

/* gravar no banco */
function GravarFornecedorQuebra() {
    RegistroInclusao.Inclusao = [];

    $.each(_fornecedoresGravar, function (i, item) {
        var registro = new Object();
        registro.Cod_Fornecedor = item;

        RegistroInclusao.Inclusao.push(JSON.stringify(registro));
    });

    _fornecedoresGravar = [];
    $("#listaFornecedor").html("");

    RegistroIncluir();
    CancelarFornecedorQuebra();
}

/* cancelar */
function CancelarFornecedorQuebra() {
    RegistroInclusao.Inclusao = [];
    _fornecedoresGravar = [];
    _listaAutoComplete = [];
    $("#listaFornecedor").html("");
}