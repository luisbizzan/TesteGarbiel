
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

/* AutoComplete Fornecedor */
var _fornecedores = [];
$("#Cod_Fornecedor").autocomplete({
    lookup: function (query, done) {

        $.post("/GarantiaConfiguracao/AutoCompleteFornecedor", { valor: $("#Cod_Fornecedor").val() }, function (s) {
            if (s.Success) {
                _fornecedores = [];
                $.each(s.Fornecedores, function (e, item) {
                    var registro = new Object();
                    registro.data = item.Id;
                    registro.value = item.Value;
                    _fornecedores.push(registro);
                });
            }
            else {
                PNotify.error({ text: s.Message });
                _fornecedores = [];
            }

        }).fail(function (f) {
            console.log(f);
            _fornecedores = [];
        }).done(function (d) {
            //console.log(d);
        });

        var result = {
            suggestions: _fornecedores
        };

        done(result);
    },

    onSelect: function (item) {
        console.log(item);
        //$('#selected_option').html(item.value);
    }
});