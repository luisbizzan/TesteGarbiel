
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
    registro.Id_Negociacao = $("#ddlNegociacao").val();
    registro.Top = $("#txtSankhyaCodigo").val();
    registro.Descricao = $("#txtSankhyaDescricao").val();
    RegistroInclusao.Inclusao.push(JSON.stringify(registro));

    RegistroIncluir();
    CancelarShankhyaTop();
}

/* cancelar */
function CancelarShankhyaTop() {
    RegistroInclusao.Inclusao = [];

    $("#ddlNegociacao").val(0);
    $("#txtSankhyaCodigo").val("");
    $("#txtSankhyaDescricao").val("");
}