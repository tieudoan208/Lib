/// <reference path="Common.js" />


Type.registerNamespace('Lib');

Lib.checkBox = function (element) {
    Lib.checkBox.initializeBase(this, [element]);

    this._onClientClick = null;
    this._name = null;
    this._isInputData = null;
    this._isReset = null;

}

Lib.checkBox.prototype = {
    initialize: function () {
        Lib.checkBox.callBaseMethod(this, 'initialize');
        $addHandlers(this.get_element(),
                          {
                              'keydown': this._onKeyDown,
                              'click': this._onClick
                          },
                          this);

    },
    dispose: function () {
        //Xóa mọi event
        $clearHandlers(this.get_element());
        Lib.checkBox.callBaseMethod(this, 'dispose');
    },
    _onClick: function (e) {
        if (!Common.isNullOrEmty(this._onClientClick))
            window.setTimeout(this._onClientClick, 0);
    },
    _onKeyDown: function (e) {
        var control = this.get_element();
        var keycode = Common.getKeyCode(e);
        if (keycode == Sys.UI.Key.enter) {
            Common.onNextControl(control, e);
        }
    },
    get_onClientClick: function () {
        return this._onClientClick;
    },

    set_onClientClick: function (value) {
        if (this._onClientClick !== value)
            this._onClientClick = value;
    },
    get_name: function () {
        return this._name;
    }
    ,
    set_name: function (value) {
        this._name = value;
    },
    get_isInputData: function () {
        return this._isInputData;
    }
    ,
    set_isInputData: function (value) {
        this._isInputData = value;
    },
    get_isReset: function () {
        return this._isReset;
    }
    ,
    set_isReset: function (value) {
        if (this._isReset !== value)
            this._isReset = value;
    },
    get_saveTypeData: function()
    {
        return "String";
    }
}

Lib.checkBox.descriptor = {
    properties: [
        { name: 'onClientClick', type: String },
        { name: 'name', type: String },
        { name: 'isInputData', type: String },
        { name: 'isReset', type: String }
    ]
}

Lib.checkBox.registerClass('Lib.checkBox', Sys.UI.Control);


if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();