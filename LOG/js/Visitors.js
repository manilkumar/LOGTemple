$(function () {

    $(document).on('click', '#btnReply', function () {

        var btn = $(this);

        $.post("ReplyVisitor", { id: $(btn).data('id') }, function (html) {

            var div = $(html);

            $.showDialog("Reply To Visitor(s)", div, {}, function () {


            });

        })

    });

});