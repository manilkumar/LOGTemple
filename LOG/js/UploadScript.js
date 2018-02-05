$(function () {

    $(document).on('click', '#btnNewUpload', function () {

        $.get("/Home/UploadNew", {}, function (html) {

            $('#uploadTbody').append($(html));


        });
    });

    $(document).on('click', '#btnDelete', function () {

        var result = confirm("Want to delete?");
        if (result) {
            alert("YES");
        }
    })

    $(document).on('click', '#btnSaveItems', function () {

        var parent = $(this).parents('tr:first');

        var fileUpload = parent.find("input[type=file]").get(0);
        var files = fileUpload.files;

        // Create FormData object  
        var fileData = new FormData();

        // Looping over all files and add it to FormData object  
        for (var i = 0; i < files.length; i++) {
            fileData.append(files[i].name, files[i]);
        }

        fileData.append("UploadType", parent.find("#type").val());
        fileData.append("Title", parent.find("#title").val());

        $.ajax({
            url: '/Home/UploadFiles',
            type: "POST",
            contentType: false, // Not to set any content header  
            processData: false, // Not to process data  
            data: fileData,
            success: function (result) {
                alert(result);
            },
            error: function (err) {
                alert(err.statusText);
            }
        });

    });

});