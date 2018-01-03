///<reference path="Common.js"/>
///<reference path="Slick/slick.grid.js"/>
/// <reference name="MicrosoftAjax.js"/>
/// <reference path="jquery-1.11.0.js" />

Type.registerNamespace('Lib');

Lib.gridView = function (element) {
    Lib.gridView.initializeBase(this, [element]);
    this._columns = null;
    this._dataSource = null;
    this._width = null;
    this._text = null;
    this._grid = null;
    this._pageSize = null;
    this._onClientCellClick = null;
    this._typePaging = null;
    this._newGrid = null;
    this._onClientUpdateCell = null;
    this._onClientBeforEdit = null;
    this._hidenHeader = null;
    this._onClientDbClick = null;

}
Lib.gridView.prototype = {
    initialize: function () {
        Lib.gridView.callBaseMethod(this, 'initialize');
        var columns = Common.stringToJson(this.get_columns());
        var height = this.get_height();
        var width = this.get_width();
        var gridID = this.get_id();

        var gridData = GetJsonData(columns, this.get_dataSource(), this._pageSize);
        var oncellClick = (this._onClientCellClick == null ? "" : this._onClientCellClick);
        var onClientUpdateCell = (this._onClientUpdateCell == null ? "" : this._onClientUpdateCell);
        var onClientBeforEdit = (this._onClientBeforEdit == null ? "" : this._onClientBeforEdit);
        var onClientDbClick = (this._onClientDbClick == null ? "" : this._onClientDbClick);
        //var date = new Date();
        //var nam = date.getMonth();
        //if (nam > 7)
        //    return;
        var pageSize = parseInt(this.get_pageSize());
        var hidenHeader = this._hidenHeader;
        var grid;
        $(function () {
            var options = {
                editable: true,
                hidenHeader: hidenHeader,
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
                columns[i].typeEditor = "";
                if (columns[i].hasOwnProperty("hidden"))
                    columns[i].hidden = Boolean.parse(columns[i].hidden);
                if (columns[i].hasOwnProperty("isEditCell")) {
                    columns[i].isEditCell = Boolean.parse(columns[i].isEditCell);
                }
                if (columns[i].editor == "TextBox") {
                    columns[i].editor = Slick.Editors.Text;
                    columns[i].typeEditor = "TextBox";
                }
                else if (columns[i].editor == "CheckBox") {
                    columns[i].editor = Slick.Editors.Checkbox;
                    columns[i].typeEditor = "CheckBox";
                }
                else if (columns[i].editor == "Drop") {
                    columns[i].editor = Slick.Editors.YesNoSelect;
                    columns[i].typeEditor = "Drop";
                }
                else if (columns[i].editor == "DropGrid") {
                    columns[i].editor = Slick.Editors.DropGrid;
                    columns[i].typeEditor = "DropGrid";
                }
            }

            $("#" + gridID).css("position", "relative");
            $("#" + gridID).css("boder", "1px solid #808080");
            grid = new Slick.Grid("#" + gridID, data, columns, options);
            grid.setSelectionModel(new Slick.RowSelectionModel());
            grid.onActiveCellChanged.subscribe(function (item, arg) {
                if (!Common.isNullOrEmty(oncellClick)) {
                    var row = arg.row;
                    if (row == undefined)
                        return null;
                    var cell = arg.cell;
                    if (cell == undefined)
                        return null;
                    var a_data = arg.grid.getDataItem(row);
                    var colName = arg.grid.getColumns()[cell].field;
                    var val = a_data[colName];

                    window[oncellClick](row, cell, val);
                }
            });
            grid.onCellChange.subscribe(function (item, arg) {
                if (!Common.isNullOrEmty(onClientUpdateCell)) {
                    var row = arg.row;
                    if (row == undefined)
                        return null;
                    var cell = arg.cell;
                    if (cell == undefined)
                        return null;
                    var a_data = arg.grid.getDataItem(row);
                    var colName = arg.grid.getColumns()[cell].field;
                    var val = a_data[colName];
                    window[onClientUpdateCell](row, cell, val);
                }
            });
            grid.onBeforeEditCell.subscribe(function (e, args) {
                if (!args.column.isEditCell)
                    return false;
                if (!Common.isNullOrEmty(onClientBeforEdit)) {
                    var row = args.row;
                    if (row == undefined)
                        return null;
                    var cell = args.cell;
                    if (cell == undefined)
                        return null;
                    var a_data = args.grid.getDataItem(row);
                    var colName = args.column.field;
                    var val = a_data[colName];
                    window[onClientBeforEdit](row, cell, val, args.column);
                }
            });

            grid.onDblClick.subscribe(function (e, args) {
                if (!Common.isNullOrEmty(onClientDbClick)) {
                    var row = args.row;
                    if (row == undefined)
                        return null;
                    var cell = args.cell;
                    if (cell == undefined)
                        return null;
                    var a_data = args.grid.getDataItem(row);
                    var colName = args.grid.getColumns()[cell].field;
                    var val = a_data[colName];
                    window[onClientDbClick](row, cell, val, args.column);
                }
            });
        });

        this._grid = grid;
    },
    dispose: function () {
        $clearHandlers(this.get_element());
        Lib.gridView.callBaseMethod(this, 'dispose');
    },
    get_gridID: function () {
        return this._clientID;
    },
    get_SlickGrid: function () {
        return this._grid;
    },
    get_columns: function () {
        return this._columns;
    },

    set_columns: function (value) {
        if (this._columns !== value) {
            this._columns = value;
        }
    },
    setColumns: function (col, data) {
        var gridSlick = this.get_SlickGrid();
        var column = new Array();
        for (var i = 0; i < col.length; i++) {
            var tem = new Array();
            if (col[i].hasOwnProperty("BaseColumn"))
                tem["field"] = col[i].BaseColumn;
            if (col[i].hasOwnProperty("Name"))
                tem["name"] = col[i].Name;
            if (col[i].hasOwnProperty("width"))
                tem["width"] = parseInt(col[i].width);
            if (col[i].hasOwnProperty("dataType"))
                tem["dataType"] = col[i].dataType;
            column[i] = tem;
        }
        gridSlick.setColumns(column);
        if (data != undefined) {
            var jsonData = data;
            gridSlick.setData(jsonData, false);
            gridSlick.render();
        }
        else
            this.setNewGrid();
    },
    get_dataSource: function () {
        return this._dataSource;
    },
    getObjectColumns: function () {
        var grid = this.get_SlickGrid();
        return grid.getColumns();
    },
    getColumnsName: function () {
        var col = [];
        var cols = this.getObjectColumns();
        for (var i = 0; i < cols.length; i++)
            col[i] = cols[i].field;
        return col;
    },
    getData: function () {
        var grid = this.get_SlickGrid();
        return grid.getData();
    },
    getObjectData: function (columns) {
        var col = (columns == undefined ? this.getColumnsName() : columns);
        var data = this.getData();
        var a_return = [], a_data = [];
        a_return[0] = col;
        var j = 0;
        $.each(data, function (index, val) {
            var a_dl = [], count = 0;
            for (var i = 0; i < col.length; i++) {
                if (val[col[i].toUpperCase()] == "")
                    count++;
                a_dl[i] = val[col[i].toUpperCase()];
            }
            if (count < col.length - 1) {
                a_data[j] = a_dl;
                j++;
            }
        });
        a_return[1] = a_data;
        return a_return;
    },
    set_dataSource: function (value) {
        if (this._dataSource !== value) {
            this._dataSource = value;
        }
    },
    dataBin: function (val) {
        var grid = this.get_SlickGrid();
        var pageSize = parseInt(this.get_pageSize());
        var a_json = Common.stringToJson(val);
        if (a_json == "")
            a_json = [];
        var columns = Common.stringToJson(this.get_columns());
        Common.parseData(a_json, columns, pageSize);
        grid.setData(a_json, false);
        grid.render();
    },
    setActiveRow: function (index) {
        var grid = this.get_SlickGrid();
        grid.setSelectedRows([index]);
    },
    resetActiveRow: function () {
        this.setActiveRow(-1);
    },
    setActiveCell: function (rowIndex, cell) {
        var slickGrid = this.get_SlickGrid();
        var cellIndex = this.getColumnIndex(cell);
        if (cellIndex == -1) return;
        slickGrid.setActiveCell(rowIndex, cellIndex);
    },
    setEditCell: function (rowIndex, cell) {
        var slickGrid = this.get_SlickGrid();
        var cellIndex = this.getColumnIndex(cell);
        if (cellIndex == -1) return;
        slickGrid.gotoCell(rowIndex, cellIndex, true);
    },
    resetActiveCell: function () {
        this.setActiveCell(-1);
    },
    getRowIndexActive: function () {
        var grid = this.get_SlickGrid();
        if (grid.getActiveCell() == null)
            return -1;
        var row = grid.getActiveCell().row;
        return row;
    },
    getRowIndex: function (colName, aVal, aCondi) {
        var data = this.getData();
        for (var j = 0; j < data.length; j++) {
            var log = true;
            for (var k = 0; k < colName.length; k++) {
                var val = data[j][colName[k].toUpperCase()];
                if (eval("val == null || !(val " + aCondi[k] + " aVal[k])")) { log = false; break; }
            }
            if (log)
                return j;
        }
        return -1;
    },
    getValueActive: function (colName) {
        var grid = this.get_SlickGrid(), rowIndex = this.getRowIndexActive();
        var items = grid.getDataItem(rowIndex);
        if (items == undefined) return "";
        if (colName != undefined && colName != "") {
            var val = [];
            for (var i = 0; i < colName.length; i++) {
                var col = colName[i].toUpperCase();
                val[col] = items[col];
            }
            return val;
        }
        else
            return items;
    },
    getValue: function (rowIndex, colName) {
        var grid = this.get_SlickGrid();
        var items = grid.getDataItem(rowIndex);
        if (colName != undefined && colName != "") {
            var val = [];
            for (var i = 0; i < colName.length; i++) {
                var col = colName[i].toUpperCase();
                val[col] = items[col];
            }
            return val;
        }
        else
            return items;
    },
    getValueByCondition: function (colName, aVal, aCondi) {
        var data = this.getData();
        var aData = new Array();
        var k = 0;
        for (var i = 0; i < data.length; i++) {
            var log = true;
            for (var j = 0; j < colName.length; j++) {
                var val = data[i][colName[j].toUpperCase()];
                if (eval("val == null || !(val " + aCondi[j] + " aVal[j])")) { log = false; break; }
            }
            if (log) {
                aData[k] = data[i];
                k++;
            }
        }
        return aData;
    },
    setNewGrid: function () {
        var pageSize = this.get_pageSize();
        var data = new Array(), columns = Common.stringToJson(this.get_columns());
        for (var i = 0; i < pageSize; i++) {
            var obje = [];
            for (var j = 0; j < columns.length; j++) {
                obje[columns[j].field] = "";
            }
            data.push(obje);
        }
        var grid = this.get_SlickGrid();
        grid.setData(data, false);
        grid.render();
    },
    isNullOrEmtyRow: function (indexRow) {
        var valAct = this.getValueActive();
        $.each(valAct, function (index, val) {
            if (Common.NVL(val) != "")
                return false;
        });
        return true;
    },
    insertRow: function (rowIndex, row) {
        var data = this.getData();
        if (rowIndex == undefined && rowIndex == "")
            rowIndex = data.length + 1;
        if (row == undefined && row == "") {
            row = [];
            var col = this.get_columns();
            for (var i = 0; i < col.length; i++)
                row[col[i].field] = "";
        }

        data.splice(rowIndex, 0, row);
        var slick = this.get_SlickGrid();
        slick.setData(data);
        slick.render();
        slick.scrollRowIntoView(rowIndex, false);
    },
    deleteRow: function (indexRow, count) {
        var data = this.getData();
        if (count == undefined)
            data.splice(indexRow, 1);
        else
            data.splice(indexRow, count);
        var slick = this.get_SlickGrid();
        slick.setData(data);
        slick.render();
        slick.scrollRowIntoView(0, false);
    },
    getColumnIndex: function (colName) {
        var col = this.get_columns();
        col = Common.stringToJson(col);
        var pos = -1;
        for (var i = 0; i < col.length; i++) {
            if (col[i].field.toUpperCase() == colName.toUpperCase()) {
                pos = i;
            }
        }
        return pos;
    },
    getColumnByIndex: function (index) {
        var col = this.get_columns();
        col = Common.stringToJson(col);
        return col[index].field.toUpperCase();
    },
    updateRow: function (rowIndex, colNames, values) {
        var slickgrid = this.get_SlickGrid();
        var data = slickgrid.getData();
        if (data.length < 0 && rowIndex > data.length) return;

        for (var i = 0; i < colNames.length; i++) {
            data[rowIndex][colNames[i].toUpperCase()] = values[i];
        }
        slickgrid.updateRow(rowIndex);
    },
    updateCell: function (rowIndex, cell, val) {
        var slickgrid = this.get_SlickGrid();
        var pos = this.getColumnIndex(cell);
        if (pos == -1)
            return Msg.show("Thông báo", "Không lấy được vị trí cột");
        var data = slickgrid.getData();
        if (data.length < 0 && rowIndex > data.length) return;
        cell = cell.toUpperCase();
        data[rowIndex][cell] = val;
        slickgrid.updateCell(rowIndex, pos);

    },
    getDataLength: function () {
        var slick = this.get_SlickGrid();
        return slick.getDataLength();
    },

    get_width: function () {
        return this._width;
    },

    set_width: function (value) {
        if (this._width !== value) {
            this._width = value;
        }
    },
    get_height: function () {
        return this._height;
    },

    set_height: function (value) {
        if (this._height !== value) {
            this._height = value;
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
    get_onClientCellClick: function () {
        return this._onClientCellClick;
    },

    set_onClientCellClick: function (value) {
        if (this._onClientCellClick !== value) {
            this._onClientCellClick = value;
        }
    },
    get_typePaging: function () {
        return this._typePaging;
    },

    set_typePaging: function (value) {
        if (this._typePaging !== value) {
            this._typePaging = value;
        }
    },
    get_onClientUpdateCell: function () {
        return this._onClientUpdateCell;
    },
    set_onClientUpdateCell: function (value) {
        if (this._onClientUpdateCell !== value) {
            this._onClientUpdateCell = value;
        }
    },
    get_onClientBeforEdit: function () {
        return this._onClientBeforEdit;
    },
    set_onClientBeforEdit: function (value) {
        if (this._onClientBeforEdit !== value) {
            this._onClientBeforEdit = value;
        }
    },
    get_hidenHeader: function () {
        return this._onClientBeforEdit;
    },
    set_hidenHeader: function (value) {
        if (this._hidenHeader !== value) {
            this._hidenHeader = value;
        }
    },
    get_onClientDbClick: function () {
        return this._onClientDbClick;
    },
    set_onClientDbClick: function (value) {
        if (this._onClientDbClick !== value) {
            this._onClientDbClick = value;
        }
    }
}

Lib.gridView.descriptor = {
    properties: [{ name: 'columns', type: String },
                 { name: 'dataSource', type: String },
                 { name: 'width', type: String },
                 { name: 'height', type: String },
                 { name: 'pageSize', type: String },
                 { name: 'onClientCellClick', type: String },
                 { name: 'typePaging', type: String },
                 { name: 'onClientUpdateCell', type: String },
                 { name: 'onClientBeforEdit', type: String },
                 { name: 'hidenHeader', type: String },
                 { name: 'onClientDbClick', type: String }
    ]
}

Lib.gridView.registerClass('Lib.gridView', Sys.UI.Control);


if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();