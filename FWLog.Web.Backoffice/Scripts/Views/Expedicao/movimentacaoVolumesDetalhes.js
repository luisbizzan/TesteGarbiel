(function () {
    $("#dataTablePedidoVendaModal").on('click', "[action='exibirDetalhesVolume']", exibirDetalhesVolume);
})();

function exibirDetalhesVolume() {
    let modalDetalhesPedidoVolume = $("#modalDetalhesPedidoVolume");

    $(".close").click();

    var id = $(this).data("id");

    var link = $('#Url').val(); 
    
    modalDetalhesPedidoVolume.load("DetalhesPedidoVolume/" + id, function () {
        modalDetalhesPedidoVolume.modal();
        $('#voltar').css('display', 'inline-block').attr('href', link)
    });
}