///<reference path="Common.js"/>
///<reference path="Slick/slick.grid.js"/>
/// <reference name="MicrosoftAjax.js"/>
/// <reference path="jquery-1.11.0.js" />

Type.registerNamespace('Lib');

Lib.dropdowGrid = function (element) {
    Lib.dropdowGrid.initializeBase(this, [element]);

    this._Name = null;
    this._lke = null;
    this._columns = null;
    this._dataSource = null;
    this._cssClass = null;
    this._multiCheck = null;
    this._pageSize = null;
    this._dataValue = null;
    this._onClientSelectItem = null;

}

Lib.dropdowGrid.prototype = {
    initialize: function () {
        Lib.dropdowGrid.callBaseMethod(this, 'initialize');

        var columns = Common.stringToJson(this.get_columns());
        var pageSize = (this._pageSize == null ? 30 : parseFloat(this._pageSize));
        var gridData = GetJsonData(columns, this.get_dataSource(), pageSize);
        var a = this.get_element();
        var grid;
        var dataValue = this._dataValue;
        var displayValue = this._displayValue;
        var onClientUpdateCell = this._onClientSelectItem;

        var dropID = this.get_element().id;
        var gridID = dropID + "_grid";
        var arrowID = dropID + "_arrow";
        var inputID = dropID + "_txt";
        var getVal = "", getDis = "";
        $(function () {
            //  debugger;
            var options = {
                editable: true,
                hidenHeader: false,
                //enableAddRow: true,
                enableColumnReorder: false,
                enableCellNavigation: true,
                asyncEditorLoading: true,
                forceFitColumns: false,
                topPanelHeight: 25
            };
           
            var data = [];
            data = gridData;
            Common.parseData(data, columns, pageSize);

            for (var i = 0; i < columns.length; i++) {
                columns[i].width = parseInt(columns[i].width);
                if (columns[i].hasOwnProperty("hidden"))
                    columns[i].hidden = Boolean.parse(columns[i].hidden);
            }
            // Khi chọn anh thi show

            grid = new Slick.Grid("#" + gridID, data, columns, options);
            grid.setSelectionModel(new Slick.RowSelectionModel());

            $("#" + gridID).hide();
            $("#" + gridID).css("z-index", 30000);

            $("#" + arrowID).click(function (e) {
                if ($(e.target).closest("#" + arrowID).length == 0) {
                    $("#" + gridID).hide(400);
                    grid.scrollRowToTop(0);
                }
                else {
                    grid.setData(data, false);
                    grid.render();
                    $("#" + gridID).show(400);
                    grid.scrollRowToTop(0);
                }
                e.preventDefault();
                return false;
            });
            $("#" + inputID).click(function (e) {
                $("#" + gridID).show(400);
                e.preventDefault();
                return false;
            });
            $("#" + inputID).keypress(function (e) {
                var code = Common.getKeyCode(e, "keypress");
                var char = Common.formCodeToChar(code);
                var position = Common.getCaret(this);
                var selectStart = this.selectionStart;
                var selectEnd = this.selectionEnd;
                if (code != 0 && selectStart < selectEnd) {
                    var newVal = $(this).val().substr(0, selectStart) + $(this).val().substr(selectEnd);
                    $(this).val(newVal);
                }

                var newVal = $(this).val().substring(0, position) +
                    char + $(this).val().substring(position);
                Common.setlectRang(this, position + 1, position + 1);
                $(this).val(newVal);


                var newData =
                jQuery.grep(data, function (element, index) {
                    var val = element[displayValue].toString().toUpperCase();
                    if (val == null)
                        return true;
                    if (newVal == "" || newVal == undefined)
                        return true;
                    return val.indexOf(newVal.toUpperCase()) >= 0;
                });
                grid.setData(newData, false);
                grid.render();

                e.preventDefault();
                return false;
            });
            $("#" + inputID).keydown(function (e) {
                var control = this;
                var position = Common.getCaret(control);
                var keyCode = Common.getKeyCode(e, "keydown");
                if (keyCode == 46) keyCode = 127;
                switch (keyCode) {
                    case Sys.UI.Key.backspace:
                        {
                            if (position <= 0) { return true; }
                            var newVal = $(control).val().substring(0, position - 1) + $(control).val().substring(position);
                            $(control).val(newVal);
                            Common.setlectRang(control, position - 1, position - 1);
                            var newData =
                           jQuery.grep(data, function (element, index) {
                               var val = element[displayValue].toString().toUpperCase();
                               if (val == null)
                                   return true;
                               if (newVal == "" || newVal == undefined)
                                   return true;
                               return val.indexOf(newVal.toUpperCase()) >= 0;
                           });
                            grid.setData(newData, false);
                            grid.render();
                            e.preventDefault();
                            break;
                        }
                    case Sys.UI.Key.del:
                        {
                            var newVal = $(control).val().substring(0, position) + $(control).val().substring(position + 1);
                            $(control).val(newVal);
                            Common.setlectRang(control, position + 1, position + 1);
                            var newData =
                           jQuery.grep(data, function (element, index) {
                               var val = element[displayValue].toString().toUpperCase();
                               if (val == null)
                                   return true;
                               if (newVal == "" || newVal == undefined)
                                   return true;
                               return val.indexOf(newVal.toUpperCase()) >= 0;
                           });
                            grid.setData(newData, false);
                            grid.render();
                            e.preventDefault();
                            break;
                        }
                    case Sys.UI.Key.enter:
                        {
                            var newVal = $(control).val();
                            var newData =
                          jQuery.grep(data, function (element, index) {
                              var val = element[displayValue].toString().toUpperCase();
                              if (val == null)
                                  return true;
                              if (newVal == "" || newVal == undefined)
                                  return true;
                              return val.indexOf(newVal.toUpperCase()) >= 0;
                          });
                            grid.setData(newData, false);
                            grid.render();
                            e.preventDefault();
                            break;
                        }
                }
            });
            $('body').delay(100).bind('click', function (e) {
                if ($(e.target).closest("#" + arrowID).length == 0) {
                    $("#" + gridID).hide(400);
                    grid.scrollRowToTop(0);
                    $("#" + arrowID).attr("selector", false);
                }
                e.preventDefault();
                return false;
            });

            //Chọn 1 Itemas
            grid.onActiveCellChanged.subscribe(function (item, arg) {
                var value = "";
                var row = arg.row;
                if (row == undefined)
                    return null;
                var a_data = arg.grid.getDataItem(row);
                getVal = a_data[dataValue], getDis = a_data[displayValue];

                if (!Common.isNullOrEmty(onClientUpdateCell)) {
                    var cell = arg.cell;
                    if (cell == undefined)
                        return null;
                    window[onClientUpdateCell](row, cell, getVal);
                }
                $("#" + inputID).val(getDis);
                return false;
            });

        });
    },
    dispose: function () {
        $clearHandlers(this.get_element());
        Lib.dropdowGrid.callBaseMethod(this, 'dispose');
    },
    get_columns: function () {
        return this._columns;
    },

    set_columns: function (value) {
        if (this._columns !== value) {
            this._columns = value;
        }
    },
    getValue: function()
    {
       
    },
    get_Name: function () {
        return this._Name;
    },
    set_Name: function (value) {
        if (this._Name !== value) {
            this._Name = value;
        }
    },
    set_dataSource: function (value) {
        if (this._dataSource !== value) {
            this._dataSource = value;
        }
    },
    get_dataSource: function () {
        return this._dataSource;
    },

    get_multiCheck: function () {
        return this._multiCheck;
    },
    set_multiCheck: function (value) {
        if (this._multiCheck !== value) {
            this._multiCheck = value;
        }
    },
    get_pageSize: function () {
        return this._pageSize;
    },
    set_pageSize: function (value) {
        if (this._pageSize !== value) {
            this._pageSize = value;
        }
    },
    get_dataValue: function () {
        return this._dataValue;
    },
    set_dataValue: function (value) {
        if (this._dataValue !== value) {
            this._dataValue = value;
        }
    },
    get_displayValue: function () {
        return this._displayValue;
    },
    set_displayValue: function (value) {
        if (this._displayValue !== value) {
            this._displayValue = value;
        }
    },
    get_onClientSelectItem: function () {
        return this._onClientSelectItem;
    },
    set_onClientSelectItem: function (value) {
        if (this._onClientSelectItem !== value) {
            this._onClientSelectItem = value;
        }
    }
}
Lib.dropdowGrid.descriptor = {
    properties: [
                 { name: 'Name', type: String },
                 { name: 'columns', type: String },
                 { name: 'dataSource', type: String },
                 { name: 'multiCheck', type: String },
                 { name: 'pageSize', type: String },
                 { name: 'dataValue', type: String },
                 { name: 'displayValue', type: String },
                 { name: 'onClientSelectItem', type: String }]
}

Lib.dropdowGrid.registerClass('Lib.dropdowGrid', Sys.UI.Control);


if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();