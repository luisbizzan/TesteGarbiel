(function () {
    $('.onlyNumber').mask('0#');

    $("#tratarDivergenciasRecebimento").click(function () {
        var isVAlid = true;

        $(".linha-divergencia").each(function () {
            if ($(this).find(".divergenciaMais").val() === "" && $(this).find(".divergenciaMenos").val() === "") {
                return;
            }
        });

        if (isVAlid) {
            console.log('erro 2');
        }
        
        //$.ajax({
        //    url: HOST_URL + "BORecebimentoNota/TratarDivergencia",
        //    cache: false,
        //    method: "POST",
        //    data: $("#divergencias").serialize(),
        //    success: function (result) {
        //        if (!result.Success) {
        //            $(".close").click();
        //            $("#dataTable").DataTable().ajax.reload();
        //        } else {
        //            PNotify.info({ text: result.Message });
        //            $(".close").click();
        //            $("#dataTable").DataTable().ajax.reload();
        //        }
        //    }
        //});
    });
})();