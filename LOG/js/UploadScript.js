$(function () {

    $(document).on('click', '#btnNewUpload', function () {

        $.get("/Home/UploadNew", {}, function (html) {

            $('#uploadTbody').append($(html));


        });
    });

});