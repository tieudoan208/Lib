///<reference path="Common.js"/>
function ErrorDb(err) {
    Msg.stopWait();
    Msg.show("Cảnh báo", err);
}
function ErrorTimeOut(err) {
    Msg.stopWait();
    Msg.show("Cảnh báo", err);
}
function CheckError(error) {
    Msg.stopWait();
    if (Common.NVL(error) == "") return false;
    return (error.indexOf("loi:") >= 0 && error.lastIndexOf(":loi") >= 0);
}
function ShowError(error) {
    try {
        Msg.stopWait();
        var error = (typeof (error) != "string") ? error.message : error;
        error = Common.NVL(error);
        if (error == "") return;
        var b_d = error.indexOf("loi:"),
            b_c = error.lastIndexOf(":loi");
        if (b_d >= 0 && b_c < 0)
            error = error.substr(b_d + 4, error.length - 4);
        else if (b_d < 0 && b_c >= 0)
            error = error.substr(0, error.length - 5);
        else if (b_d >= 0 && b_c > 0)
            error = error.substr(b_d + 4, error.length - 8);
        if (error != "") Msg.show("Cảnh báo", error);
    }
    catch (err) { }
}


function FormSize(b_rong, b_cao) {
    try {
        if (b_rong == 0) b_rong = screen.availWidth;
        if (b_cao == 0) b_cao = screen.availHeight;
        var b_x = Common.round((screen.availWidth - b_rong) / 2, 0), b_y = Common.round((screen.availHeight - b_cao) / 2, 0);
        moveTo(b_x, b_y); resizeTo(b_rong, b_cao);
    }
    catch (err) { }
}


//Tra Form goc
function GetForm() {
    var b_g = window, b_f = window.opener;
    try {
        while (b_f != null && b_f != undefined && b_f.name != null) { b_g = b_f; b_f = b_g.opener; }
        b_f = null;
    }
    catch (err) { }
    return b_g;
}

// Tra form theo ten
function FormGetName(url) {
    try {
        var b_i = url.lastIndexOf("/") + 1;
        var b_ten = (b_i > 0) ? url.substr(b_i) : url;
        b_i = b_ten.lastIndexOf(".");
        if (b_i > 0) b_ten = b_ten.substr(0, b_i);
        return b_ten;
    }
    catch (err) { return null; }
}
function AddString(oldString, val, charac) {
    if (oldString == "" || oldString == null) oldString = val;
    else oldString += ";" + val;
    return oldString;
}
//Tim chuoi trong chuoi dang dung
function IsInString(odlString, val) {
    if (Common.NVL(odlString) == "" || Common.NVL(val) == "") return false;
    odlString = odlString.toUpperCase(); val = val.toUpperCase();
    var odlString = odlString.split(';');
    for (var i = 0; i < odlString.length; i++)
        if (odlString[i] == val) return true;
}
function OpenWindow(url, tittle, pramater, bbuoc, kthuoc) {
    try {
        //   debugger;
        var form = GetForm(), formName = FormGetName(url);
        var isOpen = $(form).attr("isOpen"), b_s = "";
        if (isOpen == null || isOpen == undefined) isOpen = "";
        if (eval("form." + formName) == null || bbuoc == "C") {
            if (eval("form." + formName) != null)
                eval("form." + formName + "=null;");
            var width = 100, height = 40;
            if (kthuoc != undefined) {
                width = kthuoc[0]; height = kthuoc[1];
            }
            var b_x = Common.round((screen.availWidth - width) / 2, 0).toString(), b_y = Common.round((screen.availHeight - height) / 2, 0).toString();
            var b_opt = "width=" + width.toString() + ",height=" + height.toString() + ",left=" + b_x + ",top=" + b_y;
            b_opt += ",resizable=yes,location=no,scrollbars=yes, titlebar=no, menubar=no,status =no,toolbar =no";
            eval("form." + formName + "=form.open('" + url + "','','" + b_opt + "');");
            if (!IsInString(isOpen, formName))
                $(form).attr("isOpen", AddString(isOpen, formName));
        }
        $(form).attr("openform", tittle);
        eval("form." + formName + ".focus();");

        if (pramater != null && pramater != "") {
            var frm = eval("form." + formName);
            if (frm.Result != null) frm.Result(pramater[0], pramater[1]);
        }
        return false;
    }
    catch (err) { }
}
// Dong form. THêm biến Temp để chạy dược IE, cần test thêm
function CloseForm(nameForm, pramater) {
    try {
        if (nameForm == "login") return;
        var form = GetForm();
        var open = $(form).attr("isOpen");
        if (open == null || open == undefined || open == "") return;
        var aOpen = open.split(';');
        if (nameForm != form.name) {
            var b_tra = $(form).attr("openform");
            eval("form." + nameForm + ".close();"); eval("form." + nameForm + "=null;");
            open = "";
            for (var i = 0; i < aOpen.length; i++) {
                if (aOpen[i] != nameForm) open = AddString(open, aOpen[i]);
            }
            $(form).attr("isOpen", open);
            if (pramater != null && b_tra != null && b_tra != "") {
                var a_s = b_tra.split(',');
                if (IsInString(open, a_s[0])) {
                    if (eval("form." + a_s[0] + ".Result") != null) {
                        eval("form." + a_s[0] + ".focus();");
                        eval("form." + a_s[0] + ".Result(a_s[1], pramater);");
                    }
                }
            }
        }
        else {
            for (var i = 0; i < aOpen.length; i++) {
                eval("form." + aOpen[i] + ".close();"); eval("form." + aOpen[i] + "=null;");
            }
        }
    }
    catch (err) { }
}
//Lấy một đối tượng theo ID 
function GetObjectByID(idObject) {
    var object = (idObject == "") ? null : $get(idObject);
    if (object == null || object == undefined) object = document;
    return object;
}
//lấy giá trị trên form checked
function GetAllValueForm(b_vungId) {
    try {
        var values=GetAllValueFormJson(b_vungId);
        return Common.jsonToArray(Common.stringToJson(values));
    }
    catch (err) { ShowError(err); }
}
function GetAllValueFormJson(b_vungId)
{
    var b_gtri = "{";
    var a_vungId = b_vungId.split(',');
    for (var j = 0; j < a_vungId.length; j++) {
        var b_vung = GetObjectByID(a_vungId[j]);
        var a_ctr = b_vung.getElementsByTagName("input");
        for (var i = 0; i < a_ctr.length; i++) {
            if ($find(a_ctr[i].id) != null) {
                var b = "false";
                try {
                    b = $find(a_ctr[i].id).get_isInputData();
                }
                catch (e) { b = "false"; }
                if (Boolean.parse(b)) {
                    b_gtri = b_gtri + "\"" + $find(a_ctr[i].id).get_name() + "\":";
                    var value = "";
                    var type = $find(a_ctr[i].id).get_saveTypeData();
                    var _decimal = 0;
                    if ($(a_ctr[i]).attr("type") == "text" || $(a_ctr[i]).attr("type") == "password") {
                        _decimal = $find(a_ctr[i].id).get_decimal();
                        value = Common.NVL(a_ctr[i].value);
                    }
                    else if ($(a_ctr[i]).attr("type") == "checkbox")
                        value = a_ctr[i].checked;
                    if (_decimal > 0) {
                        var val = Common.stringNumberToNumber(value);
                        b_gtri = b_gtri + parseInt(val) + ",";
                    } else {
                        if (type == "Date")
                            b_gtri = b_gtri + "\"" + Common.stringDateToDate(value) + "\",";
                        else if (type == "Number")
                            b_gtri = b_gtri + parseInt(Common.stringDateToNumber(value)) + ",";
                        else if (type == "Unicode")
                            b_gtri = b_gtri + "\"N'" + value.toString() + "'\",";
                        else
                            b_gtri = b_gtri + "\"" + value.toString() + "\",";
                    }
                }
            }
        }
        //dropdowlist
        a_ctr = b_vung.getElementsByTagName("select");
        for (var i = 0; i < a_ctr.length; i++) {

            if ($find(a_ctr[i].id) != null) {
                var b = "false";
                try {
                    b = $find(a_ctr[i].id).get_isInputData();
                }
                catch (e) { b = "false"; }
                if (Boolean.parse(b)) {
                    b_gtri = b_gtri + "\"" + $find(a_ctr[i].id).get_name().toUpperCase() + "\":";
                    var value = Common.NVL(a_ctr[i].value);
                    var type = $find(a_ctr[i].id).get_saveType();
                    if (type == "Date")
                        b_gtri = b_gtri + "\"" + Common.stringDateToDate(value) + "\",";
                    else if (type == "Number")
                        b_gtri = b_gtri + parseInt(Common.stringDateToNumber(value)) + ",";
                    else if (type == "Unicode")
                        b_gtri = b_gtri + "\"N'" + value.toString() + "'\",";
                    else
                        b_gtri = b_gtri + "\"" + value.toString() + "\",";
                }
            }
        }
        a_ctr = b_vung.getElementsByTagName("textarea");
        for (var i = 0; i < a_ctr.length; i++) {
            if ($find(a_ctr[i].id) != null) {
                var b = "false";
                try {
                    b = $find(a_ctr[i].id).get_isInputData();
                }
                catch (e) { b = "false"; }
                if (Boolean.parse(b)) {
                    b_gtri = b_gtri + "\"" + $find(a_ctr[i].id).get_name().toUpperCase() + "\":";
                    var value = Common.NVL(a_ctr[i].value);
                    var type = $find(a_ctr[i].id).get_saveTypeData();
                    if (type == "Unicode")
                        b_gtri = b_gtri + "\"N'" + value.toString() + "'\",";
                    else
                        b_gtri = b_gtri + "\"" + value.toString() + "\",";
                }
            }
        }
    }
    if (b_gtri.length > 1)
        b_gtri = b_gtri.substr(0, b_gtri.length - 1);
    b_gtri = b_gtri + "}";
    return b_gtri;
}
//Lấy giá trị form theo vùng nào đó
function GetValueByName(b_vungId, name) {
    try {
        name = name.toUpperCase();
        var a_vungId = b_vungId.split(','), nameControl = "";
        for (var j = 0; j < a_vungId.length; j++) {
            var b_vung = GetObjectByID(a_vungId[j]);
            var a_ctr = b_vung.getElementsByTagName("input");
            for (var i = 0; i < a_ctr.length; i++) {
                if ($find(a_ctr[i].id) != null) {
                    var b = "false";
                    try {
                        b = $find(a_ctr[i].id).get_isInputData();
                    }
                    catch (e) { b = "false"; }
                    if (Boolean.parse(b)) {
                        nameControl = $find(a_ctr[i].id).get_name();
                        if (name.toUpperCase() == nameControl.toUpperCase())
                            return a_ctr[i].value;
                    }
                }
            }
            a_ctr = b_vung.getElementsByTagName("select");
            for (var i = 0; i < a_ctr.length; i++) {
                if ($find(a_ctr[i].id) != null) {
                    var b = "false";
                    try {
                        b = $find(a_ctr[i].id).get_isInputData();
                    }
                    catch (e) { b = "false"; }
                    if (Boolean.parse(b)) {
                        nameControl = $find(a_ctr[i].id).get_name();
                        if (name.toUpperCase() == nameControl.toUpperCase())
                            return a_ctr[i].value;
                    }
                }
            }
            a_ctr = b_vung.getElementsByTagName("textarea");
            for (var i = 0; i < a_ctr.length; i++) {
                if ($find(a_ctr[i].id) != null) {
                    var b = "false";
                    try {
                        b = $find(a_ctr[i].id).get_isInputData();
                    }
                    catch (e) { b = "false"; }
                    if (Boolean.parse(b)) {
                        nameControl = $find(a_ctr[i].id).get_name();
                        if (name.toUpperCase() == nameControl.toUpperCase())
                            return a_ctr[i].value;
                    }
                }
            }
        }
        return "";
    }
    catch (err) { return ""; }
}


//Lấy Object by name
function GetObjectByName(b_vungId, name) {
    try {
        name = name.toUpperCase();
        var a_vungId = b_vungId.split(','), b_gocId = "", b_id = "", b_ten_goc = "";
        for (var j = 0; j < a_vungId.length; j++) {
            var b_vung = GetObjectByID(a_vungId[j]);
            var a_ctr = b_vung.getElementsByTagName("input");
            for (var i = 0; i < a_ctr.length; i++) {
                if ($find(a_ctr[i].id) != null) {
                    var nameInput = $find(a_ctr[i].id).get_name();
                    if (name.toUpperCase() == nameInput.toUpperCase())
                        return a_ctr[i];
                }
            }
            a_ctr = b_vung.getElementsByTagName("span");
            for (var i = 0; i < a_ctr.length; i++) {
                if ($find(a_ctr[i].id) != null) {
                    var nameInput = $find(a_ctr[i].id).get_name();
                    if (name.toUpperCase() == nameInput.toUpperCase())
                        return a_ctr[i];
                }
            }
            a_ctr = b_vung.getElementsByTagName("select");
            for (var i = 0; i < a_ctr.length; i++) {
                if ($find(a_ctr[i].id) != null) {
                    var nameInput = $find(a_ctr[i].id).get_name();
                    if (name.toUpperCase() == nameInput.toUpperCase())
                        return a_ctr[i];
                }
            }
            a_ctr = b_vung.getElementsByTagName("textarea");
            for (var i = 0; i < a_ctr.length; i++) {
                if ($find(a_ctr[i].id) != null) {
                    var nameInput = $find(a_ctr[i].id).get_name();
                    if (name.toUpperCase() == nameInput.toUpperCase())
                        return a_ctr[i];
                }
            }
            a_ctr = b_vung.getElementsByTagName("button");
            for (var i = 0; i < a_ctr.length; i++) {
                if ($find(a_ctr[i].id) != null) {
                    var nameInput = $find(a_ctr[i].id).get_name();
                    if (name.toUpperCase() == nameInput.toUpperCase())
                        return a_ctr[i];
                }
            }
            a_ctr = b_vung.getElementsByTagName("img");
            for (var i = 0; i < a_ctr.length; i++) {
                if ($find(a_ctr[i].id) != null) {
                    var nameInput = $find(a_ctr[i].id).get_name();
                    if (name.toUpperCase() == nameInput.toUpperCase())
                        return a_ctr[i];
                }
            }
        }
        return null;
    }
    catch (err) { return null; }
}
//Reset form
function ResetForm(b_vungId) {
    try {
        var a_vungId = b_vungId.split(','), checkReset = "";
        for (var j = 0; j < a_vungId.length; j++) {
            var b_vung = GetObjectByID(a_vungId[j]);
            var a_ctr = b_vung.getElementsByTagName("input");
            for (var i = 0; i < a_ctr.length; i++) {
                if ($find(a_ctr[i].id) != null) {
                    var b = "false";
                    try {
                        b = $find(a_ctr[i].id).get_isReset();
                    }
                    catch (e) { b = "false"; }
                    if (Boolean.parse(b))
                        a_ctr[i].value = "";
                }
            }
            var a_ctr = b_vung.getElementsByTagName("textarea");
            for (var i = 0; i < a_ctr.length; i++) {
                if ($find(a_ctr[i].id) != null) {
                    var b = "false";
                    try {
                        b = $find(a_ctr[i].id).get_isReset();
                    }
                    catch (e) { b = "false"; }
                    if (Boolean.parse(b))
                        a_ctr[i].value = "";
                }
            }
            a_ctr = b_vung.getElementsByTagName("span");
            for (var i = 0; i < a_ctr.length; i++) {
                if ($find(a_ctr[i].id) != null) {
                    var b = "false";
                    try {
                        b = $find(a_ctr[i].id).get_isReset();
                    }
                    catch (e) { b = "false"; }
                    if (Boolean.parse(b))
                        a_ctr[i].value = "";
                }
            }
        }
    }
    catch (err) { ShowError(err) }
}

function SetValueJsonForm(b_vungId, a_json) {
    try {
        var b_vtri = 0, b_ten = "", a_vungId = b_vungId.split(','), b_so_tp = 0;
        for (var j = 0; j < a_vungId.length; j++) {
            var b_vung = GetObjectByID(a_vungId[j]);
            var a_ctr = b_vung.getElementsByTagName("input");
            for (var i = 0; i < a_ctr.length; i++) {
                if ($find(a_ctr[i].id) != null) {
                    var b = "false", nameControl, value;
                    try {
                        b = $find(a_ctr[i].id).get_isInputData();
                        nameControl = $find(a_ctr[i].id).get_name().toUpperCase();
                        value = a_json[nameControl];
                        value = (value == null ? "" : value);
                    }
                    catch (e) { b = "false"; }
                    if (Boolean.parse(b)) {
                        //Nếu ko có dữ liệu thì thoát
                        if ($(a_ctr[i]).attr("type") == "text") {
                            {
                                var type = $find(a_ctr[i].id).get_typeData();
                                _decimal = $find(a_ctr[i].id).get_decimal();
                                if (type == "Date")
                                    $(a_ctr[i]).val(Common.numberToStringDate(value));
                                else if (type == "Month")
                                    $(a_ctr[i]).val(Common.numberToStringMonth(value));
                                else if (type == "Money")
                                    $(a_ctr[i]).val(Common.stringToFormatNumber(value, _decimal));
                                else
                                    $(a_ctr[i]).val(value);
                            }
                        }
                        else if ($(a_ctr[i]).attr("type") == "checkbox") {
                            var val = value.toUpperCase();
                            if (val == "T" || val == "TRUE")
                                a_ctr[i].checked = true;
                            else a_ctr[i].checked = false;
                        }
                    }
                }
            }
            a_ctr = b_vung.getElementsByTagName("select");
            for (var i = 0; i < a_ctr.length; i++) {
                if ($find(a_ctr[i].id) != null) {
                    var b = "false";
                    try {
                        b = $find(a_ctr[i].id).get_isInputData();
                    }
                    catch (e) { b = "false"; }
                    if (Boolean.parse(b)) {
                        var nameControl = $find(a_ctr[i].id).get_name().toUpperCase();
                        var value = a_json[nameControl];
                        $(a_ctr[i]).val(value);
                    }
                }
            }
        }
    }
    catch (err) {
        ShowError(err);
    }
}
//Đưa dữ liệu vào form
function SetValueIntoForm(b_vungId, a_gtri) {
    try {

        if (a_gtri == null) return "";
        var data = Common.stringToJson(a_gtri), a_json = "";
        if (data.length == 0) return;
        a_json = data[0]; 
        SetValueJsonForm(b_vungId, a_json);
    }
    catch (err) { ShowError(err); }
}

//Tra giá trị chọn
function returnResult(object) {
    var aObject = object.split(",");
    var aResult = [];
    for (var i = 0; i < aObject.length; i++) {
        if (aObject[i].indexOf(":") >= 0) {
            var aName = aObject[i].split(":");
            var objGrid = GetObjectByName("", aName[0]);
            if (Common.NVL(objGrid) != "") {
                var grid = GetGridByID(objGrid);
                aResult[i] = grid.GetValueActive(aName[1]);
            }
            else aResult[i] = "";
        }
        else
            aResult[i] = GetValueByName("", aObject[i]);
    }
    CloseForm(window.name, aResult);
    return false;
}

//ID DROP
function GetDropByID(id) {
    return $find(id);
}
//ID TEXTBOX
function GetTextBoxByID(id) {
    return $find(id);
}
