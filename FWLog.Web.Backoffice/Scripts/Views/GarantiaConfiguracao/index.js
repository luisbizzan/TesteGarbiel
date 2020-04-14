/* variaveis fornecedor quebra */
var botaoLimpar = $("#btnLimpar")[0]; var botaoGravar = $("#btnGravar")[0];
var inputCodigoFornecedor = $("#Cod_Fornecedor")[0]; var ulFornecedores = $("#listaFornecedor")[0];
var _fornecedoresGravar = []; var _fornecedoresAutoComplete = [];
var li = '<li id="{id}"><p><button type="button" class="btn btn-danger" onclick="RemoverFornecedorQuebra({data1});">' +
    '<i class="fa fa-trash-o" alt="Remover Item"></i></button>  <b>[{data0}]  {value}</b></p></li>';
FornecedorQuebraListar();

/* gravar novo fornecedor quebra */
$(botaoGravar).click(function (e) {
    var _fornecedor = new Object();
    _fornecedor.Id = 0;
    _fornecedor.Codigos = _fornecedoresGravar;

    $.post("/GarantiaConfiguracao/FornecedorQuebraIncluir", { fornecedor: _fornecedor }, function (s) {
        if (s.Success) {
            PNotify.success({ text: s.Message });
            _fornecedoresGravar = [];
            $(ulFornecedores).html("");
            FornecedorQuebraListar();
        }
        else {
            PNotify.error({ text: s.Message });
        }
    }).fail(function (f) {
        console.log(f);
    }).done(function (d) {
        console.log(d);
    });
});

/* botão limpar lista */
$(botaoLimpar).click(function () {
    $(ulFornecedores).html("");
});

/* autocomplete fornecedor quebra */
$(inputCodigoFornecedor).autocomplete({
    lookup: function (query, done) {

        $.ajax({
            url: "/GarantiaConfiguracao/FornecedorQuebraAutoComplete",
            global: false,
            method: "POST",
            data: {
                valor: $(inputCodigoFornecedor).val()
            },
            success: function (s) {
                if (s.Success) {
                    $.each(s.Fornecedores, function (e, item) {
                        var registro = new Object();
                        registro.data = item.Id;
                        registro.value = item.Value;
                        _fornecedoresAutoComplete.push(registro);
                    });
                }
                else {
                    PNotify.error({ text: s.Message });
                }
            },
            error: function (e) {
                console.log(e);
            }
        });

        var result = {
            suggestions: _fornecedoresAutoComplete
        };

        _fornecedoresAutoComplete = [];
        done(result);
    },

    onSelect: function (item) {
        AdicionarFornecedorQuebra(item);
        $(inputCodigoFornecedor).val("");
    }
});

/* adicionar fornecedor quebra */
function AdicionarFornecedorQuebra(fornecedor) {
    _fornecedoresGravar.push(fornecedor.data);
    $(ulFornecedores).append(li.replace("{value}", fornecedor.value).replace("{id}", fornecedor.data).replace("{data0}", fornecedor.data).replace("{data1}", fornecedor.data));
}

/* remover fornecedor quebra */
function RemoverFornecedorQuebra(cnpj) {
    _fornecedoresGravar = jQuery.grep(_fornecedoresGravar, function (value) {
        return value != cnpj;
    });
    $("#" + cnpj).remove();
}

/* listar fornecedor quebra */
function FornecedorQuebraListar() {
    $.get("/GarantiaConfiguracao/FornecedorQuebraListar", function (s, status) {
        if (s.Success) {
            $("#gridFornecedorQuebra").DataTable({
                destroy: true,
                serverSide: false,
                data: s.Data,
                columns: [
                    { data: "BotaoEvento" },
                    { data: "Id", title: "Id Registro" },
                    { data: "Cod_Fornecedor", title: "CNPJ" },
                    { data: "NomeFantasia", title: "Nome Fantasia" },
                    { data: "RazaoSocial", title:"Razão Social" },
                ],
            });
        }
        else {
            PNotify.error({ text: s.Message });
        }
    });
}

/* excluir fornecedor quebra  */
function FornecedorQuebraExcluir(IdSelecionado) {
    $.post("/GarantiaConfiguracao/FornecedorQuebraExcluir", { Id: IdSelecionado }, function (s) {
        if (s.Success) {
            PNotify.success({ text: s.Message });
            ListarFornecedorQuebra();
        }
        else {
            PNotify.error({ text: s.Message });
        }
    }).fail(function (f) {
        console.log(f);
    }).done(function (d) {
        console.log(d);
    });
    
}