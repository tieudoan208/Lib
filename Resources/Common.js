/// <reference path="jquery-1.11.0.js" />
/// <reference name="MicrosoftAjax.js"/>
///Quannm:
Common = function () {
    return {
        //Lấy keycode trong mọi trình duyệt
        getKeyCode: function (event, eventType) {
            if (window.event) {
                event = window.event;
            }
            if (eventType == "keypress")
                return (event.charCode == undefined ? event.keyCode : event.charCode);
            else
                return event.keyCode;
        },
        //Vịt trị contro
        setlectRang: function (input, selectionStart, selectionEnd) {
            if (input.setSelectionRange) {
                input.focus();
                input.setSelectionRange(selectionStart, selectionEnd);
            }
            else if (input.createTextRange) {
                var range = input.createTextRange();
                range.collapse(true);
                range.moveStart('character', selectionStart);
                range.select();
            }
        },
        //Kiểm tra có phải là ký tự số ko
        isKeyDigit: function (keyCode) {
            return (0x30 <= keyCode && keyCode <= 0x39);
        },
        isKeyNavigation: function (keyCode) {
            return (Sys.UI.Key.left <= keyCode && keyCode <= Sys.UI.Key.down);
        },
        getCaret: function (el) {
            if (el.selectionStart) {
                return el.selectionStart;
            }
            else if (document.selection) {
                el.focus();
                var r = document.selection.createRange();
                if (r == null) {
                    return 0;
                }

                var re = el.createTextRange(),
                    rc = re.duplicate();
                re.moveToBookmark(r.getBookmark());
                rc.setEndPoint('EndToStart', re);
                return rc.text.length;
            }
            return 0;
        },
        getTextSelect: function (el) {
            var selection = "";
            if ('selectionStart' in el) {
                if (el.selectionStart != el.selectionEnd) {
                    selection = el.value.substring(el.selectionStart, el.selectionEnd);
                }
            }
            else {
                var textRange = document.selection.createRange();
                var rangeParent = textRange.parentElement();
                if (rangeParent === el) {
                    selection = textRange.text;
                }
            }
            return selection;
        },
        round: function (number, tp) {
            return Math.round(number * Math.pow(10, tp)) / Math.pow(10, tp);
        },
        onNextControl: function (control, e) {
            var b_form = document.forms[0], b_vtri = 0, b_tim = -1;
            for (var i = 0; i < b_form.elements.length - 1; i++) {
                if (b_tim < 0 && b_form.elements[i].type != 'hidden' && b_form.elements[i].focus
                    && this.NVL(b_form.elements[i].style.display) != "none" && b_form.elements[i].disabled == false) {
                    b_tim = i;
                }

                if (control == b_form.elements[i]) {
                    b_vtri = i + 1;
                    break;
                }
            }

            for (var i = b_vtri; i < b_form.elements.length; i++) {
                if (b_form.elements[i].type != 'hidden' && b_form.elements[i].focus
                    && this.NVL(b_form.elements[i].style.display) != "none" && b_form.elements[i].disabled == false) {
                    b_tim = i;
                    break;
                }
            }

            if (b_tim >= 0) {
                b_form.elements[b_tim].focus();
                if (b_form.elements[b_tim].type != "select-one")
                    b_form.elements[b_tim].select();
            }
        },
        onPreviousControl: function (control) {
            var b_form = document.forms[0], b_vtri = 0, b_tim = -1;
            for (var i = 0; i < b_form.elements.length - 1; i++) {
                if (b_tim < 0 && b_form.elements[i].type != 'hidden' && b_form.elements[i].focus &&
                    this.NVL(b_form.elements[i].style.display) != "none" && b_form.elements[i].disabled == false) {
                    b_tim = i;
                }

                if (control == b_form.elements[i]) {
                    b_vtri = i - 1;
                    break;
                }
            }

            for (var i = b_vtri; i < b_form.elements.length; i++) {
                if (b_form.elements[i].type != 'hidden' && b_form.elements[i].focus
                    && this.NVL(b_form.elements[i].style.display) != "none" && b_form.elements[i].disabled == false) {
                    b_tim = i;
                    break;
                }
            }

            if (b_tim >= 0) {
                b_form.elements[b_tim].focus();
                if (b_form.elements[b_tim].type != "select-one")
                    b_form.elements[b_tim].select();
            }
            return false;
        },
        daysOfMonth: function (moth, year) {
            var mon = parseInt(moth, 10);
            var yar = parseInt(year, 10);
            switch (mon) {
                case 2:
                    if ((yar % 4 == 0) && (yar % 400 != 0))
                        return 29;
                    else
                        return 28;
                    break;
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    return 31;
                    break;
                default:
                    return 30;
            }
        },
        // Kiểm tra xem chuỗi có chứa ký tự số không
        containsDigits: function (value) {
            return /\d/g.test(value);
        },
        formCodeToChar: function (code) {
            return String.fromCharCode(code);
        },
        isNullOrEmty: function (value) {
            if (value == null || value == "" || value == undefined)
                return true;
            else
                return false;
        },
        isNumber: function (input) {
            return $.isNumeric(input);
        },
        transformValue: function (currentVal, listVal) {
            if (this.isNullOrEmty(listVal)) return currentVal;
            var aVal = listVal.split(","), bPost = -1, i;
            for (i = 0; i < aVal.length; i++) {
                if (currentVal == aVal[i]) { bPost = i + 1; break; }
            }
            if (bPost < 0 || bPost >= aVal.length) bPost = 0;
            return aVal[bPost];
        },
        stringToJson: function (val) {
            if (val == "" || val == undefined) return "";
            return jQuery.parseJSON(val);
        },
        jsonToString: function (obj) {
            if ("JSON" in window) {
                return JSON.stringify(obj);
            }

            var t = typeof (obj);
            if (t != "object" || obj === null) {
                // simple data type
                if (t == "string") obj = '"' + obj + '"';

                return String(obj);
            } else {
                // recurse array or object
                var n, v, json = [], arr = (obj && obj.constructor == Array);

                for (n in obj) {
                    v = obj[n];
                    t = typeof (v);
                    if (obj.hasOwnProperty(n)) {
                        if (t == "string") {
                            v = '"' + v + '"';
                        } else if (t == "object" && v !== null) {
                            v = jQuery.stringify(v);
                        }

                        json.push((arr ? "" : '"' + n + '":') + String(v));
                    }
                }

                return (arr ? "[" : "{") + String(json) + (arr ? "]" : "}");
            }
        },
        jsonToArray: function (json) {
            var name = [], val = [];
            var i = 0;
            $.each(json, function (index, value) {
                name[i] = index;
                val[i] = value;
                i++;
            });
            return [name, val];
        },
        NVL: function (val, newval) {
            if (val == undefined || val == null) return "";
            else return val;
        },
        deCode: function (val, condi, valT, valF) {
            if (val == condi)
                return valT;
            else return valF;
        },
        toBool: function (val) {
            return Boolean.parse(val);
        },
        toInt: function (val) {
            return parseInt(val);
        },
        stringNumberToNumber: function (val) {
            if (val == "") return 0;
            while (val.indexOf(',') >= 0) val = val.replace(',', '');
            return val * 1.0;
        },
        isEmtyDate: function (val) {
            if (val == null) return true;
            val = this.NVL(val);
            while (val.indexOf('/') >= 0) val = val.replace('/', '');
            return (val == "");
        },
        numberToStringDate: function (val) {
            if (val == 0 || val == undefined)
                return "";
            var sNumber = val.toString();
            var year = sNumber.substr(0, 4), b_month = sNumber.substr(4, 2), b_day = sNumber.substr(6, 2);
            return b_day + "/" + b_month + "/" + year;
        },
        numberToStringMonth: function (val) {
            if (val == 0 || val == undefined)
                return "";
            var sNumber = val.toString();
            var year = sNumber.substr(0, 4), b_month = sNumber.substr(4, 2);
            return b_month + "/" + year;
        },
        stringDateToDate: function (val) {
            if (this.isEmtyDate(val)) val = "01/01/30000";
            var day = parseInt(val.substr(0, 2), 10), month = parseInt(val.substr(3, 2), 10), year = parseInt(val.substr(6, 4), 10);
            var result = new Date(year, month, day);
            return result;
        },
        dateToStringDate: function (val) {
            var day = b_ngay.getDate().toString(), month = (b_ngay.getMonth() + 1).toString();
            if (day.length < 2) day = "0" + day
            if (month.length < 2) month = "0" + month;
            return day + '/' + month + '/' + val.getFullYear().toString();
        },
        checkDate: function (b_value, typeData) {
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
            return true;
        },
        stringMonthToNumber: function (month) {
            if (month.indexOf("_") >= 0) {
                Msg.show("Cảnh báo", "tháng không hợp lê");
                return;
            }
            var a_month = month.split("/");
            return a_month[1] + a_month[0];
        },
        stringDateToNumber: function (val) {
            if (this.NVL(val) == "") return 0;
            if (val.indexOf("_") >= 0) {
                Msg.show("Cảnh báo", "Ngày không hợp lê");
                return;
            }
            return val.substr(6, 4) + val.substr(3, 2) + val.substr(0, 2);
        },
        stringToNumber: function (val, dec) {
            val = this.stringToFormatNumber(val, dec);
            if (val == "") return 0;
            while (val.indexOf(',') >= 0) val = val.replace(',', '');
            return val * 1.0;
        },
        formatNumberToNumber: function (numFormat) {
            if (typeof (numFormat) == "number")
                numFormat = numFormat.toString();
            if (numFormat == "") return 0;
            while (numFormat.indexOf(',') >= 0) numFormat = numFormat.replace(',', '');
            return numFormat * 1.0;
        },
        stringToFormatNumber: function (val, dec) {
            if (typeof (val) == "number")
                val = val.toString();
            if (val == null || val == "") return "";
            var b_moi = "", b_chan = "", b_tp = "", b_dau = "", b_cham = "", b_s, i;
            for (i = 0; i < val.length; i++) {
                b_s = val.substr(i, 1);
                if (b_s == ".") b_cham = ".";
                else if (b_s == "-") b_dau = "-";
                else if ("0123456789".indexOf(b_s) >= 0) {
                    if (b_cham == ".") dec = dec + b_s;
                    else if (b_chan == "0") b_chan = b_s;
                    else b_chan = b_chan + b_s;
                }
            }
            if (b_chan.length > 3) {
                while (b_chan != "") {
                    i = b_chan.length - 3;
                    if (i < 0) i = 0;
                    if (b_moi != "") b_moi = b_chan.substr(i) + "," + b_moi;
                    else b_moi = b_chan.substr(i);
                    if (i == 0) b_chan = ""; else b_chan = b_chan.substr(0, i);
                }
            }
            else b_moi = b_chan;
            if (dec != null)
                if (dec > 0 && b_tp.length > dec) b_tp = b_tp.substr(0, dec);
            b_moi = b_dau + b_moi + b_cham + b_tp;
            return b_moi;
        },
        parseData: function (data, column, max) {
            if (data.length > max) return data;
            var len = data.length;;
            for (var j = 0; j <= max - len; j++) {
                var obj = [];
                for (k = 0; k < column.length; k++) {
                    obj[column[k].field] = "";
                }
                data.push(obj);
            }
            return data;
        },
        setTooltip: function (id, ndung) {
            $("#"+id).tooltip({
                content: ndung,
                track: true
            });
        }
    };
}();


Msg = function () {
    return {
        show: function (title, msg, calbackfunction) {
            this.stopWait();
            var b_d = msg.indexOf("loi:"),
            b_c = msg.lastIndexOf(":loi");
            if (b_d >= 0 && b_c < 0)
                msg = msg.substr(b_d + 4, msg.length - 4);
            else if (b_d < 0 && b_c >= 0)
                msg = msg.substr(0, msg.length - 5);
            else if (b_d >= 0 && b_c > 0)
                msg = msg.substr(b_d + 4, msg.length - 8);
            $("<div  title=\"" + title + "\"><p>" + msg + "</p></div>").appendTo("BODY").dialog(
                {
                    modal: true,
                    buttons: {
                        "OK": function () {
                            $(this).dialog("close");
                            if (calbackfunction != undefined) {
                                calbackfunction(this);
                                return false;
                            }
                        }
                    }
                });
            $(".ui-button-text").focus();
        },
        confirm: function (calbackfunction, msg) {
            this.stopWait();
            if (msg == undefined)
                msg = "Bạn có chắc chắn xóa không";
            $("<div  title=\"Thông báo\"><p>" + msg + "</p></div>").appendTo("BODY").dialog(
                {
                    modal: true,
                    buttons: {
                        "Yes": function () {
                            $(this).dialog("close");
                            calbackfunction(true);
                        },
                        "No": function () {
                            $(this).dialog('close');
                            calbackfunction(false);
                        }
                    }
                });
            $(".ui-button-text").focus();
        },
        //
        openWindow: function (title, url, width, heigth, id) {
            this.stopWait();
            var dialog = $('<div id="' + id + '"  style="width:' + width + '; height:' + heigth + '; overflow:hidden;  "></div>')
                .html('<iframe  width="100%" height="100%" frameborder="0" src=' + url + ' marginwidth="0" marginheight="0"  pramater=\"\" scrolling="no"></iframe>')
                .dialog({
                    title: title,
                    modal: true,
                    width: width,
                    height: heigth,
                    close: function () {
                        $("div[id*=" + id + "]").remove();
                    }
                });
            return dialog;
        },
        wait: function () {
            $('<div id="che_trang" class="ui-widget-overlay ui-front"></div>').appendTo('body');
            var doi = $('<div id="doi_trangnen" div class="css_tbao" style="width: 200px; height: 50px; padding-top: 5px; top: 50%; left: 50%;z-index:' +
                   '1000; display: block; position: absolute;">' +
                   '<div  class="loadding" ></div> <div style="float:right; width: 70%;  margin-top:15px;"> Đang xử lý. Xin đợi!</div>' +
           '</div>');
            if (doi.parents().context != undefined)
                doi = doi.parents();
            doi.appendTo('body');
        },
        stopWait: function () {
            $("div[id*=doi_trangnen]").remove();
            $("div[id*=che_trang]").remove();
        },
        openDiv: function (title, name, width, height) {
            this.stopWait();
            var dialog = $("#" + name).dialog({
                title: title,
                width: width,
                modal: true,
                height: height
            });
            $("#" + name).dialog("open");
            return dialog;
        }
    };
}();