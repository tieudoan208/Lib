//Chứa hàm thao tác lưới
/// <reference path="Common.js" />
/// <reference path="Until.js" />
/// <reference path="gridView.js" />
/// <reference path="MicrosoftAjax.js" />


//Lấy PageSize
function GetPageSize(gridID) {
    var grid = $find(gridID);
    return Common.toInt(grid.get_pageSize());
}

//Set trạng thái trang hiện tại đang hiện thị
function SetCurrentPage(gridID, crrPage) {
    $("#" + gridID + "_display").val(crrPage);
}

//lấy trang hiện tại
function GetCurrentPage(gridID) {
    var val = $("#" + gridID + "_display").val();
    return val;
}

//Set lại tổng số trang, đầu vào là tổng số dòng đang có
function SetTotalPage(gridID, totalRow) {
    var pageSize = GetPageSize(gridID);
    var pageCount = Common.round(Common.ToInt(totalRow) / Common.ToInt(pageSize) + .5, 0);
    if (pageCount != null)
        $("#" + gridID + "_total").text(pageCount);
}

//Lấy tổng số trang
function GetTotalPage(gridID) {
    var val = $("#" + gridID + "_total").text();
    return val;
}

//Lấy từ đến trong số trang
function GetObjectNumberPage(gridID) {
    var crrPage = GetCurrentPage(gridID), totalPage = GetTotalPage(gridID), pageSize = GetPageSize(gridID);
    var form = (crrPage - 1) * pageSize + 1, to = form + parseInt(pageSize) - 1;
    return [form, to];
}

//Lấy grid theo ID truyền vào
function GetGridByID(gridID) {
    if (gridID == "" || gridID == undefined)
        return Msg.show("Thông báo", "ID lưới không rỗng", "error");
    var grid = $find(gridID);
    return grid;
}
//Lấy cột đang Act
function GetColumnIndexActive(gridID) {
    var grid = GetGridByID(gridID);
    var cell = grid.getActiveCell().cell;
    return cell;
}

//Tính tổng một cột theo điều kiện
function SumValueByConditionColumn(gridID, colName, colCondition, valCondition) {
    var gridID = GetGridByID(gridID);
    var aData = gridID.getData();
    var sumVal = 0;
    colCondition = colCondition.toUpperCase();
    colName = colName.toUpperCase();
    for (var i = 0; i < aData.length; i++) {
        if (aData[i][colCondition] == valCondition) {
            sumVal = sumVal + Common.stringToNumber(aData[i][colName]);
        }
    }
    return sumVal;
}

//Trả về dữ liệu mạng Json trắng
function GetJsonData(cols, jsonData, pageSize) {
    var data = [];
    if (jsonData != "" && jsonData != undefined)
        data =Common.stringToJson(jsonData);
    if (data.length < pageSize) {
        for (var i = data.length; i < pageSize - data.length; i++) {
            var objData = [];
            for (var j = 0; j < cols.length; j++) {
                objData[cols[j].field] = "";
            }
            data.push(objData);
        }
    }
    return data;
}