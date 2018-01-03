/// <reference path="Common.js" />
/// <reference path="jquery-1.11.0.js" />

Type.registerNamespace('Lib');

Lib.dao = function (element) {
    Lib.dao.initializeBase(this, [element]);

    this._onClientClick = null;
    this._onClientDblClick = null;
    this._Name = null;
    this._lke = null;
    this._cssClass = null;
}

Lib.dao.prototype = {
    initialize: function () {
        Lib.dao.callBaseMethod(this, 'initialize');
        $addHandlers(this.get_element(),
                          {
                              'click': this._onClick
                          },
                          this);
        $(this._element).css("cursor", "pointer");
        $(this._element).css("text-align", "center");
    },
    dispose: function () {
        $clearHandlers(this.get_element());
        Lib.dao.callBaseMethod(this, 'dispose');
    },
    _onClick: function (e) {
        var control = this._element;
        $(control).text(Common.transformValue($(control).text(), this._lke));
        Common.setlectRang(control, 0, 0);
        e.preventDefault();
        return false;
    },
    get_Name: function () {
        return this._Name;
    },

    set_Name: function (value) {
        if (this._Name !== value) {
            this._Name = value;
        }
    },
    get_lke: function () {
        return this._lke;
    },

    set_lke: function (value) {
        if (this._lke !== value) {
            this._lke = value;
        }
    }
}

Lib.dao.descriptor = {
    properties: [{ name: 'onClientClick', type: String },
                 { name: 'Name', type: String },
                 { name: 'lke', type: String }]
}

Lib.dao.registerClass('Lib.dao', Sys.UI.Control);


if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();