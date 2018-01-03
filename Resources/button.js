/// <reference path="Common.js" />

/*
quannm:button
*/

Type.registerNamespace('Lib');

Lib.button = function (element) {
    Lib.button.initializeBase(this, [element]);

    this._onClientClick = null;
    this._onClientDblClick = null;
    this._width = null;
    this._text = null;
    this._cssClass = null;
}

Lib.button.prototype = {
    initialize: function () {
        Lib.button.callBaseMethod(this, 'initialize');
        $addHandlers(this.get_element(),
                          {
                              'click': this._onClick,
                              'dblclick': this._onbdlclick
                          },
                          this);

    },
    dispose: function () {
        $clearHandlers(this.get_element());
        Lib.button.callBaseMethod(this, 'dispose');
    },
    _onClick: function (e) {
        var click = this.get_onClientClick();
        if (!Common.isNullOrEmty(this._onClientClick)) {
            var flag = true;
            if ($(this._element).attr("icon").indexOf("DEL") >= 0) {
                flag = false;
                Msg.confirm(function (val) {
                    if (val) {
                        Msg.wait();
                        window.setTimeout(click, 0);
                        e.preventDefault();
                        return false;
                    }
                });

            }
            if (flag) {
                Msg.wait();
                window.setTimeout(click, 0);
            }
        }
        e.preventDefault();
        return false;
    },
    _onbdlclick: function (e) {
        if (!Common.isNullOrEmty(this._onClientDblClick))
            window.setTimeout(this._onClientDblClick, 0);
        e.preventDefault();
    },
    get_onClientClick: function () {
        return this._onClientClick;
    },

    set_onClientClick: function (value) {
        if (this._onClientClick !== value) {
            this._onClientClick = value;
        }
    },
    get_onClientDblClick: function () {
        return this._onClientDblClick;
    },

    set_onClientDblClick: function (value) {
        if (this._onClientDblClick !== value) {
            this._onClientDblClick = value;
        }
    },
    get_width: function () {
        return this._width;
    },

    set_width: function (value) {
        if (this._width !== value) {
            this._width = value;
            $(this.get_element()).attr("width", this._width);
        }
    },
    get_text: function () {
        return this._text;
    },

    set_text: function (value) {
        if (this._text !== value) {
            this._text = value;
            $(this.get_element()).val(value);
        }
    },
    get_cssClass: function () {
        return this._cssClass;
    },

    set_cssClass: function (value) {
        if (this._cssClass !== value) {
            this._cssClass = value;
            $(this.get_element()).attr("class", value);
        }
    }
}

Lib.button.descriptor = {
    properties: [{ name: 'onClientClick', type: String },
                 { name: 'onClientDblClick', type: String },
                 { name: 'width', type: String },
                 { name: 'text', type: String },
                 { name: 'cssClass', type: String }]
}

Lib.button.registerClass('Lib.button', Sys.UI.Control);


if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();