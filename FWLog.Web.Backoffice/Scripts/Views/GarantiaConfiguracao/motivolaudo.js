/* gravar registro */
function GravarMotivoLaudo() {
    RegistroInclusao.Inclusao = [];
    var registro = new Object();
    registro.Id_Tipo = $("#spanIdTipo").html();
    registro.MotivoLaudoDescricao = $("#txtDescricaoMotivoLaudo").val();
    RegistroInclusao.Inclusao.push(JSON.stringify(registro));
    CancelarMotivoLaudo();
    RegistroIncluir();
}

/* cancelar */
function CancelarMotivoLaudo() {
    $("#txtDescricaoMotivoLaudo").val("");
}