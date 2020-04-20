/* [FORNECEDOR QUEBRA] variaveis */
var botaoFornQuebraLimpar = $("#btnLimpar")[0];
var botaoFornQuebraGravar = $("#btnGravar")[0];
var inputCodigoFornecedor = $("#Cod_Fornecedor")[0];
var ulFornecedores = $("#listaFornecedor")[0];
var _fornecedoresGravar = []; var _fornecedoresAutoComplete = [];
var liFornecedorQuebra = '<li id="{id}"><p><button type="button" class="btn btn-danger" onclick="FornecedorQuebraRemoverLista({data1});">' +
    '<i class="fa fa-trash-o"></i></button>  <b>[{data0}]  {value}</b></p></li>';

/* [SANKHYA TOP] variaveis */
var botaoSankhyaGravar = $("#btnSankhyaGravar")[0];
var botaoSankhyaAdicionar = $("#btnSankhyaAdicionar")[0];
var inputSankhyaCodigo = $("#txtSankhyaCodigo")[0];
var inputSankhyaDescricao = $("#txtSankhyaDescricao")[0];
var _SankhyaTopLista = [];
var ulSankhyaTop = $("#listaSankhyaTop")[0];
var liSankhyaTop = '<li id="{top}"><p><button type="button" class="btn btn-danger" onclick="ShankhyaTopRemoverLista(*{top}|{descricao}*);">' +
    '<i class="fa fa-trash-o"></i></button>  <b>[{top}]  {descricao}</b></p></li>';

/* [ GENÉRICO ] */
var RegistroInclusao = new Object();
var TagPadrao = $("#ulMenuConfig")[0].firstElementChild.firstElementChild.id != null && $("#ulMenuConfig")[0].firstElementChild.firstElementChild.id != "" ?
    $("#ulMenuConfig")[0].firstElementChild.firstElementChild.id : "FORN_QUEBRA";

/* [GENÉRICO] evento clique do menu de configuração */
$(document).ready(function (e) {
    RegistroListar(TagPadrao);
    $("#ulMenuConfig").click(function (c) {
        TagPadrao = c.target.id.toString();
        RegistroListar(TagPadrao);
    });
});

/* [GENÉRICO] incluir registro(s) no banco dados */
function RegistroIncluir() {
    RegistroInclusao.Tag = TagPadrao;

    $.post("/GarantiaConfiguracao/RegistroIncluir", { Registro: RegistroInclusao }, function (s) {
        if (s.Success) {
            PNotify.success({ text: s.Message });
            RegistroListar(TagPadrao);
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

/* [GENÉRICO] excluir no banco dados */
function RegistroExcluir(TagSelecionada, IdSelecionado) {
    var _Registro = new Object();
    _Registro.Id = IdSelecionado;
    _Registro.Tag = TagSelecionada;

    $.post("/GarantiaConfiguracao/RegistroExcluir", { Registro: _Registro }, function (s) {
        if (s.Success) {
            PNotify.success({ text: s.Message });
            RecarregarGrid(TagSelecionada);
        }
        else {
            PNotify.error({ text: s.Message });
        }
    }).fail(function (f) {
        console.log(f);
    }).done(function (d) {
        //console.log(d);
    });

}

/* [GENÉRICO] listar registros */
function RegistroListar(TagInformada) {
    $.get("/GarantiaConfiguracao/RegistroListar", { TAG: TagInformada }, function (s, status) {
        if (s.Success) {
            $(s.GridNome).DataTable({
                destroy: true,
                serverSide: false,
                searching: true,
                data: s.Data,
                columns: s.GridColunas,
            });
        }
        else {
            PNotify.error({ text: s.Message });
        }
    });
}

/***    FORNECEDOR QUEBRA   ***/
/* [FORNECEDOR QUEBRA] limpar lista */
$(botaoFornQuebraLimpar).click(function () {
    $(ulFornecedores).html("");
});

/* [FORNECEDOR QUEBRA] autocomplete */
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
                        registro.data = item.Data;
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
        FornecedorQuebraAdicionarLista(item);
        $(inputCodigoFornecedor).val("");
    }
});

/* [FORNECEDOR QUEBRA] adicionar na lista */
function FornecedorQuebraAdicionarLista(fornecedor) {
    if (jQuery.inArray(fornecedor.data, _fornecedoresGravar) == -1) {
        _fornecedoresGravar.push(fornecedor.data);
        $(ulFornecedores).append(liFornecedorQuebra.replace("{value}", fornecedor.value).replace("{id}", fornecedor.data).replace("{data0}", fornecedor.data).replace("{data1}", fornecedor.data));
    }
}

/* [FORNECEDOR QUEBRA] remover da lista */
function FornecedorQuebraRemoverLista(cnpj) {
    _fornecedoresGravar = jQuery.grep(_fornecedoresGravar, function (value) {
        return value != cnpj;
    });
    $("#" + cnpj).remove();
}

/* [FORNECEDOR QUEBRA] gravar no banco */
function FornecedorQuebraGravar() {
    RegistroInclusao.Inclusao = [];

    $.each(_fornecedoresGravar, function (i, item) {
        var registro = new Object();
        registro.Cod_Fornecedor = item;

        RegistroInclusao.Inclusao.push(JSON.stringify(registro));
    });

    _fornecedoresGravar = [];
    $(ulFornecedores).html("");

    RegistroIncluir();
}

/***    SANKHYA TOP     ***/
/* [SANKHYA TOP] remover lista  */
function ShankhyaTopRemoverLista(sankhyaTopItem) {
    var codigo = sankhyaTopItem.toString().split('|')[0];

    _SankhyaTopLista = jQuery.grep(_SankhyaTopLista, function (value) {
        return value != sankhyaTopItem;
    });
    $("#" + codigo).remove();
}

/* [SANKHYA TOP] adicionar na lista */
$(botaoSankhyaAdicionar).click(function () {
    if ($(inputSankhyaCodigo).val() != "" && $(inputSankhyaDescricao).val() != "") {
        var SankhyaTopItem = new Object();
        SankhyaTopItem.Top = $(inputSankhyaCodigo).val().toString().toUpperCase();
        SankhyaTopItem.Descricao = $(inputSankhyaDescricao).val().trim().toString().toUpperCase();
        SankhyaTopItem.Data = SankhyaTopItem.Top + "|" + SankhyaTopItem.Descricao;

        if (jQuery.inArray(SankhyaTopItem.Data, _SankhyaTopLista) == -1) {
            _SankhyaTopLista.push(SankhyaTopItem.Data);

            var liFormatado = liSankhyaTop.replace("{top}", SankhyaTopItem.Top).replace("{top}", SankhyaTopItem.Top).replace("{top}", SankhyaTopItem.Top);
            liFormatado = liFormatado.replace("{descricao}", SankhyaTopItem.Descricao).replace("{descricao}", SankhyaTopItem.Descricao).replace("*", "'").replace("*", "'");

            $(ulSankhyaTop).append(liFormatado);
        }
        $(inputSankhyaCodigo).val("");
        $(inputSankhyaDescricao).val("");
    }
    else {
        PNotify.error({ text: "Informe o Código e Descrição da Top!" });
    }
});

/* [SANKHYA TOP] gravar no banco */
function ShankhyaTopGravar() {
    RegistroInclusao.Inclusao = [];

    $.each(_SankhyaTopLista, function (i, item) {
        var registro = new Object();
        registro.Top = item.split('|')[0];
        registro.Descricao = item.split('|')[1];
        RegistroInclusao.Inclusao.push(JSON.stringify(registro));
    });

    RegistroIncluir();
}