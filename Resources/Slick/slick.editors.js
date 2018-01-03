/// <reference name="../../Common.js"/>
/// <reference name="../MicrosoftAjax.js"/>
/// <reference name="../jquery-1.11.0.js"/>

(function ($) {
    // register namespace
    $.extend(true, window, {
        "Slick": {
            "Editors": {
                "Text": TextEditor,
                "Integer": IntegerEditor,
                "Date": DateEditor,
                "YesNoSelect": YesNoSelectEditor,
                "Checkbox": CheckboxEditor,
                "PercentComplete": PercentCompleteEditor,
                "LongText": LongTextEditor,
                "DropGrid": DropGrid
            }
        }
    });

    function TextEditor(args) {
        var $input;
        var defaultValue;
        var scope = this;
        var typeData = args.column.dataType;
        var gridID = args.grid.getGridID().replace("#", "");
        this.init = function () {
            $input = $("<INPUT  type=text class='editor-text ' id='" + gridID + "_" + args.column.field + "'/>")
                .appendTo(args.container)
                .bind("keydown.nav", function (e) {
                    if (e.keyCode === Sys.UI.Key.left || e.keyCode === Sys.UI.Key.right) {
                        e.stopImmediatePropagation();
                    }
                })
                .click(function (e) {
                    $(this).attr("isChange", true);
                    var control = this;
                    if (args.column.lke != "") {
                        var newVal = Common.transformValue($(control).val(), args.column.lke);
                        $(control).val(newVal);
                    }
                    e.preventDefault();
                    return false;
                })
                .keypress(function (e) {
                    var keyCode = Common.getKeyCode(e);
                    if (keyCode <= 0) {
                        return;
                    }

                    if ($(this).val().length > parseInt(args.column.maxLength) && parseInt(args.column.maxLength) > 0) {
                        e.preventDefault();
                        return false;
                    }
                    var position = Common.getCaret(this);
                    $(this).attr("isChange", true);
                    if (typeData == "Number") {
                        if (e.keyCode == Sys.UI.Key.del || e.keyCode == Sys.UI.Key.left || e.keyCode == Sys.UI.Key.backspace)
                            var a = 0;
                        else {
                            var old = $(this).val();
                            var newchar = Common.formCodeToChar(keyCode);
                            var dec = parseInt(args.column.dec);
                            if (dec > 0) {
                                var check = "0123456789.";
                                if (check.indexOf(newchar) < 0 || (newchar == "0" && old == "0")
                                    || (newchar == "." && old.indexOf(".") >= 0))
                                    return false;
                                old = old.substr(0, position) + newchar + old.substr(position);
                                var startIndex = old.indexOf(".");
                                if (startIndex >= 0 && old.length - startIndex - 1 > dec) {
                                    return false;
                                    e.preventDefault();
                                }
                                $(this).val(old);
                                e.preventDefault();
                                e.stopImmediatePropagation();
                                return false;
                            }
                            else
                                if (!Common.isKeyDigit(e.keyCode)) { e.preventDefault(); return false; }
                        }
                    }
                    else if (typeData == "Money") {
                        var newchar = Common.formCodeToChar(keyCode);
                        var old = $(this).val();
                        var dec = parseInt(args.column.dec);
                        var check = "-0123456789";
                        if (dec > 0)
                            check = check + ".";
                        if (check.indexOf(newchar) < 0 || (newchar == "0" && old == "0")
                                     || (newchar == "." && old.indexOf(".") >= 0)) {
                            return false;
                        }
                        old = old.substr(0, position) + newchar + old.substr(position);
                        newchar = Common.stringToFormatNumber(old, dec);
                        $(this).val(newchar);
                        e.preventDefault();
                        e.stopImmediatePropagation();
                        return false;
                    }
                    else if (typeData == "String") {
                        if (args.column.lke != "") {
                            var newchar = Common.formCodeToChar(keyCode).toUpperCase();
                            if (args.column.lke.indexOf(newchar) >= 0) {
                                $(this).val(newchar);
                            }
                            e.preventDefault();
                            e.stopImmediatePropagation();
                            return false;
                        }
                        if (Common.toBool(args.column.upperCase) == true) {

                            var newVal = $(this).val().substring(0, position) +
                                Common.formCodeToChar(keyCode).toUpperCase() + $(this).val().toString().substring(position);
                            $(this).val(newVal);
                            Common.setlectRang(this, position + 1, position + 1);
                            e.preventDefault();
                            e.stopImmediatePropagation();
                            return false;
                        }
                    }
                    else if (typeData == "Date" || typeData == "Month") {
                        if (typeData.toString() == "Date" && position >= 10) { e.preventDefault(); return true; }
                        if (typeData.toString() == "Month" && position >= 7) { e.preventDefault(); return true; }

                        if (keyCode != 46)
                            if (!Common.isKeyDigit(keyCode)) { e.preventDefault(); return true; }
                        var charLost = "";
                        if (position == 2 || (position == 5 && typeData == "Date")) {
                            if ($(this).val().toString().substring(position, position + 1) == "/")
                                charLost = "";
                            else
                                charLost = "/";
                            position++;
                        }
                        var newVal = $(this).val().toString().substring(0, position) + charLost +
                   Common.formCodeToChar(keyCode) + $(this).val().toString().substring(position + 1);
                        $(this).val(newVal);
                        Common.setlectRang(this, position + 1, position + 1);
                        e.preventDefault();
                        return false;
                    }
                })
                .keydown(function (e) {
                    var keyCode = Common.getKeyCode(e);
                    var dec = parseInt(args.column.dec);
                    if (keyCode == 46) keyCode = 127;
                    switch (keyCode) {

                        case 112:
                            var url = args.column.url;
                            document.onhelp = new Function("return false;");
                            if (navigator.userAgent.search(/msie/i) == -1) { e.stopPropagation(); e.preventDefault(); }
                            if (Common.isNullOrEmty(url))
                                return false;
                            if ("aspx".indexOf(url) >= 0) {
                                var val = $(this).val();
                                OpenWindow(url, window.name + "," + $(this).attr("id"), [$(this).val()]);
                            }
                            else {
                                window[url](this);
                            }

                            return false;
                            break;
                        case Sys.UI.Key.enter:
                            if ($(this).attr("isChange")) {
                                var updateCell = $("#" + gridID).attr("onClientUpdateCell");
                                if (Common.isNullOrEmty(updateCell))
                                    return;
                                var grid = GetGridByID(gridID);
                                var indexRow = grid.getRowIndexActive();
                                var val = $(this).val();

                                grid.updateCell(indexRow, args.column.field.toUpperCase(), val);
                                $(this).attr("isChange", false);
                                if (updateCell == "")
                                    return;
                                window[updateCell](gridID, this, indexRow, args.column.field, $(this).val());
                                e.preventDefault();
                            }
                            break;
                        case Sys.UI.Key.del:
                            var position = Common.getCaret(this);
                            var char = Common.formCodeToChar(keyCode);
                            if (typeData == "Money") {
                                if (char == "," && position < $(control).val().length)
                                    position = position + 1;
                                var newchar = $(this).val().substring(0, position) + $(this).val().substring(position + 1);
                                var newcharNumber = Common.stringToNumber(newchar).toString();
                                var format = Common.stringToFormatNumber(newcharNumber, dec);
                                $(this).val(format);
                                Common.setlectRang(this, position, position);
                                e.preventDefault();
                                return true;
                            }
                            else if (typeData == "Date" || typeData == "Month") {
                                if (typeData.toString() == "Date" && position >= 10) { e.preventDefault(); return true; }
                                if (typeData.toString() == "Month" && position >= 7) { e.preventDefault(); return true; }

                                if (position == 2 || (position == 5 && typeData.toString() == "Date")) {
                                    position = position + 1;
                                }
                                var val = $(this).val().substring(0, position) + "_" + $(this).val().substring(position + 1);
                                var s = $(this).val().replace(/\//g, "");
                                var valInput = s.substring(0, 1) + "/" + s.substring(2, 4) + "/" + s.substring(4);
                                $(this).val(val);
                                Common.setlectRang(this, position + 1, position + 1);
                                e.preventDefault();
                                break;
                            }
                            break;
                        case Sys.UI.Key.backspace:
                            var position = Common.getCaret(this);
                            if (position <= 0)
                                return;
                            var char = Common.formCodeToChar(keyCode);
                            if (typeData == "Money") {
                                if (char == "," && position < $(control).val().length)
                                    position = position - 1;
                                var newchar = $(this).val().substring(0, position - 1) + $(this).val().substring(position);
                                var newcharNumber = Common.stringToNumber(newchar).toString();
                                var format = Common.stringToFormatNumber(newcharNumber, dec);
                                $(this).val(format);
                                Common.setlectRang(this, position, position);
                                e.preventDefault();
                                return true;
                            }
                            else if (typeData == "Date" || typeData == "Month") {
                                if (position == 3 || (position == 6 && typeData.toString() == "Date"))
                                    position = position - 1;
                                var val = $(this).val().substring(0, position - 1) + "_" + $(this).val().substring(position);
                                $(this).val(val);
                                Common.setlectRang(this, position - 1, position - 1);
                                e.preventDefault();
                                break;
                            }
                            break;
                    }
                })
                .focus(function (e) {
                    var val = $(this).val();

                    if (typeData == "Month" && val=="") {
                        $(this).val("__/____");
                    }
                    else if (typeData == "Date" && val == "") {
                        $(this).val("__/__/____");
                    }
                    Common.setlectRang(this, 0, 0);
                    e.preventDefault();
                    return false;
                })
                .select();

            if (typeData == "Number" || typeData == "Money") {
                $input.css("text-align", "right");
            }
            if (args.column.lke != "" || typeData == "Date" || typeData == "Month") {
                $input.css("text-align", "center");
                $input.css("cursor", "pointer");
            }
        };

        this.destroy = function () {
            $input.remove();
        };

        this.focus = function () {
            $input.focus();
        };

        this.getValue = function () {
            return $input.val();
        };

        this.setValue = function (val) {

            $input.val(val);
        };

        this.loadValue = function (item) {
            defaultValue = item[args.column.field] || "";
            if (typeData == "Money") {
                if ($.isNumeric(defaultValue)) {
                    var dec = parseInt(args.column.dec);
                    var tg = parseFloat(defaultValue).toString();
                    defaultValue = Common.stringToFormatNumber(tg, dec);
                }
            }
           
            $input.val(defaultValue);
            $input[0].defaultValue = defaultValue;
            $input.select();
            $input.focus();
        };

        this.serializeValue = function () {
            return $input.val();
        };

        this.applyValue = function (item, state) {
            var defaultValue = state == "__/__/____" ? "" : state;
            item[args.column.field] = defaultValue;
        };

        this.isValueChanged = function () {
            return (!($input.val() == "" && defaultValue == null)) && ($input.val() != defaultValue);
        };

        this.validate = function () {
            if (args.column.validator) {
                var validationResults = args.column.validator($input.val());
                if (!validationResults.valid) {
                    return validationResults;
                }
            }

            return {
                valid: true,
                msg: null
            };
        };

        this.init();
    }

    function IntegerEditor(args) {
        var $input;
        var defaultValue;
        var scope = this;

        this.init = function () {
            $input = $("<INPUT type=text class='editor-text' />");

            $input.bind("keydown.nav", function (e) {
                if (e.keyCode === $.ui.keyCode.LEFT || e.keyCode === $.ui.keyCode.RIGHT) {
                    e.stopImmediatePropagation();
                }
            });

            $input.appendTo(args.container);
            $input.focus().select();
        };

        this.destroy = function () {
            $input.remove();
        };

        this.focus = function () {
            $input.focus();
        };

        this.loadValue = function (item) {
            defaultValue = item[args.column.field];
            $input.val(defaultValue);
            $input[0].defaultValue = defaultValue;
            $input.select();
        };

        this.serializeValue = function () {
            return parseInt($input.val(), 10) || 0;
        };

        this.applyValue = function (item, state) {
            item[args.column.field] = state;
        };

        this.isValueChanged = function () {
            return (!($input.val() == "" && defaultValue == null)) && ($input.val() != defaultValue);
        };

        this.validate = function () {
            if (isNaN($input.val())) {
                return {
                    valid: false,
                    msg: "Please enter a valid integer"
                };
            }

            return {
                valid: true,
                msg: null
            };
        };

        this.init();
    }

    function DateEditor(args) {
        var $input;
        var defaultValue;
        var scope = this;
        var calendarOpen = false;

        this.init = function () {
            $input = $("<INPUT type=text class='editor-text' />");
            $input.appendTo(args.container);
            $input.focus().select();
            $input.datepicker({
                showOn: "button",
                buttonImageOnly: true,
                buttonImage: "../images/calendar.gif",
                beforeShow: function () {
                    calendarOpen = true
                },
                onClose: function () {
                    calendarOpen = false
                }
            });
            $input.width($input.width() - 18);
        };

        this.destroy = function () {
            $.datepicker.dpDiv.stop(true, true);
            $input.datepicker("hide");
            $input.datepicker("destroy");
            $input.remove();
        };

        this.show = function () {
            if (calendarOpen) {
                $.datepicker.dpDiv.stop(true, true).show();
            }
        };

        this.hide = function () {
            if (calendarOpen) {
                $.datepicker.dpDiv.stop(true, true).hide();
            }
        };

        this.position = function (position) {
            if (!calendarOpen) {
                return;
            }
            $.datepicker.dpDiv
                .css("top", position.top + 30)
                .css("left", position.left);
        };

        this.focus = function () {
            $input.focus();
        };

        this.loadValue = function (item) {
            defaultValue = item[args.column.field];
            $input.val(defaultValue);
            $input[0].defaultValue = defaultValue;
            $input.select();
        };

        this.serializeValue = function () {
            return $input.val();
        };

        this.applyValue = function (item, state) {
            item[args.column.field] = state;
        };

        this.isValueChanged = function () {
            return (!($input.val() == "" && defaultValue == null)) && ($input.val() != defaultValue);
        };

        this.validate = function () {
            return {
                valid: true,
                msg: null
            };
        };

        this.init();
    }

    function YesNoSelectEditor(args) {
        var $select;
        var defaultValue;
        var scope = this;
        var data = args.column.source;

        this.init = function () {
            var opt = "";
            if (data != undefined && data != "")
                $.each(data, function (index, val) {
                    opt = opt + "<OPTION value='" + val.MA + "'>" + val.TEN + "</OPTION>";
                });

            $select = $("<SELECT tabIndex='0' class='editor-yesno'>" + opt + "</SELECT>");
            $select.css("font-size", "13");
            $select.appendTo(args.container);
            $select.focus();
        };

        this.destroy = function () {
            $select.remove();
        };

        this.focus = function () {
            $select.focus();
        };

        this.loadValue = function (item) {
            $select.val(item["ID_" + args.column.field]);
            $select.select();
        };

        this.serializeValue = function () {
            return ($select.val());
        };

        this.applyValue = function (item, state) {
            item[args.column.field] = $select.find('option:selected').text();
            item["ID_" + args.column.field] = state
        };

        this.isValueChanged = function () {
            return ($select.val() != defaultValue);
        };

        this.validate = function () {
            return {
                valid: true,
                msg: null
            };
        };

        this.init();
    }

    function CheckboxEditor(args) {
        var $select;
        var defaultValue;
        var scope = this;

        this.init = function () {
            $select = $("<INPUT type='checkbox' value='true' class='editor-checkbox' style='position: absolute; left:45%;' >");
            $select.appendTo(args.container);
            $select.focus();
        };

        this.destroy = function () {
            $select.remove();
        };

        this.focus = function () {
            $select.focus();
        };

        this.loadValue = function (item) {
            if (item[args.column.field].toString() == "")
                $select.prop('checked', false);
            else {
                defaultValue = !!Boolean.parse(item[args.column.field].toString());
                if (defaultValue) {
                    $select.prop('checked', true);
                } else {
                    $select.prop('checked', false);
                }
            }
        };

        this.serializeValue = function () {
            return $select.prop('checked');
        };

        this.applyValue = function (item, state) {
            item[args.column.field] = state;
        };

        this.isValueChanged = function () {
            return (this.serializeValue() !== defaultValue);
        };

        this.validate = function () {
            return {
                valid: true,
                msg: null
            };
        };

        this.init();
    }

    function PercentCompleteEditor(args) {
        var $input, $picker;
        var defaultValue;
        var scope = this;

        this.init = function () {
            $input = $("<INPUT type=text class='editor-percentcomplete' />");
            $input.width($(args.container).innerWidth() - 25);
            $input.appendTo(args.container);

            $picker = $("<div class='editor-percentcomplete-picker' />").appendTo(args.container);
            $picker.append("<div class='editor-percentcomplete-helper'><div class='editor-percentcomplete-wrapper'><div class='editor-percentcomplete-slider' /><div class='editor-percentcomplete-buttons' /></div></div>");

            $picker.find(".editor-percentcomplete-buttons").append("<button val=0>Not started</button><br/><button val=50>In Progress</button><br/><button val=100>Complete</button>");

            $input.focus().select();

            $picker.find(".editor-percentcomplete-slider").slider({
                orientation: "vertical",
                range: "min",
                value: defaultValue,
                slide: function (event, ui) {
                    $input.val(ui.value)
                }
            });

            $picker.find(".editor-percentcomplete-buttons button").bind("click", function (e) {
                $input.val($(this).attr("val"));
                $picker.find(".editor-percentcomplete-slider").slider("value", $(this).attr("val"));
            })
        };

        this.destroy = function () {
            $input.remove();
            $picker.remove();
        };

        this.focus = function () {
            $input.focus();
        };

        this.loadValue = function (item) {
            $input.val(defaultValue = item[args.column.field]);
            $input.select();
        };

        this.serializeValue = function () {
            return parseInt($input.val(), 10) || 0;
        };

        this.applyValue = function (item, state) {
            item[args.column.field] = state;
        };

        this.isValueChanged = function () {
            return (!($input.val() == "" && defaultValue == null)) && ((parseInt($input.val(), 10) || 0) != defaultValue);
        };

        this.validate = function () {
            if (isNaN(parseInt($input.val(), 10))) {
                return {
                    valid: false,
                    msg: "Please enter a valid positive number"
                };
            }

            return {
                valid: true,
                msg: null
            };
        };

        this.init();
    }

    /*
     * An example of a "detached" editor.
     * The UI is added onto document BODY and .position(), .show() and .hide() are implemented.
     * KeyDown events are also handled to provide handling for Tab, Shift-Tab, Esc and Ctrl-Enter.
     */
    function LongTextEditor(args) {
        var $input, $wrapper;
        var defaultValue;
        var scope = this;

        this.init = function () {
            var $container = $("body");

            $wrapper = $("<DIV style='z-index:10000;position:absolute;background:white;padding:5px;border:3px solid gray; -moz-border-radius:10px; border-radius:10px;'/>")
                .appendTo($container);

            $input = $("<TEXTAREA hidefocus rows=5 style='backround:white;width:250px;height:80px;border:0;outline:0'>")
                .appendTo($wrapper);

            $("<DIV style='text-align:right'><BUTTON>Save</BUTTON><BUTTON>Cancel</BUTTON></DIV>")
                .appendTo($wrapper);

            $wrapper.find("button:first").bind("click", this.save);
            $wrapper.find("button:last").bind("click", this.cancel);
            $input.bind("keydown", this.handleKeyDown);

            scope.position(args.position);
            $input.focus().select();
        };

        this.handleKeyDown = function (e) {
            if (e.which == $.ui.keyCode.ENTER && e.ctrlKey) {
                scope.save();
            } else if (e.which == $.ui.keyCode.ESCAPE) {
                e.preventDefault();
                scope.cancel();
            } else if (e.which == $.ui.keyCode.TAB && e.shiftKey) {
                e.preventDefault();
                args.grid.navigatePrev();
            } else if (e.which == $.ui.keyCode.TAB) {
                e.preventDefault();
                args.grid.navigateNext();
            }
        };

        this.save = function () {
            args.commitChanges();
        };

        this.cancel = function () {
            $input.val(defaultValue);
            args.cancelChanges();
        };

        this.hide = function () {
            $wrapper.hide();
        };

        this.show = function () {
            $wrapper.show();
        };

        this.position = function (position) {
            $wrapper
                .css("top", position.top - 5)
                .css("left", position.left - 5)
        };

        this.destroy = function () {
            $wrapper.remove();
        };

        this.focus = function () {
            $input.focus();
        };

        this.loadValue = function (item) {
            $input.val(defaultValue = item[args.column.field]);
            $input.select();
        };

        this.serializeValue = function () {
            return $input.val();
        };

        this.applyValue = function (item, state) {
            item[args.column.field] = state;
        };

        this.isValueChanged = function () {
            return (!($input.val() == "" && defaultValue == null)) && ($input.val() != defaultValue);
        };

        this.validate = function () {
            return {
                valid: true,
                msg: null
            };
        };

        this.init();
    }

    function DropGrid(args) {

        var $input, $span1, $span2, $grid;
        var defaultValue;
        var scope = this;
        var grid;
        var dropId = args.grid.getGridID().replace("#", "");
        var typeData = args.column.dataType;
        var data = args.column.source;
        var gridID = dropId + "_grid";
        var dropID = dropId + "_" + args.column.field;
        var itemTemplate = args.column.itemTemplate;
        var htmlGrid;
        this.init = function () {
            $("div[id*=" + gridID + "]").remove();
            for (var i = 0; i < itemTemplate.length; i++) {
                itemTemplate[i].width = parseInt(itemTemplate[i].width);
            }
            var lenData = data.length;
            _height = (lenData > 120 ? 120 : lenData + 10);
            $input = $("<span class='combo-grid combo-grid-text' id='" + dropID + "'/>")
             .bind("keydown.nav", function (e) {
                 if (e.keyCode === Sys.UI.Key.left || e.keyCode === Sys.UI.Key.right) {
                     e.stopImmediatePropagation();
                 }
             });

            $input.css("width", "100%");
            $input.css("margin-top", "-4px");
            $span1 = $("<input  type=\"text\"  class=\"textbox-text  validatebox-text textbox-prompt\" style=\"width: 99%; padding-top: 2px; padding-bottom: 1px; margin-right: 18px; margin-left: 0px;\"/>");
            $span1.appendTo($input);

            $span2 = $("<span class=\"combo-grid-text-addon\" style=\"right: 0px;\"><a tabindex=\"-1\" class=\"combo-grid-text-addon-icon combo-grid-arrow\" id=\"" + dropId + "_arrow\" style=\"border: 0px currentColor; border-image: none; width: 18px; height: 20px;\" /></a>");
            $span2.appendTo($input);

            var elementOffset = Common.round($(args.container).offset().left, 0),
            top = Common.round($(args.container).offset().top, 0);

            htmlGrid = ("<div id=\"" + gridID + "\" style=\"width: 410px; height:350px;  border:1px solid #808080; z-index: 30000; top:" + top + 23 + "px; left: " + elementOffset + "px \"/>");
            $grid = $(htmlGrid);
            $grid.css("left", elementOffset);
            $grid.css("top", top + 23);

            $input.appendTo(args.container);
            $grid.appendTo(document.body);
            $grid.hide();
            $input.attr("selector", true);
            if (typeData == "Number" || typeData == "Money") {
                $span1.css("text-align", "right");
            }
            if (args.column.lke != "" || typeData == "Date" || typeData == "Month") {
                $span1.css("text-align", "center");
                $span1.css("cursor", "pointer");
            }

            var options = {
                editable: true,
                hidenHeader: false,
                enableColumnReorder: false,
                enableCellNavigation: true,
                asyncEditorLoading: true,
                forceFitColumns: false,
                topPanelHeight: 25
            };

            grid = new Slick.Grid("#" + gridID, data, itemTemplate, options);
            grid.setSelectionModel(new Slick.RowSelectionModel());

            grid.onActiveCellChanged.subscribe(function (item, arg) {
                //debugger;
                var row = arg.row;
                if (row == undefined)
                    return null;
                var cell = arg.cell;
                if (cell == undefined)
                    return null;
                var a_data = arg.grid.getDataItem(row);
                var colName = arg.grid.getColumns()[cell].field;
                var val = a_data[colName];
                $span1.val(val);
                $grid.hide(400);

            });

            $grid.show(400);
            //click arrow
            $("#" + dropId + "_arrow").click(function (e) {
                // debugger;
                var flag = Common.toBool($input.attr("selector"));
                if (flag) {
                    $grid.hide(400);
                    // grid.scrollRowToTop(0);
                    $input.attr("selector", false);
                }
                else {
                    $grid.show(400);
                    $input.attr("selector", true);
                    //grid.scrollRowToTop(0);
                    //Close
                }
                e.preventDefault();
                return false;
            });
            $('body').bind('click', function (e) {
                var flag = Common.toBool($input.attr("selector"));
                if (flag) {
                    $grid.hide(400);
                    // grid.scrollRowToTop(0);
                    $input.attr("selector", false);
                }
                e.preventDefault();
                return false;
            });

        };

        this.destroy = function () {
            $input.remove();
        };

        this.focus = function () {
            $span1.focus();
        };

        this.loadValue = function (item) {
            //debugger;
            $span1.val(defaultValue = item[args.column.field]);
            $span1.select();
        };

        this.serializeValue = function () {
            return parseInt($span1.val(), 10) || 0;
        };

        this.applyValue = function (item, state) {
            item[args.column.field] = state;
        };
        this.isValueChanged = function () {
            return ($span1.val() != defaultValue);
        };
        this.validate = function () {
            return {
                valid: true,
                msg: null
            };
        };
        this.init();
    }

})(jQuery);
