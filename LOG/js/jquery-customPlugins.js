
Array.prototype.unique = function () {
    var r = new Array();
    o: for (var i = 0, n = this.length; i < n; i++) {
        for (var x = 0, y = r.length; x < y; x++) {
            if (r[x] == this[i]) {
                continue o;
            }
        }
        r[r.length] = this[i];
    }
    return r;
};

Array.prototype.diff = function (a) {
    return this.filter(function (i) { return a.indexOf(i) < 0; });
};

Number.prototype.pad = function (size) {
    var s = String(this);
    while (s.length < (size || 2)) { s = "0" + s; }
    return s;
};

(function ($) {

    $.fn.eleValidate = function () {

        var allValid = true;

        var elements = this;

        $.each(elements, function (i, that) {

            if ($(that).data('validator')) {

                $(that).validate();

                var valid = $(that).valid();

                if (!valid && allValid)
                    allValid = false;

            } else {

                $(that).validate();

                if ($(that).data('validator')) {

                    var valid = $(that).valid();

                    if (!valid && allValid)
                        allValid = false;
                }

            }
        });

        return allValid;

    };

    $.fn.applyValidation = function (msg, type, imValidate) {

        var that = this;

        var frm = $(that).parents('form:first').length > 0 ? $(that).parents('form:first') : $('form');

        if (!frm.data('validator'))
            frm.validate();

        if (!imValidate)
            imValidate = false;

        if (!type || $.trim(type) == "") {
            type = 'required';
        }

        if (!msg || $.trim(msg) == "") {
            msg = 'Please fix this field.';
        }

        var _ruleObj = {};

        if (typeof type == 'object') {

            _ruleObj = type;

            if (!_ruleObj['messages']) {

                _ruleObj['messages'] = {};

                for (var key in _ruleObj) {
                    if (key != "messages")
                        _ruleObj['messages'][key] = msg;
                }
            }

        } else if (typeof type == 'string') {

            _ruleObj[type] = true;
            _ruleObj['messages'] = {};
            _ruleObj['messages'][type] = msg;
        }

        $.validator.unobtrusive.parse(frm);

        $.each($(that), function (i, ele) {

            $(ele).rules('add', _ruleObj);

        });

        $.validator.unobtrusive.parse(frm);

        if (imValidate) {
            $(that).eleValidate();
        }
    };

    $.fn.clearValidation = function () {

        var that = this;

        var items = $(that).filter(function () {
            return ($(this).data('validator') != undefined || $(this).data('valRequired') != undefined)
        });

        $.each(items, function (i, ele) {

            $(ele).data('validator', null);
            $(ele).rules('remove', 'required');
            $(ele).unbind('validate');
            $(ele).validate({
                errorPlacement: function () {
                    return true;
                }
            });

            $(ele).removeClass('input-validation-error');

            $(ele).removeClass('error');

            $('span[for="' + $(ele).attr("id") + '"]').empty();
            $('span[for="' + $(ele).attr("name") + '"]').empty();


            $('span[data-valmsg-for="' + $(ele).attr("id") + '"] span').empty();
            $('span[data-valmsg-for="' + $(ele).attr("name") + '"] span').empty();

            $('span[data-valmsg-for="' + $(ele).attr("id") + '"]').empty();
            $('span[data-valmsg-for="' + $(ele).attr("name") + '"]').empty();

            $('label[id="' + $(ele).attr("id") + '-error"]').empty();
            $('label[id="' + $(ele).attr("name") + '-error"]').empty();
        });

        return that;
    };

    $.fn.remoteValidate = function (fnName, remoteUrl, msg, dataFn) {

        var that = this;

        if ($.validator.methods[fnName])
            delete $.validator.methods[fnName];

        $.validator.addMethod(fnName, function (value, element, param) {

            if (this.optional(element)) {
                return "dependency-mismatch";
            }

            var previous = this.previousValue(element),
                validator, data;

            if (!this.settings.messages[element.name]) {
                this.settings.messages[element.name] = {};
            }

            previous.originalMessage = msg; //this.settings.messages[element.name].remote;

            this.settings.messages[element.name].remote = previous.message;

            param = typeof param === "string" && { url: param } || param;

            if (previous.old === value) {
                return previous.valid;
            }

            previous.old = value;
            validator = this;
            this.startRequest(element);

            data = typeof dataFn == 'object' ? dataFn : typeof dataFn == 'function' ? dataFn() : {};

            data[$(element).attr('name')] = value;

            $.ajax($.extend(true, {
                url: remoteUrl,
                mode: "abort",
                port: "validate" + element.name,
                dataType: "json",
                data: data,
                aysnc: false,
                type: 'post',
                context: validator.currentForm,
                success: function (response) {
                    var valid = (response === true) || (response === "true"),
                        errors, message, submitted;

                    validator.settings.messages[element.name].remote = previous.originalMessage;
                    if (valid) {
                        submitted = validator.formSubmitted;
                        validator.prepareElement(element);
                        validator.formSubmitted = submitted;
                        validator.successList.push(element);
                        delete validator.invalid[element.name];
                        validator.showErrors();
                    } else {
                        errors = {};
                        message = response || validator.defaultMessage(element, "remote");
                        errors[element.name] = previous.message = $.isFunction(message) ? message(value) : message;
                        validator.invalid[element.name] = true;
                        validator.showErrors(errors);
                    }
                    previous.valid = valid;
                    validator.stopRequest(element, valid);
                }
            }, param));
            return "pending";
        });

        $(that).attr("data-val-remote", "").applyValidation(msg, fnName, false);
    };

    $.fn.validateFormSubmit = function () {

        var _frm = $(this);

        $.validator.unobtrusive.parse(_frm);

        if (_frm.length == 0 || $(_frm).prop('tagName').toLowerCase() !== 'form') {
            throw "Invalid From";
        }

        $(_frm).eleValidate();

        var validPendingTimeout;

        var _submitFormOnValid = function (frm) {

            var isValid = $(frm).eleValidate();

            var isPending = $(frm).validate().pendingRequest !== 0;

            if (isPending) {

                if (typeof validPendingTimeout !== "undefined") {

                    clearTimeout(validPendingTimeout);
                }

                validPendingTimeout = setTimeout(_submitFormOnValid, 100, frm);
            }

            if (isValid && !isPending) {

                $(frm).submit();

            } else if (console) {

                console.log("Invalid Form: Please fix the fields");
            }
        };

        _submitFormOnValid(_frm);
    };

    $.fn.customAjaxFormSubmit = function (url, onSuccess, postObj, onFailure, onComplete) {

        var frm = $(this);

        frm.removeData('validator');

        frm.removeData('unobtrusiveValidation');

        $.validator.unobtrusive.parse(frm);

        $.validator.methods.date = function () { return true; };

        $(frm).find(".optional").clearValidation();

        if (frm.length == 0) {
            throw "Invalid From";
        }

        $(frm).validate();

        var validPendingTimeout;

        var _submitFormOnValid = function (frm) {

            var isValid = $(frm).valid();

            var isPending = $(frm).validate().pendingRequest !== 0;

            if (isPending) {

                if (typeof validPendingTimeout !== "undefined") {

                    clearTimeout(validPendingTimeout);
                }

                validPendingTimeout = setTimeout(_submitFormOnValid, 1, frm);
            }

            if (isValid && !isPending) {

                console.log($(frm).validate());

                $.ajax({
                    type: 'POST',
                    url: url || $(frm).attr('action'),
                    contentType: "application/json",
                    data: JSON.stringify({ model: $(frm).getSerializeJsonObject() }),
                    async: true,
                    success: onSuccess ? onSuccess : console && function (data) { console.log(data); }
                });

            } else if (console) {

                console.log("Invalid Form: Please fix the fields");
            }
        };

        _submitFormOnValid(frm);
    };

    $.fn.submitFormData = function (url, onSuccess, onFailure, onComplete) {

        var frm = $(this);

        frm.removeData('validator');

        frm.removeData('unobtrusiveValidation');

        $.validator.unobtrusive.parse(frm);

        $.validator.methods.date = function () { return true; };

        $(frm).find(".optional").clearValidation();

        if (frm.length == 0) {
            throw "Invalid From";
        }

        $(frm).validate();

        var validPendingTimeout;

        var _submitFormOnValid = function (frm) {

            var isValid = $(frm).valid();

            var isPending = $(frm).validate().pendingRequest !== 0;

            if (isPending) {

                if (typeof validPendingTimeout !== "undefined") {

                    clearTimeout(validPendingTimeout);
                }

                validPendingTimeout = setTimeout(_submitFormOnValid, 1, frm);
            }

            if (isValid && !isPending) {
                $.ajax({
                    type: 'POST',
                    url: url || $(frm).attr('action'),
                    data: new FormData(frm[0]),
                    async: true,
                    success: onSuccess ? onSuccess : console && function (data) { console.log(data); },
                    cache: false,
                    contentType: false,
                    processData: false
                });

            } else if (console) {

                console.log("Invalid Form: Please fix the fields");
            }
        };

        _submitFormOnValid(frm);
    };

    $.fn.getSerializeJsonObject = function () {

        var that = this;

        var allFields = [];

        if ($(that).prop("tagName").toLowerCase() !== "form") {

            allFields = $(that).divSerializeArray();

        } else {

            allFields = $(that).serializeArray();
        }


        var data = {};

        var listIndexes = [];

        var listItems = [];

        $.each(allFields, function (i, ele) {

            if (ele.name.indexOf('.index') != -1 || ele.name.indexOf('.Index') != -1) {

                listIndexes.push(ele);
            }

            else if ($(ele.name).getIndexes('[').length && $(ele.name).getIndexes(']').length) {

                listItems.push(ele);
            }
            else if (data[ele.name]) {

                var values = data[ele.name];

                if ($.isArray(values)) {

                    values.push(ele.value);

                } else {

                    values = [];
                    values.push(data[ele.name]);
                    values.push(ele.value);
                }

                data[ele.name] = values;

            } else {
                data[ele.name] = ele.value;
            }

        });

        if (listIndexes.length && listItems.length) {

            $.each(listIndexes, function (i, ele) {

                var _key = ele.name.indexOf('.index') != -1 ? ele.name.replace(".index", "") : ele.name.replace(".Index", "");

                var key = _key + "[" + ele.value + "]";

                if (data[_key] == undefined) {

                    data[_key] = new Array();
                }

                var valObj = new Object();

                $.each(listItems, function (i, item) {

                    if (item.name.indexOf(key) != -1) {

                        valObj[item.name.replace(key + ".", "")] = item.value;
                    }

                });

                data[_key].push(valObj);

            });

        }

        return data;
    };

    $.fn.getIndexes = function (find) {

        var source = this.selector;

        var result = [];

        if (typeof source == 'string') {

            for (i = 0; i < source.length; ++i) {

                if (source.substring(i, i + find.length) == find) {
                    result.push(i);
                }
            }
            return result;
        }

        if (source instanceof Array) {

            for (i = 0; i < source.length; ++i) {

                if (source[i] == find) {
                    result.push(i);
                }
            }

            return result;
        }
    };

    $.fn.customAjax = function (url, postObj, onSuccess) {

        var sendObj = $.extend({}, $(this).getSerializeJsonObject(), postObj || {});

        var ajaxObj = $.ajax({
            type: 'POST',
            url: url,
            contentType: "application/json",
            data: JSON.stringify({ model: sendObj }),
            async: true,
            success: onSuccess
        });

        return ajaxObj;

    };

    $.fn.divSerializeArray = function () {


        var rsubmitterTypes = /^(?:submit|button|image|reset|file)$/i,
            rsubmittable = /^(?:input|select|textarea|keygen)/i,
            rcheckableType = (/^(?:checkbox|radio)$/i),
            rCRLF = /\r?\n/g;

        var array = $(this).find("input,textarea,select").filter(function () {

            var type = this.type;

            return this.name && !jQuery(this).is(":disabled") &&
                rsubmittable.test(this.nodeName) && !rsubmitterTypes.test(type) &&
                (this.checked || !rcheckableType.test(type));
        })
        .map(function (i, elem) {
            var val = jQuery(this).val();

            return val == null ?
                null :
                jQuery.isArray(val) ?
                    jQuery.map(val, function (val) {
                        return { name: $(elem).data('defaultName') || elem.name, value: val.replace(rCRLF, "\r\n") };
                    }) :
    				{ name: $(elem).data('defaultName') || elem.name, value: val.replace(rCRLF, "\r\n") };
        }).get();

        return array;
    };

    $.fn.getFormElements = function () {

        var rsubmitterTypes = /^(?:submit|button|image|reset|file)$/i,
            rsubmittable = /^(?:input|select|textarea|keygen)/i,
            rcheckableType = (/^(?:checkbox|radio)$/i),
            rCRLF = /\r?\n/g;

        var array = $(this).find("input,textarea,select").filter(function () {

            var type = this.type;

            return this.name && !jQuery(this).is(":disabled") &&
                rsubmittable.test(this.nodeName) && !rsubmitterTypes.test(type) &&
                (this.checked || !rcheckableType.test(type)) && this.name.indexOf(".index") == -1 && this.name.indexOf(".Index") == -1;
        });

        return $(array);
    };

    $.fn.clear = function () {

        var that = this;

        if ($(that).prop("tagName").toLowerCase() === "form") {

            that.clear();

        } else {

            $(that).val("");
        }

        return $(that);
    };

    $.fn.disableElements = function (notSelector) {

        var block = $(this);

        var elements = $(block).find("input:not(:hidden),textarea,select,button,a").not(notSelector);

        $(elements).attr('disabled', 'disabled');
        $(elements).prop('disabled', true);

        $("input:checkbox,input:radio", block).parent('.btn').addClass("disabled");

        $(".input-group-addon", block).css("pointer-events", "none");

        var tbs = $("input.selectized,select.selectized", block).filter(function () {

            if ($(this)[0].selectize)
                return this;
        });

        $.each(tbs, function (i, e) { $(e)[0].selectize.disable(); });

        return block;
    };

    $.fn.enableElements = function () {

        var block = $(this);

        var elements = $(block).find("input,textarea,select,button,a");

        $(elements).removeAttr('disabled');
        $(elements).prop('disabled', false);


        $("input:checkbox,input:radio", block).parent('.btn').removeClass("disabled");

        var tbs = $("input.selectized,select.selectized", block).filter(function () {

            if ($(this)[0].selectize) {
                return this;
            }
        });

        $.each(tbs, function (i, e) { $(e)[0].selectize.enable(); });

        return block;

    };

    $.fn.clearElements = function (elements) {

        var block = $(this);

        if (elements == undefined) {

            elements = $(block).find("input:not(:checkbox),textarea,select");
        }

        elements.val('');

        $(block).find("input:checkbox").attr("checked", false);

        var tbs = $(":input", block).filter(function () {
            if ($(this)[0].selectize)
                return this;
        });

        $.each(tbs, function (i, e) { $(e)[0].selectize.clear(); });

        return block;
    };

    $.fn.scrollUp = function () {

        var that = $(this);

        $(that).stop().animate({ scrollTop: 0 }, "slow");

        return that;
    }

    $.fn.scrollDown = function () {

        var that = $(this);

        var height = $(that)[0].scrollHeight - $(that).height();

        $(that).stop().animate({ scrollTop: height }, "slow");

        return that;
    };

    $.fn.scrollView = function () {

        var that = $(this);

        $(that)[0].scrollIntoView();

        return that;
    };

    $.fn.highlight = function (query) {

        if (!query) {
            query = 2000;
        }

        if ($.isNumeric(query)) {

            var that = this;

            $(that)[0].scrollIntoView();

            $(that).addClass("textHighlight");

            setTimeout(function () {

                $(that).removeClass("textHighlight");

            }, query);

        } else if (typeof query == 'string') {

        }

    };

    $.fn.fillObject = function (Obj) {

        if (typeof Obj != 'object') {
            throw "Parameter is not a Object";
        }

        var content = $(this);

        $.each(Obj, function (k, v) {

            var ele = $(content).find('[name="' + k + '"]');

            if (!ele.length) {

                ele = $(content).find('[data-default-name="' + k + '"]');
            }

            if (ele.length && $(ele)[0].selectize) {

                setTimeout(function () {

                    if ($(ele).data('selectizeUrl') && ($(ele).data('selectizePreload') == 'False' || $(ele).data('selectizePreload') == 'false')) {

                        $(ele)[0].selectize.load(function (callback) {

                            var _Url = $(ele).data("selectizeUrl");

                            var Url = ServerUrl + $.trim(_Url);

                            var paramFn = $(ele).data("selectizeDatafn") || $(ele).data("selectizeParams");

                            var _callBack = paramFn != undefined ? window[paramFn] : "";

                            var paramObj = null;

                            if (_callBack != undefined && $.trim(_callBack).length) {

                                paramObj = _callBack();
                            }

                            $.get(Url, $.extend({}, { searchText: "" }, paramObj), callback).fail(callback).done(function () {

                                $(ele)[0].selectize.setValue(v.toString().split(','));
                            });
                        });

                    } else {

                        $(ele)[0].selectize.setValue(v);
                    }

                    if ($(ele).attr('id').toLowerCase() == 'title') {
                        SetPatientName(Obj);
                    }

                }, 1000);

            }


            if ($(ele).parents('.date').data('datepicker') != undefined || $(ele).data('datepicker') != undefined) {

                var _dob = Obj.DOB, setDate;

                if (!(_dob == null || _dob === "null" || _dob.length == 0)) {

                    var cond = _dob.match(/(-|\d)\d*.*\d/g); // /(-|\d)\d*.*\d/.exec(_dob)

                    if (cond[0].match(/-/g) != undefined && cond[0].match(/-/g) != null) {

                        if (cond[0].match(/-/g).length > 1) {
                            var splited = Obj.DOB.split("-");

                            setDate = new Date(splited[2], splited[1] - 1, splited[0]);
                        }
                        else
                            setDate = new Date(parseInt(cond[0]));
                    }
                    else
                        setDate = new Date(parseInt(cond[0]));

                    if ($(ele).data('datepicker') != undefined) {

                        $(ele).datepicker('setDate', setDate);
                    }
                    else {
                        $(ele).parents('.date').datepicker('setDate', setDate);
                    }
                }

            }
            else {
                $(ele).val(v == '0' ? "" : v).focus().focusout();
            }


        });

        return content;
    };

    $.fn.numberInput = function () {

        var elements = $(this);

        $.each(elements, function () {

            $(this).on("keypress", function (event) {

                var charCode = event.keyCode || event.which;

                var allowKeyPress = event.ctrlKey || charCode == 8 || charCode == 17 || (charCode >= 35 && charCode <= 40);

                if (allowKeyPress) {

                    return true;

                } else {

                    return !(charCode < 48 || charCode > 57);
                }

            })

            $(this).bind('paste', null, function (e) {
                return e.keyCode !== undefined;
            })
        });

        return elements;
    };

    $.fn.decimalInput = function () {

        var elements = $(this);

        $.each(elements, function () {

            $(this).on("keypress", function (event) {

                var charCode = event.keyCode || event.which;

                var allowKeyPress = charCode == 8 || charCode == 17 || (charCode >= 48 && charCode <= 57) || charCode == 46;

                if (allowKeyPress) {
                    return true;
                }
                else {

                    var element = this;

                    if ((charCode != 46 || $(element).val().indexOf('.') != -1) &&      // “.” CHECK DOT, AND ONLY ONE.
                        (charCode < 48 || charCode > 57))
                        return false;


                    return true;
                }
            });


            $(this).bind('paste', null, function (e) {
                return e.keyCode !== undefined;
            })
        });

        return elements;
    };

    $.fn.negativeInput = function () {

        var elements = $(this);

        $.each(elements, function () {

            $(this).on("keypress", function (event) {

                var charCode = event.keyCode || event.which;

                var allowKeyPress = charCode == 8 || charCode == 17 || (charCode >= 35 && charCode <= 40) || charCode == 46;

                if (allowKeyPress) {
                    return true;

                } else {

                    var element = this;

                    if ((charCode != 45 || $(element).val() !== '') &&      // “-” CHECK MINUS For -Ve Values, AND ONLY ONE.
                        (charCode < 48 || charCode > 57))
                        return false;

                    return true;
                }

            });


            $(this).bind('paste', null, function (e) {
                return e.keyCode !== undefined;
            })
        });

        return elements;

    };

    $.fn.negativeDecimalInput = function () {

        var elements = $(this);

        $.each(elements, function () {

            $(this).on("keypress", function (event) {

                var charCode = event.which || event.keyCode;

                var allowKeyPress = charCode == 8 || charCode == 17 || (charCode >= 35 && charCode <= 40) || charCode == 46;

                if (event.shiftKey)
                    return false;

                if (allowKeyPress) {
                    return true;
                }
                else {

                    var element = this;

                    if ((charCode != 45 || $(element).val() !== '') &&      // “-” CHECK MINUS For -Ve Values, AND ONLY ONE.
                        (charCode != 46 || $(element).val().indexOf('.') != -1) &&      // “.” CHECK DOT, AND ONLY ONE.
                        (charCode < 48 || charCode > 57))
                        return false;

                    return true;
                }
            });

            $(this).bind('paste', null, function (e) {
                return e.keyCode !== undefined;
            })
        });

        return elements;
    };

    $.fn.clearSelectize = function (clearOptions) {

        var that = $(this);

        $.each($(that).filter('.selectized'), function (i, ele) {

            $(ele)[0].selectize.clear(true);

            if (clearOptions === true)
                $(ele)[0].selectize.clearOptions();

        });


        $.each($(that).find('.selectized'), function (i, ele) {

            $(ele)[0].selectize.clear(true);

            if (clearOptions === true)
                $(ele)[0].selectize.clearOptions();

        });

        return that;
    };

    $.fn.toQueryString = function (prefix) {

        var obj = this;

        var serialize = function (obj, prefix) {

            var str = [];

            for (var p in obj) {

                if (obj.hasOwnProperty(p)) {

                    var k = prefix ? prefix + "[" + p + "]" : p,
                        v = obj[p];

                    str.push(typeof v == "object" ?
                      serialize(v, k) :
                      encodeURIComponent(k) + "=" + encodeURIComponent(v));
                }
            }
            return str.join("&");
        }

        return serialize(obj, prefix);

    };

    $.fn.outerHTML = function () {
        return $(this).clone().wrap('<div></div>').parent().html();
    };

    $.fn.disable = function () {

        var that = $(this);

        $(that).attr('disabled', 'disabled').prop('disabled', true);

        $(that).parent('.btn').addClass("disabled");

        return that;
    };

    $.fn.enable = function () {

        var that = $(this);

        $(that).removeAttr('disabled').prop('disabled', false);

        $(that).parent('.btn').removeClass("disabled");

        return that;
    };

    $.fn.check = function () {

        var that = $(this);

        $.each(that, function (i, ele) {

            var prop = $(ele).prop('type');

            if (prop == 'checkbox' || prop == 'radio') {

                $(ele).prop("checked", true).attr('checked', 'checked');
            }
        });

        return that;
    };

    $.fn.uncheck = function () {

        var that = $(this);

        $.each(that, function (i, ele) {

            var prop = $(ele).prop('type');

            if (prop == 'checkbox' || prop == 'radio') {

                $(ele).prop("checked", false).removeAttr('checked');
            }
        });

        return that;
    };

    $.fn.highlightClass = function (cls, sec) {

        var that = $(this);

        if (sec == undefined) {
            sec = 3000;
        }

        $(that).addClass(cls);

        setTimeout(function () { $(that).removeClass(cls); }, sec);

        return that;
    };


    $.fn.cFocus = function () {

        return this.each(function () {

            $(this).focus()

            // If this function exists...
            if (this.setSelectionRange) {
                // ... then use it (Doesn't work in IE)

                // Double the length because Opera is inconsistent about whether a carriage return is one character or two. Sigh.
                var len = $(this).val().length * 2;

                this.setSelectionRange(len, len);

            } else {
                // ... otherwise replace the contents with itself
                // (Doesn't work in Google Chrome)

                $(this).val($(this).val());

            }

            // Scroll to the bottom, in case we're in a tall textarea
            // (Necessary for Firefox and Google Chrome)
            this.scrollTop = 999999;

        });

    };


})(jQuery);
