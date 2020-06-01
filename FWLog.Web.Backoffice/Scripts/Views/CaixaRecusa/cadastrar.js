(function () {
    $("#adicionarLinhaTabela").click(function () {
        var idProduto = $("#IdProduto").val();
        var produto = $("#DescricaoProduto").val();
        var idCaixa = $("#IdCaixa").val();
        var caixa = $("#DescricaoCaixa").val();

        if (!produto || !caixa) {
            PNotify.warning({ text: "Preencha os campos produto e caixa." });
            return;
        }

        if (validarCaixaCadastrada(idCaixa)) {
            PNotify.warning({ text: "A caixa informada já foi cadastrada. Para adicionar novos produtos, acesse a opção Editar na tela de listagem." });
            return;
        }

        if (validarItemExisteTabela(idCaixa, idProduto)) {
            PNotify.warning({ text: "O produto já foi adicionado na tabela." });
            return;
        }

        if (validarCaixaDiferente(idCaixa)) {
            PNotify.warning({ text: "A caixa não pode ser diferente." });
            return;
        }

        var markup = "<tr><td><input type='checkbox' name='record'></td><td class='hide'>" + idCaixa + "</td><td>" + caixa + "</td><td class='hide'>" + idProduto + "</td><td>" + produto + "</td></tr>";
        $("table tbody").append(markup);
        limparProduto();
    });

    $("#removerLinhaTabela").click(function () {
        $("table tbody").find('input[name="record"]').each(function () {
            if ($(this).is(":checked")) {
                $(this).parents("tr").remove();
            }
        });
    });

    $("#salvarCaixaRecusa").click(function () {
        salvarCaixaRecusa();
    });

    $("#pesquisarProduto").on('click', function () {
        if (!$(this).attr('disabled')) {
            $("#modalProduto").load(HOST_URL + "Produto/SearchModal", function () {
                $("#modalProduto").modal();
            });
        }
    });

    function limparProduto() {
        $("#IdProduto").val("");
        $("#DescricaoProduto").val("");
    }

    $("#limparProduto").click(function () {
        if (!$(this).attr('disabled')) {
            limparProduto();
        }
    });

    $("#pesquisarCaixa").on('click', function () {
        if (!$(this).attr('disabled')) {
            $("#modalCaixa").load(HOST_URL + "Caixa/SearchModal", function () {
                $("#modalCaixa").modal();
            });
        }
    });

    function limparCaixa() {
        $("#IdCaixa").val("");
        $("#DescricaoCaixa").val("");
    }

    $("#limparCaixa").click(function () {
        if (!$(this).attr('disabled')) {
            limparCaixa();
        }
    });
})();

function validarItemExisteTabela(novaCaixa, novoProduto) {
    var existe = false;

    $('table tbody tr').each(function (i, el) {
        var caixa = $(el).children().eq(1).text();
        var produto = $(el).children().eq(3).text();
        if (caixa == novaCaixa && produto == novoProduto) {
            existe = true;
        }
    });

    return existe;
}

function validarCaixaCadastrada(idCaixa) {
    var caixaCadastrada = false;

    $.ajax({
        url: HOST_URL + CONTROLLER_PATH + "VerificarCaixaCadastrada",
        method: "POST",
        async: false,
        data: {
            idCaixa: idCaixa
        },
        success: function (result) {
            if (result.Success) {
                caixaCadastrada = true;
            }
        },
        error: function (data) {
            PNotify.error({ text: "Ocorreu um erro na verificação da Caixa." });
            NProgress.done();
        }
        });

    return caixaCadastrada;
}

function salvarCaixaRecusa() {
    debugger
    var idEmpresa = $("#IdEmpresa").val();
    var listaCaixaRecusa = new Array();

    $("table tbody tr").each(function () {
        var row = $(this);
        var caixaRecusa = {};
        caixaRecusa.IdCaixa = row.find("td").eq(1).html();
        caixaRecusa.IdProduto = row.find("td").eq(3).html();
        caixaRecusa.idEmpresa = idEmpresa;
        listaCaixaRecusa.push(caixaRecusa);
    });

    $.ajax({
        url: HOST_URL + CONTROLLER_PATH + "Cadastrar",
        method: "POST",
        data: JSON.stringify(listaCaixaRecusa),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            if (result.Success) {
                PNotify.success({ text: result.Message });
                setTimeout(redirecionar, 2000);
            }
            else {
                PNotify.error({ text: result.Message });
            }
        },
        error: function (data) {
            PNotify.error({ text: "Ocorreu um erro para salvar a caixa e os produtos de recusa." });
            NProgress.done();
        }
    });
}

function redirecionar() {
    window.location.href = HOST_URL + CONTROLLER_PATH + "Index"
}

function validarCaixaDiferente(novaCaixa) {
    var diferente = false;

    var tbody = $("table tbody");

    if (tbody.children().length != 0) {
        $('table tbody tr').each(function (i, el) {
            var caixa = $(el).children().eq(1).text();
            if (caixa != novaCaixa) {
                diferente = true;
            }
        });
    }

    return diferente;
}

function setProduto(idProduto, descricao) {
    $("#IdProduto").val(idProduto);
    $("#DescricaoProduto").val(descricao);
    $("#modalProduto").modal("hide");
    $("#modalProduto").empty();
}

function setCaixa(idCaixa, descricao) {
    $("#IdCaixa").val(idCaixa);
    $("#DescricaoCaixa").val(descricao);
    $("#modalCaixa").modal("hide");
    $("#modalCaixa").empty();
}