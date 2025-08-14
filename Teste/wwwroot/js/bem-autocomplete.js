<script>
    $(document).ready(function () {
        
        // Bem Principal
        $('#bemPrincipal').select2({
            ajax: {
                url: '/Sede/BuscarBens',
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    return {
                        termo: params.term
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
            minimumInputLength: 2
        });
    });

</script>