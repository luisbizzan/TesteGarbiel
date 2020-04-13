/* gravar novo fornecedor quebra */
$("#btnGravar").click(function (e) {
    var _fornecedor = new Object();
    _fornecedor.Id = 0;
    _fornecedor.Cod_Fornecedor = $("#Cod_Fornecedor").val();

    $.post("/GarantiaConfiguracao/IncluirFornecedorQuebra", { fornecedor: _fornecedor }, function (s) {
        if (s.Success) {
            PNotify.success({ text: s.Message });
            $("#Cod_Fornecedor").val("");
        }
        else {
            PNotify.error({ text: s.Message });
        }
        console.log(s);
    }).fail(function (f) {
        //console.log(f);
    }).done(function (d) {
        //console.log(d);
    });
});

/* autocomplete fornecedor quebra */
var _fornecedores = [];
$("#Cod_Fornecedor").autocomplete({
    lookup: function (query, done) {

        $.post("/GarantiaConfiguracao/AutoCompleteFornecedor", { valor: $("#Cod_Fornecedor").val() }, function (s) {
            if (s.Success) {
                $.each(s.Fornecedores, function (e, item) {
                    var registro = new Object();
                    registro.data = item.Id;
                    registro.value = item.Value;
                    _fornecedores.push(registro);
                });
            }
            else {
                PNotify.error({ text: s.Message });
            }

        }).fail(function (f) {
            console.log(f);
        }).done(function (d) {
        });

        var result = {
            suggestions: _fornecedores
        };

        _fornecedores = [];

        done(result);
    },

    onSelect: function (item) {
        AdicionarFornecedorQuebra(item);
        $("#Cod_Fornecedor").val("");
    }
});

/* adicionar fornecedor quebra */
var _listaFornecedores = [];
var li = '<li id="{id}"><p>[{data0}]  {value}  <button type="button" class="btn btn-danger" onclick="RemoverFornecedorQuebra({data1});"><i class="glyphicon glyphicon-trash" alt="Remover Item"></i></button></p></li>';
function AdicionarFornecedorQuebra(fornecedor) {
    _listaFornecedores.push(fornecedor.data);
    $("#listaFornecedor").append(li.replace("{value}", fornecedor.value).replace("{id}", fornecedor.data).replace("{data0}", fornecedor.data).replace("{data1}", fornecedor.data));
}

/* remover fornecedor quebra */
function RemoverFornecedorQuebra(cnpj) {
    _listaFornecedores = jQuery.grep(_listaFornecedores, function (value) {
        return value != cnpj;
    });
    $("#" + cnpj).remove();
}