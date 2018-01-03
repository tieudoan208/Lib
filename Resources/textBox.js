/// <reference path="Common.js" />
/// <reference path="Until.js" />
/// <reference path="jquery-1.11.0.js" />
/// <reference path="MicrosoftAjax.js" />

///Other: quannm

Type.registerNamespace('Lib');

Lib.textBox = function (element) {
    Lib.textBox.initializeBase(this, [element]);
    this._typeData = null;
    this._onClientChange = null;
    this._upperCase = null;
    this._lke = null;
    this._onClientBlur = null;
    this._onClientClick = null;
    this._onClinetDblClick = null;
    this._urlReference = null;
    this._checkDate = null;
    this._msg = null;
    this._name = null;
    this._isInputData = null;
    this._saveTypeData = null;
    this._decimal = null;
    this._isReset = null;
    this._maxLength = null;


}
//Thuộc tính
Lib.textBox.prototype = {
    initialize: function () {
        Lib.textBox.callBaseMethod(this, 'initialize');
        //Thêm event
        $addHandlers(this.get_element(),
                          {
                              // 'keydown': this._onKeyDown,
                              'focus': this._onFocus,
                              'blur': this._onBlur,
                              // 'keypress': this._onKeyPress,
                              'click': this._onClick,
                              'dblclick': this._onDblClick
                          },
                          this);

        var control = this.get_element();
        var typeData = this.get_typeData();
        var lke = this.get_lke();
        var upperCase = this.get_upperCase();
        var nameControl = this.get_name();
        var msg = this.get_msg();
        var checkDate = this.get_checkDate();
        var urlReference = this.get_urlReference();
        var decimal = this.get_decimal();
        var onClientChange = this.get_clientChange();


        if (typeData == "Month" || typeData == "Date")
            $(control).css("text-align", "center");
        else if (lke != "" && lke != null) {
            $(control).css("text-align", "center");
            $(control).css("cursor", "pointer");
        }
        $(this.get_element()).change(function (e) {
            if (!Common.isNullOrEmty(onClientChange))
                window.setTimeout(onClientChange, 0);
            e.preventDefault();
        });
        $(this.get_element()).keypress(function (e) {
            var keyCode = Common.getKeyCode(e, "keypress");
            if (keyCode <= 0 || keyCode == Sys.UI.Key.enter) return;
            $(this).attr("isChange", "true");
            var position = Common.getCaret(control);
            //cat dng select
            var selectStart = control.selectionStart;
            var selectEnd = control.selectionEnd;
            if (keyCode != 0 && selectStart < selectEnd) {
                var newVal = $(control).val().substr(0, selectStart) + $(control).val().substr(selectEnd);
                $(control).val(newVal);
            }
            if (typeData.toString() == "String") {
                if (lke != "" && lke != null) {
                    var bVal = control.value;
                    $(control).val(Common.transformValue(bVal, lke));
                    Common.setlectRang(control, 0, 0);
                    e.preventDefault();
                    return false;
                }
                //kiem tra maxLength
                
                if (upperCase == true) {
                    var newVal = $(control).val().toString().substring(0, position) +
                    Common.formCodeToChar(keyCode).toUpperCase() + $(control).val().substring(position);
                    $(control).val(newVal);
                    Common.setlectRang(control, position + 1, position + 1);
                }
                else {
                    control.value = $(control).val().substring(0, position) +
                    Common.formCodeToChar(keyCode) + $(control).val().substring(position);
                    Common.setlectRang(control, position + 1, position + 1);
                }

                e.preventDefault();
                return false;
            }
            if (typeData.toString() == "Number") {
                if (keyCode != 46)
                    if (!Common.isKeyDigit(keyCode)) { e.preventDefault(); return true; }
            }
            if (typeData.toString() == "Money") {
                var newchar = Common.formCodeToChar(keyCode);
                var old = $(control).val();
                var dec = parseInt(decimal);
                var check = "-0123456789";
                if (dec > 0)
                    check = check + ".";
                if (check.indexOf(newchar) < 0 || (newchar == "0" && old == "0")
                             || (newchar == "." && old.indexOf(".") >= 0)) {
                    return false;
                }
                var position = Common.getCaret(control);
                old = old.substr(0, position) + newchar + old.substr(position);
                newchar = Common.stringToFormatNumber(old, dec);
                $(control).val(newchar);
                e.preventDefault();
                e.stopImmediatePropagation();
                return false;
            }
            else {
                if (typeData.toString() == "Date" && position >= 10) { e.preventDefault(); return true; }
                if (typeData.toString() == "Month" && position >= 7) { e.preventDefault(); return true; }
                if (keyCode != 46)
                    if (!Common.isKeyDigit(keyCode)) { e.preventDefault(); return true; }
                var charLost = "";
                if (position == 2 || (position == 5 && typeData.toString() == "Date")) {
                    if (control.value.toString().substring(position, position + 1) == "/")
                        charLost = "";
                    else
                        charLost = "/";
                    position++;
                }
                var newVal = control.value.toString().substring(0, position) + charLost +
                     Common.formCodeToChar(keyCode) + control.value.toString().substring(position + 1);

                $(control).val(newVal);
                Common.setlectRang(control, position + 1, position + 1);
                e.preventDefault();
                return false;
            }
        });

        $(this.get_element()).keydown(function (e) {
            var position = Common.getCaret(control);
            var keyCode = Common.getKeyCode(e, "keydown");
            if (keyCode == 46) keyCode = 127;
            switch (keyCode) {
                case Sys.UI.Key.backspace:
                    {
                        if (position <= 0) { return true; }
                        if ("Number,String".indexOf(typeData.toString()) >= 0) return true;
                        else if (typeData == "Money")
                        {
                            var dec = parseInt(decimal);
                            var char = Common.formCodeToChar(keyCode);
                            if (char == "," && position < $(control).val().length)
                                position=position-1;
                            var newchar = $(control).val().substring(0, position - 1) + $(control).val().substring(position);
                            var newcharNumber = Common.stringToNumber(newchar).toString();
                            var format = Common.stringToFormatNumber(newcharNumber, dec);
                            $(control).val(format);
                            Common.setlectRang(control, position, position);
                            e.preventDefault();
                            return true;
                        }
                        
                        if (position == 3 || (position == 6 && typeData.toString() == "Date"))
                            position = position - 1;
                        var val = $(control).val().substring(0, position - 1) + "_" + $(control).val().substring(position);
                        $(control).val(val);
                        Common.setlectRang(control, position - 1, position - 1);
                        e.preventDefault();
                        break;
                    }
                case Sys.UI.Key.del:
                    if ("Number,String".indexOf(typeData.toString()) >= 0) return true;
                    else if (typeData == "Money") {
                        var dec = parseInt(decimal);
                        var char = Common.formCodeToChar(keyCode);
                        if (char == "," && position < $(control).val().length)
                            position = position + 1;
                        var newchar = $(control).val().substring(0, position) + $(control).val().substring(position+1);
                        var newcharNumber = Common.stringToNumber(newchar).toString();
                        var format = Common.stringToFormatNumber(newcharNumber, dec);
                        $(control).val(format);
                        Common.setlectRang(control, position, position);
                        e.preventDefault();
                        return true;
                    }
                    var check = false;
                    if (typeData.toString() == "Date" && position >= 10) { e.preventDefault(); return true; }
                    if (typeData.toString() == "Month" && position >= 7) { e.preventDefault(); return true; }
                    if (position == 2 || (position == 5 && typeData.toString() == "Date")) {
                        position = position + 1;
                    }
                    var val = $(control).val().substring(0, position) + "_" + $(control).val().substring(position + 1);
                    var s = $(control).val().replace(/\//g, "");
                    var valInput = s.substring(0, 1) + "/" + s.substring(2, 4) + "/" + s.substring(4);
                    $(control).val(val);
                    Common.setlectRang(control, position + 1, position + 1);
                    e.preventDefault();
                    break;
                case Sys.UI.Key.tab:
                    if (e.shiftKey) {
                        Common.onPreviousControl(control, e);
                        e.preventDefault();
                    }
                    else
                        Common.onNextControl(control, e);
                    e.preventDefault();
                    break;
                case Sys.UI.Key.tab:
                case Sys.UI.Key.enter:
                    if (nameControl == nameControl.toUpperCase()) {
                        if ($(control).val() == "") {
                            Msg.show("Cảnh báo", "Không được nhập trống " + (msg == null ? "" : msg));
                            return false;
                        }
                    }
                    if (checkDate) {
                        var b_value = $(control).val();
                        if (b_value.indexOf("_") >= 0) {
                            Msg.show("Cảnh báo", "Giá trị nhập không hợp lệ");
                            return false;
                        }
                        var a_value = b_value.split('/');
                        var b_thang = 0, b_nam = 0;
                        var ok = false;
                        if (typeData.toString() == "Date") {
                            b_thang = parseInt(a_value[1].toString());
                            b_nam = a_value[2].toString();
                            ok = true;
                        }
                        else {
                            b_thang = parseInt(a_value[0].toString());
                            b_nam = a_value[1].toString();
                            ok = false;
                        }

                        if (b_thang < 0 || b_thang > 12) {
                            Msg.show("Cảnh báo", "Tháng không hợp lệ");
                            return false;
                        }

                        if (b_nam.length <= 3) {
                            Msg.show("Cảnh báo", "Năm không hợp lệ");
                            return false;
                        }
                        if (ok) {
                            var b_ngay = Common.daysOfMonth(b_thang, b_nam);
                            var day = parseInt(a_value[0]);
                            if (day < 0 || day > b_ngay) {
                                Msg.show("Cảnh báo", "Ngày không hợp lệ");
                                return false;
                            }
                        }

                    }
                    if ($(this).attr("isChange") == "true") {
                        $(this).trigger("change");
                        $(this).attr("isChange", "false");
                    }
                    Common.onNextControl(control, e);
                    break
                case 112:
                    // debugger;
                    document.onhelp = new Function("return false;");
                    if (navigator.userAgent.search(/msie/i) == -1) { e.stopPropagation(); e.preventDefault(); }
                    if ("aspx".indexOf(urlReference) >= 0) {
                        //Popup trang web
                        OpenWindow(urlReference, "", $(control).val());
                    }
                    else {
                        if (Common.NVL(urlReference) == "")
                            return false;
                        window[urlReference](this);
                    }

                    return false;
            }
        });

    },
    dispose: function () {
        //Xóa mọi event
        $clearHandlers(this.get_element());
        Lib.textBox.callBaseMethod(this, 'dispose');
    },
    _onClick: function (e) {

        var control = this.get_element();
        var keycode = Common.getKeyCode(e);
        var bVal = control.value;
        if (!Common.isNullOrEmty(this._onClientClick))
            window.setTimeout(this._onClientClick, 0);

        if (this._lke != "" && this._lke != null) {
            $(control).val(Common.transformValue(bVal, this._lke));
            Common.setlectRang(control, 0, 0);
        }
        e.preventDefault();
    },

    _onFocus: function (e) {
        var control = this.get_element();
        if ("Number,String".indexOf(this._typeData.toString()) >= 0) return true;
        if (this._typeData.toString() == "Month" && !Common.containsDigits(control.value))
            control.value = "__/____"
        else if (this._typeData.toString() == "Date" && !Common.containsDigits(control.value))
            control.value = "__/__/____";
        Common.setlectRang(control, 0, 0);
        e.preventDefault();
        return false;
    },

    _onBlur: function (b_control) {
        var control = this.get_element();
        if (!Common.isNullOrEmty(this._onClientBlur))
            window.setTimeout(this._onClientBlur, 0);
        if ("Number,String".indexOf(this._typeData.toString()) >= 0) return true;
        if (this._typeData.toString() == "Date" || this._typeData.toString() == "Month")
            if (!Common.containsDigits(control.value))
                control.value = "";
    },
    _onDblClick: function (e) {
        if (!Common.isNullOrEmty(this._onClinetDblClick))
            window.setTimeout(this._onClinetDblClick, 0);
        e.preventDefault();
    },
    get_value: function () {
        var control = this.get_element();
        return control.value;
    },
    set_value: function (value) {
        $(this.get_element()).val(value);
    },
    get_typeData: function () {
        return this._typeData;
    },

    set_typeData: function (value) {
        if (this._typeData !== value) {
            this._typeData = value;
        }
    },
    get_upperCase: function () {
        return this._upperCase;
    },

    set_upperCase: function (value) {
        if (this._upperCase !== value) {
            this._upperCase = value;
        }
    },
    get_lke: function () {
        return this._lke;
    },

    set_lke: function (value) {
        if (this._lke !== value) {
            this._lke = value;
        }
    },
    get_clientChange: function () {
        return this._onClientChange;
    },
    set_clientChange: function (value) {
        if (this._onClientChange != value) {
            this._onClientChange = value;
        }
    },
    get_onClientBlur: function () {
        return this._onClientBlur;
    },

    set_onClientBlur: function (value) {
        if (this._onClientBlur !== value) {
            this._onClientBlur = value;
        }
    },
    get_onClientClick: function () {
        return this._onClientClick;
    },

    set_onClientClick: function (value) {
        if (this._onClientClick !== value) {
            this._onClientClick = value;
        }
    },
    get_onClinetDblClick: function () {
        return this._onClinetDblClick;
    },

    set_onClinetDblClick: function (value) {
        if (this._onClinetDblClick !== value) {
            this._onClinetDblClick = value;
        }
    },
    get_urlReference: function () {
        return this._urlReference;
    },
    set_urlReference: function (value) {
        if (this._urlReference !== value) {
            this._urlReference = value;
        }
    },
    get_checkDate: function () {
        return this._checkDate;
    },

    set_checkDate: function (value) {
        if (this._checkDate !== value) {
            this._checkDate = value;
        }
    },
    get_msg: function () {
        return this._msg;
    },
    set_msg: function (value) {
        if (this._msg !== value) {
            this._msg = value;
        }
    },
    get_name: function () {
        return this._name;
    }
    ,
    set_name: function (value) {
        if (this._name !== value)
            this._name = value;
    },
    get_isInputData: function () {
        return this._isInputData;
    }
    ,
    set_isInputData: function (value) {
        if (this._isInputData !== value)
            this._isInputData = value;
    },
    get_saveTypeData: function () {
        return this._saveTypeData;
    }
    ,
    set_saveTypeData: function (value) {
        if (this._saveTypeData !== value)
            this._saveTypeData = value;
    },
    get_decimal: function () {
        return this._decimal;
    }
    ,
    set_decimal: function (value) {
        if (this._decimal !== value)
            this._decimal = value;
    },
    get_isReset: function () {
        return this._isReset;
    }
    ,
    set_isReset: function (value) {
        if (this._isReset !== value)
            this._isReset = value;
    },
    get_maxLength: function () {
        return this._isReset;
    }
    ,
    set_maxLength: function (value) {
        if (this._maxLength !== value)
            this._maxLength = value;
    }
}
Lib.textBox.descriptor = {
    properties: [{ name: 'typeData', type: String },
                 { name: 'clientChange', type: String },
                 { name: 'upperCase', type: String },
                 { name: 'lke', type: String },
                 { name: 'onClientBlur', type: String },
                 { name: 'onClientClick', type: String },
                 { name: 'onClinetDblClick', type: String },
                 { name: 'urlReference', type: String },
                 { name: 'checkDate', type: String },
                 { name: 'msg', type: String },
                 { name: 'name', type: String },
                 { name: 'isInputData', type: String },
                 { name: 'saveTypeData', type: String },
                 { name: 'decimal', type: String },
                 { name: 'isReset', type: String },
                 { name: 'maxLength', type: String }
    ]
}

Lib.textBox.registerClass('Lib.textBox', Sys.UI.Control);


if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();