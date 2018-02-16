


$.confirmDialog = function (title, body, yesFn, cancelFn, yesBtnText, canceBtnText, yesBtnClas, cancelBtnClass) {

    var dilogBox = $('#delConfrimDialog');

    var dBtn = $(dilogBox).find('.dBtn'), cBtn = $(dilogBox).find('.cBtn');

    dBtn.unbind('click');
    cBtn.unbind('click');


    if (!$.trim(title).length) {
        title = "Are you sure?";
    }

    dilogBox.find(".modal-title").empty().append(title);

    if ($.trim(body).length) {

        dilogBox.find(".modal-body").empty().append(body).show();

    } else {

        dilogBox.find(".modal-body").empty().hide();
    }

    if (yesFn) {

        dBtn.bind('click', yesFn);
        $(".modal-dialog").drags();
    } else {

        dBtn.bind('click', function () {
            $('#delConfrimDialog').modal('hide');
        });
    }

    if (cancelFn) {

        cBtn.bind('click', cancelFn);

    } else {

        cBtn.bind('click', function () {
            $('#delConfrimDialog').modal('hide');
        });
    }

    if ($.trim(yesBtnText).length) {

        dBtn.text(yesBtnText);

    } else {
        dBtn.text('Delete');
    }

    if ($.trim(yesBtnClas).length) {

        dBtn.removeClass('btn-danger').addClass(yesBtnClas);

    } else {
        dBtn.addClass('btn-danger');
    }

    if ($.trim(cancelBtnClass).length) {

        cBtn.addClass(cancelBtnClass);

    }

    if ($.trim(canceBtnText).length) {

        cBtn.text(canceBtnText);

    } else {
        cBtn.text('Cancel');
    }

    $(dilogBox).modal({ keyboard: false, show: true, backdrop: 'static' });

    //$('.modal-dialog').draggable({
    //    handle: ".modal-header"
    //});
};

$.closeDialogs = function () {
    $(".modal").modal('hide');
};

$.notify = function (msg, type, position, onCloseFn, delay) {

    //var id = "notify" + ("" + Math.random()).substr(2, 6);

    if (type == undefined || !$.trim(type).length)
        type = "success";


    if (position == undefined || !$.trim(position).length)
        position = "top-right";

    var div = $('div.notifications.' + position);

    $(div).notify({

        fadeOut: { enabled: true, delay: delay || 3000 },
        message: { text: msg },

        type: type,

        onClosed: function () {

            //$("#" + id).remove();

            if (onCloseFn) {
                onCloseFn();
            }

        },
        transition: 'fade'

    }).show();
};

$.dangerNotify = function (msg, delay) {

    $.notify(msg, "danger", null, null, delay);

};

$.dNotify = function (msg) {

    $.notify(msg, "danger");

};

$.showDialog = function (title, body, attr, onShow, onHide, actionFn) {

    $('#modalDialog').showDialog(title, body, attr, actionFn);

    $('#modalDialog').on('shown.bs.modal', function (e) {

        if (onShow && typeof onShow == "function") {

            onShow();
          
        }

    });


    $('#modalDialog').on('hidden.bs.modal', function (e) {

        var _that = this;

        if (onHide && typeof onHide == "function") {

            onHide();
        }

        $(_that).find('.modal-body,.modal-title').empty();

        $(_that).find('.modal-footer').find("button").removeAttr("onclick").unbind("bind");

        $(_that).off("hide.bs.modal");
    })
};

(function ($) {

    $.fn.showDialog = function (title, body, attr, actionFn) {

        var that = this;

        var _div = $('<div/>').html(body);

        var _title = $(_div).find(".modal-title");

        if (_title.length > 0) {

            $(that).find('.modal-title').replaceWith(_title);

        } else {

            $(that).find('.modal-title').html(title);
        }

        var _footer = $(_div).find(".modal-footer");

        if (_footer.length > 0) {

            if (_footer.length > 1) {
                _footer.not(":first").remove();
            }

            $(that).find('.modal-footer').replaceWith(_footer);

        };

        $(that).find('.modal-body').empty().append(_div);

        $(that).find('.modal-body').find(".modal-title,.modal-footer").remove();

        $(that).modal(attr || { keyboard: false, show: true });

        if (actionFn && typeof actionFn == "function") {

            $(that).find(".btn-action").click(actionFn);
        }


        var _frm = $(that).find("form");

        _frm.removeData("validator");

        _frm.removeData("unobtrusiveValidation");

        $.validator.unobtrusive.parse(_frm);

    };

})(jQuery);