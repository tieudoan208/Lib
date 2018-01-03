/// <reference path="Common.js" />

/*
quannm: dropdowlist
*/

Type.registerNamespace('Lib');

Lib.drop = function (element) {
    Lib.drop.initializeBase(this, [element]);

    this._onClientChange = null;
    this._onClientBlur = null;
    this._saveType = null;
    this._dataValue = null;
    this._dataText = null;
    this._Name = null;
    this._IsInputData = null;
    this._IsReset = null;

}

Lib.drop.prototype = {
    initialize: function () {
        Lib.drop.callBaseMethod(this, 'initialize');

        //debugger;
        //Thêm event
        $addHandlers(this.get_element(),
                          {
                              'keydown': this._onKeyDown,
                              'blur': this._onBlur,
                              'change': this._onChange
                          },
                          this);
    },
    dispose: function () {
        //Xóa mọi event
        $clearHandlers(this.get_element());
        Lib.drop.callBaseMethod(this, 'dispose');
    },
    _onKeyDown: function (e) {
        var control = this.get_element();
        var keycode = Common.getKeyCode(e);
        if (keycode == Sys.UI.Key.enter) {
            Common.onNextControl(control, e);
            e.preventDefault();
            return false;
        }
    },
    _onBlur: function (e) {
        if (!Common.isNullOrEmty(this._onClientBlur))
            window.setTimeout(_onClientBlur, 0);
        e.preventDefault();
    },
    _onChange: function (e) {
        if (!Common.isNullOrEmty(this._onClientChange))
            window.setTimeout(this._onClientChange, 0);
        e.preventDefault();
    },
    get_onClientBlur: function () {
        return this._onClientBlur;
    },

    set_onClientBlur: function (value) {
        if (this._onClientBlur !== value) {
            this._onClientBlur = value;
        }
    },
    get_onClientChange: function () {
        return this._onClientChange;
    },

    set_onClientChange: function (value) {
        if (this._onClientChange !== value) {
            this._onClientChange = value;
        }
    },
    get_saveType: function () {
        return this._saveType;
    },

    set_saveType: function (value) {
        if (this._saveType !== value) {
            this._saveType = value;
        }
    },
    dataBin: function (val) {
        var dropID = $("#" + this.get_element().id);
        if (this._dataText == null || this._dataValue == null) return;
        dropID.children().remove();
        var b_val = this._dataValue;
        var b_txt = this._dataText;
        var aJSon = Common.stringToJson(val);
        $.each(aJSon, function (val, text) {
            var value = text[b_val.toString()];
            var txt = text[b_txt.toString()];
            dropID.append($('<option></option>').val(value).html(txt));
        });
    },
    get_dataValue: function () {
        return this._dataValue;
    },

    set_dataValue: function (value) {
        if (this._dataValue !== value) {
            this._dataValue = value;
        }
    },
    get_dataText: function () {
        return this._dataText;
    },

    set_dataText: function (value) {
        if (this._dataText !== value) {
            this._dataText = value;
        }
    },
    get_name: function () {
        return this._Name;
    }
    ,
    set_name: function (value) {
        this._Name = value;
    },
    get_isInputData: function () {
        return this._isInputData;
    }
    ,
    set_isInputData: function (value) {
        this._isInputData = value;
    },
    get_isReset: function () {
        return this._IsReset;
    }
    ,
    set_isReset: function (value) {
        if (this._IsReset !== value)
            this._IsReset = value;
    }
}

Lib.drop.descriptor = {
    properties: [{ name: 'onClientBlur', type: String },
                 { name: 'onClientChange', type: String },
                 { name: 'saveType', type: String },
                 { name: 'dataValue', type: String },
                 { name: 'dataText', type: String },
                 { name: 'name', type: String },
                 { name: 'isInputData', type: String },
                 { name: 'isReset', type: String }
    ]
}

Lib.drop.registerClass('Lib.drop', Sys.UI.Control);


if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();