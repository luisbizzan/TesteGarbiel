﻿
/***  GENÉRICO  ***/
var RegistroInclusao = new Object();
var TagPadrao = "SankhyaTop";

var _listaAutoComplete = [];
$('.onlyNumber').mask('0#');

$("#txtConfigEstFrete").on('change', function (e) {
    FormatarPercentagem("#txtConfigEstFrete");
});

$("#txtConfigDesvalorizacao").on('change', function (e) {
    FormatarPercentagem("#txtConfigDesvalorizacao");
});

/* [GENÉRICO] formatar percentagem configuração */
function FormatarPercentagem(IdControle) {
    var valor = $(IdControle).val().replace(",", ".");
    valor = (valor > 99.99) ? 100 : valor;
    $(IdControle).val(valor);
}

/* [GENÉRICO] evento clique do menu de configuração */
$(document).ready(function (e) {
    ListarNegociacao();
    RegistroListar(TagPadrao);

    $("#ulMenuConfig").click(function (c) {
        TagPadrao = c.target.id.toString();
        RegistroListar(TagPadrao);
    });
});

/* [GENÉRICO] mostrar mensagens */
function Mensagem(sucesso, mensagem) {
    if (sucesso) {
        PNotify.success({ text: mensagem, delay: 1000 });
    }
    else {
        PNotify.error({ text: mensagem, delay: 5000 });
    }
}

/* [GENÉRICO] incluir registro(s) no banco dados */
function RegistroIncluir() {
    RegistroInclusao.Tag = TagPadrao;

    $.post("/GarantiaConfiguracao/RegistroIncluir", { Registro: RegistroInclusao }, function (s) {
        if (s.Success) {
            RegistroListar(TagPadrao);
        }
        Mensagem(s.Success, s.Message);

    }).fail(function (f) {
        console.log(f);
    }).done(function (d) {
        //console.log(d);
    });

    RegistroInclusao.Inclusao = [];
    _listaAutoComplete = [];
}

/* [GENÉRICO] incluir registro(s) no banco dados */
function RegistroAtualizar() {
    RegistroInclusao.Tag = TagPadrao;

    $.post("/GarantiaConfiguracao/RegistroAtualizar", { Registro: RegistroInclusao }, function (s) {
        if (s.Success) {
            RegistroListar(TagPadrao);
        }
        Mensagem(s.Success, s.Message);

    }).fail(function (f) {
        console.log(f);
    }).done(function (d) {
        //console.log(d);
    });

    RegistroInclusao.Inclusao = [];
    _listaAutoComplete = [];
}

/* [GENÉRICO] excluir no banco dados */
function RegistroExcluir(TagSelecionada, IdSelecionado) {
    var _Registro = new Object();
    _Registro.Id = IdSelecionado;
    _Registro.Tag = TagSelecionada;

    $.post("/GarantiaConfiguracao/RegistroExcluir", { Registro: _Registro }, function (s) {
        if (s.Success) {
            RegistroListar(TagSelecionada);
        }

        Mensagem(s.Success, s.Message);

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
                data: s.Data,
                columns: s.GridColunas,
            });
        }
        else {
            Mensagem(s.Success, s.Message);
        }
    });
}