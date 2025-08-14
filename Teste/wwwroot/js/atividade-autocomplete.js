<script>
    $(document).ready(function () {
        // Atividade Principal
        $('#atividadePrincipal').select2({
            ajax: {
                url: '/Sede/BuscarActividades',
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
            placeholder: 'Selecione a atividade principal',
            minimumInputLength: 2
        });

        // Tipo de Entidade (sem AJAX, apenas estilização do select)
        $('#TipoEntidade').select2({
            placeholder: 'Selecione o tipo de entidade',
            allowClear: true
        });

        $('#FormaJuridica').select2({
            placeholder: 'Selecione o tipo de entidade',
            allowClear: true
        });
    });

</script>