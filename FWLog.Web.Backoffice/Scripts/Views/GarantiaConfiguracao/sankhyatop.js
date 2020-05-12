$("#divEditarSankhyaTop").attr("style", "display:none;");
var IdRegistroSelecionado = 0;

/* drop down list Id Negociação  */
function ListarNegociacao() {
    $.get("/GarantiaConfiguracao/ListarIdNegociacao", {}, function (s, status) {
        if (s.Success) {
            $("#ddlNegociacao").empty();
            $.each(s.Lista, function (key, item) {
                $("#ddlNegociacao").append('<option value=' + item.Data + '>' + item.Value + '</option>');
            });
        }
        else {
            Mensagem(s.Success, s.Message);
        }
    });
}

/* gravar no banco */
function GravarShankhyaTop() {
    RegistroInclusao.Inclusao = [];

    var registro = new Object();
    registro.Id = IdRegistroSelecionado;
    registro.Id_Negociacao = $("#ddlNegociacao").val();
    registro.Top = $("#txtSankhyaCodigo").val();
    RegistroInclusao.Inclusao.push(JSON.stringify(registro));

    console.log(RegistroInclusao);

    RegistroAtualizar();
    CancelarShankhyaTop();
}

/* cancelar */
function CancelarShankhyaTop() {
    $("#divEditarSankhyaTop").attr("style", "display:none;");
    RegistroInclusao.Inclusao = [];

    $("#ddlNegociacao").val(0);
    $("#txtSankhyaCodigo").val("");
    $("#spanSankhyaDescricao").html("");
}

/* editar selecionar registro */
function RegistroEditar(TagSelecionada, IdSelecionado, Parametros) {
    $("#divEditarSankhyaTop").attr("style", "display:normal;");
    var registroEditar = new Object();
    registroEditar.Parametros = Parametros;
    registroEditar.IdNegociacao = registroEditar.Parametros.split(";")[0];
    registroEditar.Top = registroEditar.Parametros.split(";")[1];
    registroEditar.Descricao = registroEditar.Parametros.split(";")[2];
    IdRegistroSelecionado = IdSelecionado;

    $("#ddlNegociacao").val(registroEditar.IdNegociacao);
    $("#txtSankhyaCodigo").val(registroEditar.Top);
    $("#spanSankhyaDescricao").html(registroEditar.Descricao);   
}
