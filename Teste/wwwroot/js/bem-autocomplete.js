//<script>
//    $(document).ready(function () {

//        // Bem Principal
//        $('#bemPrincipal').select2({
//            ajax: {
//                url: '/Sede/BuscarBens',
//                dataType: 'json',
//                delay: 250,
//                data: function (params) {
//                    return {
//                        termo: params.term
//                    };
//                },
//                processResults: function (data) {
//                    return {
//                        results: data
//                    };
//                },
//                cache: true
//            },
//            placeholder: 'Selecione o bem principal',
//            minimumInputLength: 2
//        });
//    });

//</script>

$(document).ready(function () {
    // Bem Principal
    $('#BemPrincipalId').select2({
        ajax: {
            url: '/Sede/BuscarBens',
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return {
                    termo: params.term || '' // Permite busca inicial com termo vazio
                };
            },
            processResults: function (data) {
                return {
                    results: data
                };
            },
            cache: true
        },
        placeholder: 'Selecione o bem principal',
        minimumInputLength: 0, // Permite busca sem digitar
        allowClear: true
    });

    // Forçar busca inicial
    $('#BemPrincipalId').trigger('change');
});