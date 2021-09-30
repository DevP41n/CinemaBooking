
        $(document).ready(function () {
            var message = $('#Message').text();
            var warning = $('#Warning').text();
            var error = $('#Error').text();
            if (message != '') {
                toastr.success(message);
            }

            if (warning != '') {
                toastr.warning(warning);
            }

            if (error != '') {
                toastr.error(error);
            }

        });
