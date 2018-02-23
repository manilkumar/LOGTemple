$(function () {



    $(document).on('click', '#btnReply', function () {

        var btn = $(this);

        $.post("ReplyVisitor", { emailId: $(btn).data('email') }, function (html) {

            var div = $(html);

            $.showDialog("Reply To Visitor(s)", div, {}, function () {

                $(div).find('#summernote').summernote({

                    height: 150,

                    placehodler: 'Type your reply message...'
                });

                $('.note-insert,.note-view').remove();

            });

        })

    });

    $(document).on('click', '#btnSendEmails', function () {

        var parent = $(this).parents('.modal-content:first');

        var btn = parent.find("#btnSendEmails")

        var fileUpload = parent.find("input[type=file]").get(0);
        var files = fileUpload.files;

        // Create FormData object  
        var fileData = new FormData();

        // Looping over all files and add it to FormData object  
        for (var i = 0; i < files.length; i++) {
            fileData.append(files[i].name, files[i]);
        }

        if (!parent.find("#chkAll").is(':checked')) {

            fileData.append("TO", $(btn).data('email'));
        }

        fileData.append("Subject", parent.find("#txtSubject").val());

        fileData.append("Body", parent.find("#summernote").summernote('code'));

        $.ajax({
            url: '/Home/SendEmails',
            type: "POST",
            contentType: false, // Not to set any content header  
            processData: false, // Not to process data  
            data: fileData,
            success: function (result) {

                if (result == "Success") {

                    alert("Email(s) Send Succesfully.");
                }

                else {

                    alert("Unable to send mail " + result);
                }

                $.closeDialogs();
            },
            error: function (err) {
                alert(err.statusText);
            }
        });


    });

});