
jQuery(document).ready(function ($) {
    "use strict";

    $('ul.nav li.dropdown').hover(function () {
        $(this).find('.dropdown-menu').stop(true, true).delay(200).fadeIn(500);
    }, function () {
        $(this).find('.dropdown-menu').stop(true, true).delay(200).fadeOut(500);
    });


    var emailExp = /^[^\s()<>@,;:\/]+@\w[\w\.-]+\.[a-z]{2,}$/i,
       phoneno = /^\d{10}$/,
       ferror = false;

    $('input,textarea').on('change', function () {

        var ele = $(this);

        ApplyValidation(ele);
    });

    //Contact
    $('form.contactForm').submit(function () {

        var f = $(this).find('.form-group'),
            ferror = false;


        f.children('input').each(function () { // run all inputs

            var i = $(this); // current input

            ApplyValidation(i);
        });
        f.children('textarea').each(function () { // run all inputs

            var i = $(this); // current input

            ApplyValidation(i);
        });

        if (ferror) return false;
        else var str = $(this).serialize();

        if ($('.validation').text().trim() == "") {

            $.ajax({
                type: "POST",
                url: "Home/SaveUser",
                data: str,
                success: function (msg) {

                    $("#sendmessage").addClass("show").fadeOut(8000, function () {

                        $(this).removeClass('show');

                    });

                    $("#errormessage").removeClass("show");

                    $('form.contactForm').clearElements();
                },
                error: function () {

                    $("#errormessage").addClass("show").fadeOut(8000, function () {

                        $(this).removeClass('show');

                    });
                    $('#errormessage').html(msg);
                }
            });
        }
        return false;
    });

    function ApplyValidation(ele) {

        var rule = ele.attr('data-rule');

        if (rule !== undefined) {
            var ierror = false; // error flag for current input
            var pos = rule.indexOf(':', 0);
            if (pos >= 0) {
                var exp = rule.substr(pos + 1, rule.length);
                rule = rule.substr(0, pos);
            } else {
                rule = rule.substr(pos + 1, rule.length);
            }

            switch (rule) {
                case 'required':
                    if (ele.val() === '') { ferror = ierror = true; }
                    break;

                case 'minlen':
                    if (ele.val().length < parseInt(exp)) { ferror = ierror = true; }
                    break;

                case 'email':
                    if (!emailExp.test(ele.val())) { ferror = ierror = true; }
                    break;

                case 'phoneno':
                    if (!phoneno.test(ele.val())) { ferror = ierror = true; }
                    break;

                case 'checked':
                    if (!ele.attr('checked')) { ferror = ierror = true; }
                    break;

                case 'regexp':
                    exp = new RegExp(exp);
                    if (!exp.test(ele.val())) { ferror = ierror = true; }
                    break;
            }
            ele.parent('div').find('.validation').html((ierror ? (ele.attr('data-msg') !== undefined ? ele.attr('data-msg') : 'wrong Input') : '')).show('blind');

        }
    }
});