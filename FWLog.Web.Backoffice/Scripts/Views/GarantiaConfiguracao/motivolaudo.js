/* gravar registro */
function GravarMotivoLaudo() {
    RegistroInclusao.Inclusao = [];
    var registro = new Object();
    registro.Id_Tipo = 8;
    registro.MotivoLaudoDescricao = $("#txtDescricaoMotivoLaudo").val();
    RegistroInclusao.Inclusao.push(JSON.stringify(registro));
    console.log(RegistroInclusao);
    RegistroIncluir();
    MotivoLaudoCancelar();
}

/* limpar */
function CancelarMotivoLaudo() {
    RegistroInclusao.Inclusao = [];
    $("#txtDescricaoMotivoLaudo").val("");
}