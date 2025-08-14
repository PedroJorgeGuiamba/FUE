//$.ajax({
//    url: '/Sede/Create',
//    type: 'POST',
//    data: $('form').serialize(),
//    success: function (response) {
//        if (response.success) {
//            $('#successModal .modal-body').html(response.message);
//            $('#successModal').modal('show');
//        } else {
//            $('#mainContent').html(response.partialView);
//            if (response.errors.length > 0) {
//                alert(response.errors.join('\n'));
//            }
//        }
//    },
//    error: function (xhr, status, error) {
//        alert('Erro inesperado: ' + error);
//    }
//});