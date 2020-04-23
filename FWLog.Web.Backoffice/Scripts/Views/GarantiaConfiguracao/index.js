
/***  GENÉRICO  ***/
var RegistroInclusao = new Object();
var TagPadrao = $("#ulMenuConfig")[0].firstElementChild.firstElementChild.id != null && $("#ulMenuConfig")[0].firstElementChild.firstElementChild.id != "" ?
    $("#ulMenuConfig")[0].firstElementChild.firstElementChild.id : "FornecedorQuebra";
var _listaAutoComplete = [];

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
                //searching: true,
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
/* [FORNECEDOR QUEBRA] variaveis */
var _fornecedoresGravar = [];
var _fornecedoresAutoComplete = [];
var liFornecedorQuebra = '<li id="{id}"><p><button type="button" class="btn btn-danger" onclick="FornecedorQuebraRemoverLista({data1});">' +
    '<i class="fa fa-trash-o"></i></button>  <b>[{data0}]  {value}</b></p></li>';

/* [FORNECEDOR QUEBRA] limpar lista */
$("#btnLimpar").click(function () {
    $("#listaFornecedor").html("");
});

/* [FORNECEDOR QUEBRA] autocomplete */
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
                    $.each(s.Lista, function (e, item) {
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
        $("#Cod_Fornecedor").val("");
    }
});

/* [FORNECEDOR QUEBRA] adicionar na lista */
function FornecedorQuebraAdicionarLista(fornecedor) {
    if (jQuery.inArray(fornecedor.data, _fornecedoresGravar) == -1) {
        _fornecedoresGravar.push(fornecedor.data);
        $("#listaFornecedor").append(liFornecedorQuebra.replace("{value}", fornecedor.value).replace("{id}", fornecedor.data).replace("{data0}", fornecedor.data).replace("{data1}", fornecedor.data));
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
    $("#listaFornecedor").html("");

    RegistroIncluir();
}

/***    SANKHYA TOP     ***/
/* [SANKHYA TOP] variaveis */
var _SankhyaTopLista = [];
var liSankhyaTop = '<li id="{top}"><p><button type="button" class="btn btn-danger" onclick="ShankhyaTopRemoverLista(*{top}|{descricao}*);">' +
    '<i class="fa fa-trash-o"></i></button>  <b>[{top}]  {descricao}</b></p></li>';

/* [SANKHYA TOP] remover lista  */
function ShankhyaTopRemoverLista(sankhyaTopItem) {
    var codigo = sankhyaTopItem.toString().split('|')[0];

    _SankhyaTopLista = jQuery.grep(_SankhyaTopLista, function (value) {
        return value != sankhyaTopItem;
    });
    $("#" + codigo).remove();
}

/* [SANKHYA TOP] adicionar na lista */
$("#btnSankhyaAdicionar").click(function () {
    if ($("#txtSankhyaCodigo").val() != "" && $("#txtSankhyaDescricao").val() != "") {
        var SankhyaTopItem = new Object();
        SankhyaTopItem.Top = $("#txtSankhyaCodigo").val().toString().toUpperCase();
        SankhyaTopItem.Descricao = $("#txtSankhyaDescricao").val().trim().toString().toUpperCase();
        SankhyaTopItem.Data = SankhyaTopItem.Top + "|" + SankhyaTopItem.Descricao;

        if (jQuery.inArray(SankhyaTopItem.Data, _SankhyaTopLista) == -1) {
            _SankhyaTopLista.push(SankhyaTopItem.Data);

            var liFormatado = liSankhyaTop.replace("{top}", SankhyaTopItem.Top).replace("{top}", SankhyaTopItem.Top).replace("{top}", SankhyaTopItem.Top);
            liFormatado = liFormatado.replace("{descricao}", SankhyaTopItem.Descricao).replace("{descricao}", SankhyaTopItem.Descricao).replace("*", "'").replace("*", "'");

            $("#listaSankhyaTop").append(liFormatado);
        }
        $("#txtSankhyaCodigo").val("");
        $("#txtSankhyaDescricao").val("");
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

/***  REMESSA USUÁRIO ***/
/* [REMESSA USUÁRIO] */
var _remessaUsuarioAutoComplete = [];
var _remessaUsuarioGravar = [];

var liRemessaUsuario = '<li id="{data}"><p><button type="button" class="btn btn-danger" onclick="RemessaUsuarioRemoverLista(*{data}*);">' +
    '<i class="fa fa-trash-o"></i></button>  <b> {value}</b></p></li>';

/* [REMESSA USUÁRIO] autocomplete */
$("#txtRemessaUsuario").autocomplete({
    lookup: function (query, done) {

        var AutoComplete = new Object();
        AutoComplete.tag = TagPadrao;
        AutoComplete.palavra = $("#txtRemessaUsuario").val();

        $.ajax({
            url: "/GarantiaConfiguracao/AutoComplete",
            global: false,
            method: "POST",
            data: { autocomplete: AutoComplete },
            success: function (s) {
                if (s.Success) {
                    $.each(s.Lista, function (e, item) {
                        var registro = new Object();
                        registro.data = item.Data;
                        registro.value = item.Value;
                        _remessaUsuarioAutoComplete.push(registro);
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
            suggestions: _remessaUsuarioAutoComplete
        };

        _remessaUsuarioAutoComplete = [];
        done(result);
    },

    onSelect: function (item) {
        RemessaUsuarioAdicionarLista(item);
        $("#txtRemessaUsuario").val("");
    }
});

/* [REMESSA USUÁRIO] adicionar na lista */
function RemessaUsuarioAdicionarLista(usuario) {
    if (jQuery.inArray(usuario.data, _remessaUsuarioGravar) == -1) {
        _remessaUsuarioGravar.push(usuario.data);
        $("#listaRemessaUsuarios").append(liRemessaUsuario.replace("{data}", usuario.data).replace("{data}", usuario.data).replace("{value}", usuario.value).replace("*", "'").replace("*", "'"));
    }
    $("#txtRemessaUsuario").val("");
}

/* [REMESSA USUÁRIO] remover da lista */
function RemessaUsuarioRemoverLista(IdUsuario) {
    _remessaUsuarioGravar = jQuery.grep(_remessaUsuarioGravar, function (value) {
        return value != IdUsuario;
    });
    $("#" + IdUsuario).remove();
}

/* [REMESSA USUÁRIO] gravar no banco */
function RemessaUsuarioGravar() {
    RegistroInclusao.Inclusao = [];

    $.each(_remessaUsuarioGravar, function (i, item) {
        var registro = new Object();
        registro.Id_Usr = item;

        RegistroInclusao.Inclusao.push(JSON.stringify(registro));
    });

    _remessaUsuarioGravar = [];
    $("#listaRemessaUsuarios").html("");

    RegistroIncluir();
}

/*** REMESSA CONFIGURAÇÃO  ***/

/* [REMESSA CONFIGURAÇÃO] gravar no banco */
function RemessaConfigGravar() {
    RegistroInclusao.Inclusao = [];

    var registro = new Object();
    registro.Id_Filial_Sankhya = $("#spanIdFilial").html() == "" ? 0 : $("#spanIdFilial").html();
    registro.Filial = $("#spanFilial").html();
    registro.Cod_Fornecedor = $("#spanFornecedor").html() == "" ? 0 : $("#spanFornecedor").html();
    registro.Automatica = $("#chkRCAutomatica")[0].checked ? 1 : 0;
    registro.Vlr_Minimo = $("#txtRCMinimoEnvio").val();
    registro.Total = $("#txtRCTotal").val();
    RegistroInclusao.Inclusao.push(JSON.stringify(registro));

    console.log(RegistroInclusao);

    RegistroIncluir();
}

/* [REMESSA CONFIGURAÇÃO] autocomplete - FORNECEDOR */
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
                if (s.Success) {
                    $.each(s.Lista, function (e, item) {
                        var registro = new Object();
                        registro.data = item.Data;
                        registro.value = item.Value;
                        _listaAutoComplete.push(registro);
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


/* [REMESSA CONFIGURAÇÃO] autocomplete - FILIAL */
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
                    $.each(s.Lista, function (e, item) {
                        var registro = new Object();
                        registro.data = item.Data;
                        registro.value = item.Value;
                        _listaAutoComplete.push(registro);
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