using System;
using System.Data;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lib.Utilities
{
    /// <summary>
    /// Thao tác với table dữ liệu
    /// </summary>
    public class LibTable
    {
        public static int FindRowByColumn(DataTable table, string name, object value)
        {
            if (!isNullOrEmtyTable(table))
            {
                string str = LibConvert.ObjectToString(value);
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    if (!(table.Rows[i][name] is DBNull) && (LibConvert.ObjectToString(table.Rows[i][name]) == str))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
        public static bool isNullOrEmtyTable(DataTable table)
        {
            if ((table == null) || (table.Rows.Count == 0))
            {
                return true;
            }
            return false;
        }
        public static string TableToJson(DataTable dt)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row = null;

            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName.Trim().ToUpper(), dr[col]);
                }
                rows.Add(row);
            }
            return serializer.Serialize(rows);
        }

        /// <summary>
        /// Chuyển định dạng bảng sang định dạng chuỗi Json
        /// </summary>
        /// <param name="dt">Bảng cần chuyển</param>
        /// <param name="a_cot">Cột cần chuỷen</param>
        /// <returns></returns>
        public static string TableToJson(DataTable dt, string[] a_cot)
        {
            if ((a_cot == null) || (a_cot.Length == 0))
            {
                return TableToJson(dt);
            }

            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row = null;

            DataColumnCollection columns = dt.Columns;

            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                for (int i = 0; i < a_cot.Length; i++)
                {
                    if (columns.Contains(a_cot[i]))
                    {
                        row.Add(a_cot[i].Trim().ToUpper(), dr[a_cot[i]]);
                    }
                }
                rows.Add(row);
            }
            return serializer.Serialize(rows);
        }


        public static string TableToJson(DataTable dt, int indexRow)
        {
            if (isNullOrEmtyTable(dt)) return "";
            if (indexRow >= dt.Rows.Count) throw new Exception("Dòng vượt quá tổng số dòng");
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row = new Dictionary<string, object>();
            DataRow dr = dt.Rows[indexRow];
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                row.Add(dt.Columns[i].ColumnName.Trim().ToUpper(), dr[dt.Columns[i].ColumnName]);
            }
            rows.Add(row);
            return serializer.Serialize(rows);
        }
        public static DataTable JsonStringToDataTable(string jsonString)
        {
            DataTable dt = new DataTable();
            string[] jsonStringArray = Regex.Split(jsonString.Replace("[", "").Replace("]", ""), "},{");
            List<string> ColumnsName = new List<string>();
            foreach (string jSA in jsonStringArray)
            {
                string[] jsonStringData = Regex.Split(jSA.Replace("{", "").Replace("}", ""), ",");
                foreach (string ColumnsNameData in jsonStringData)
                {
                    try
                    {
                        int idx = ColumnsNameData.IndexOf(":");
                        string ColumnsNameString = ColumnsNameData.Substring(0, idx - 1).Replace("\"", "");
                        if (!ColumnsName.Contains(ColumnsNameString))
                        {
                            ColumnsName.Add(ColumnsNameString);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Error Parsing Column Name : {0}", ColumnsNameData));
                    }
                }
                break;
            }
            foreach (string AddColumnName in ColumnsName)
            {
                dt.Columns.Add(AddColumnName);
            }
            foreach (string jSA in jsonStringArray)
            {
                string[] RowData = Regex.Split(jSA.Replace("{", "").Replace("}", ""), ",");
                DataRow nr = dt.NewRow();
                foreach (string rowData in RowData)
                {
                    try
                    {
                        int idx = rowData.IndexOf(":");
                        string RowColumns = rowData.Substring(0, idx - 1).Replace("\"", "");
                        string RowDataString = rowData.Substring(idx + 1).Replace("\"", "");
                        nr[RowColumns] = RowDataString;
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
                dt.Rows.Add(nr);
            }
            return dt;
        }
        /// <summary>
        /// Tìm dòng có cột bằng với giá trị truyền
        /// </summary>
        /// <param name="table">Bảng cần tìm</param>
        /// <param name="name">Tên cột</param>
        /// <param name="val">Giá trị truyền</param>
        public static int FindIndexRow(DataTable table, string name, object val)
        {
            if (!isNullOrEmtyTable(table))
            {
                string str = LibConvert.ObjectToString(val);
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    if (!(table.Rows[i][name] is DBNull) && (LibConvert.ObjectToString(table.Rows[i][name]) == str))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
        /// <summary>
        /// Tìm vị trí chỉ số cột 
        /// </summary>
        /// <param name="table">bảng chứa cột cần tìm</param>
        /// <param name="ColName">Tên cột</param>
        public static int FindIndexColumn(DataTable table, string ColName)
        {
            if (isNullOrEmtyTable(table))
                return -1;
            for (int i = 0; i < table.Columns.Count; i++)
            {
                if (table.Columns[i].ColumnName.ToUpper() == ColName.ToUpper())
                    return i;
            }
            return -1;
        }
        #region ADD COLS
        /// <summary>
        /// Thêm một cột mới vào cuối, mặc định tên cột tự động viết hoa
        /// </summary>
        /// <param name="b_dt">Tên bảng cần thêm cột</param>
        /// <param name="nameCol">Tên cột muốn thêm</param>
        public static void AddNewColumns(ref DataTable b_dt, string nameCol)
        {
            if (b_dt == null)
                b_dt = new DataTable();
            b_dt.Columns.Add(nameCol);
        }

        /// <summary>
        /// Thêm danh sách cột dạng mảng, mặc định viết hoa và chèn theo thứ tự vào cuối
        /// </summary>
        /// <param name="table">Table cần thêm</param>
        /// <param name="nameCol">Mảng cột cần thêm</param>
        public static void AddNewColumns(ref DataTable table, string[] nameCol)
        {
            if (table == null)
                table = new DataTable();

            for (int i = 0; i < nameCol.Length; i++)
                AddNewColumns(ref table, nameCol[i]);

            table.AcceptChanges();
        }
        /// <summary>
        /// Thêm danh sách các cột vào bảng
        /// </summary>
        /// <param name="table">Bảng cần thêm</param>
        /// <param name="nameCol"></param>
        /// <param name="typeCol"></param>
        public static void AddNewColumns(ref DataTable table, string[] nameCol, Type[] typeCol)
        {
            if (table == null)
                table = new DataTable();

            for (int i = 0; i < nameCol.Length; i++)
                AddNewColumns(ref table, nameCol[i], typeCol[i]);

            table.AcceptChanges();
        }
        /// <summary>
        /// Thêm cột vào bảng, có định nghĩa kiểu dữ liệu của cột
        /// </summary>
        /// <param name="b_dt">Bảng cần thêm cột</param>
        /// <param name="nameCol">Tên cột</param>
        /// <param name="typeCol">Kiểu số liệu</param>
        public static void AddNewColumns(ref DataTable b_dt, string nameCol, Type typeCol)
        {
            if (b_dt == null)
                b_dt = new DataTable();
            b_dt.Columns.Add(nameCol, typeCol);
        }
        /// <summary>
        /// Tạo bảng với dữ liệu cho sẵn ( tên và giá trị)
        /// </summary>
        /// <param name="nameCol">Mảng cot</param>
        /// <param name="val">Mảng giá trị</param>
        /// <returns></returns>
        public static DataTable CreateTable(string[] nameCol, object[] val)
        {
            DataTable table = new DataTable();
            for (int i = 0; i < nameCol.Length; i++)
            {
                table.Columns.Add(nameCol[i]);
            }
            AddNewRows(ref table, val);
            return table;
        }


        #endregion

        #region REMOVE COL

        /// <summary>
        /// Xóa một cột theo tên
        /// </summary>
        /// <param name="table">Bảng chưa cột cần xóa</param>
        /// <param name="ColName">Tên cột cần xóa</param>
        public static void RemoveColumns(ref DataTable table, string ColName)
        {
            int indexCol = FindIndexColumn(table, ColName);
            if (indexCol == -1)
                return;
            table.Columns.RemoveAt(indexCol);
        }
        /// <summary>
        /// Thêm 1 cột vào Table từ 1 giá trị String
        /// </summary>
        /// <param name="b_dt">Bảng cần thêm</param>
        /// <param name="b_truong">Tên trường cần thêm</param>
        /// <param name="b_gtri">Giá trị trường cần thêm</param>
        public static void AddNewColumns(ref DataTable b_dt, string nameCol, string val)
        { // Dan
            b_dt.Columns.Add(nameCol, val.GetType());
            for (int i = 0; i < b_dt.Rows.Count; i++) b_dt.Rows[i][nameCol] = val;
            b_dt.AcceptChanges();
        }
        /// <summary>
        /// Xóa danh sách cột theo mảng truyền vào
        /// </summary>
        /// <param name="table">Bảng chưa danh sách cột</param>
        /// <param name="ColsName">Mảng tham số</param>
        public static void RemoveColumns(ref DataTable table, string[] ColName)
        {
            for (int i = 0; i < ColName.Length; i++)
                RemoveColumns(ref table, ColName[i]);
        }
        #endregion

        #region REMOVE ROW

        /// <summary>
        /// Xóa bỏ một cột theo chỉ số ROW đã đưa sẵn
        /// </summary>
        /// <param name="table">Table có dòng cần xóa</param>
        /// <param name="index">Vị trí cần xóa</param>
        public static void RemoveRows(ref DataTable table, int index)
        {
            if (isNullOrEmtyTable(table))
                return;
            if (index < table.Rows.Count)
                table.Rows.RemoveAt(index);
            table.AcceptChanges();
        }

        public static void RemoveRows(ref DataTable table, string colName, object val)
        {
            if (isNullOrEmtyTable(table))
                return;
            int rowLength = table.Rows.Count - 1;
            for (int i = rowLength; i >= 0; i--)
            {
                if (table.Rows[i][colName] is DBNull || table.Rows[i][colName] == val)
                    table.Rows.RemoveAt(i);
            }
            table.AcceptChanges();
        }
        #endregion

        #region ADD ROW

        /// <summary>
        /// Thêm một dong
        /// </summary>
        /// <param name="b_dt"></param>
        /// <param name="val"></param>
        public static void AddNewRows(ref DataTable b_dt, object val)
        {
            DataRow row = b_dt.NewRow();
            row[0] = val;
            b_dt.Rows.Add(row);
            b_dt.AcceptChanges();
        }
        public static void AddNewRows(ref DataTable b_dt, object[] val)
        {
            DataRow row = b_dt.NewRow();
            for (int i = 0; i < val.Length; i++)
            {
                row[i] = val[i];
            }
            b_dt.Rows.Add(row);
            b_dt.AcceptChanges();
        }

        /// <summary>
        /// Thêm dòng theo cột
        /// </summary>
        /// <param name="table">Bảng cần thêm</param>
        /// <param name="colName">Tên cột</param>
        /// <param name="value">Mảng giá trị cần thêm</param>
        public static void AddNewRows(ref DataTable table, string colName, object[] value)
        {
            DataRow _row;
            for (int i = 0; i < value.Length; i++)
            {
                _row = table.NewRow();
                _row[colName] = value[i];
                table.Rows.Add(_row);
            }
            table.AcceptChanges();
        }
        #endregion
    }
}
