
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web;

namespace Lib.Utilities
{
    public class LibForm
    {
        /// <summary>
        /// Lọc thông tin lỗi có cấu trúc dạng loi: Nội dung lỗi :loi
        /// </summary>
        /// <param name="error">Nọi dung chuỗi cần lặp</param>
        public static string GetError(string error)
        {

            error = Until.NVL(error);
            if (error != "")
            {
                error = error.Replace("\r", "").Replace("\n", "").Replace("'", "");
                int index = error.IndexOf("loi:");
                int num2 = error.IndexOf(":loi");
                if ((index != -1) && (num2 > (index + 4)))
                {
                    error = error.Substring(index + 4, (num2 - index) - 4);
                }
                error = "loi:" + error + ":loi";
            }
            return error;
        }
        /// <summary>
        /// Kiểm tra xem tên controk trong mot vùng control nào đó có phải là control hay không
        /// </summary>
        /// <param name="form">Vùng có chưa control</param>
        /// <param name="nameOfControl">Tên control cần kiểm tra</param>
        public static bool IsControl(Control form, string nameOfControl)
        {
            nameOfControl = Until.NVL(nameOfControl).ToUpper();
            if ((nameOfControl != "") && (Until.NVL(form.ID).ToUpper() != nameOfControl))
            {
                for (int i = 0; i < form.Controls.Count; i++)
                {
                    Control control = form.Controls[i];
                    if (Until.NVL(control.ID).ToUpper() == nameOfControl)
                    {
                        return true;
                    }
                    if ((control.Controls.Count > 0) && IsControl(control, nameOfControl))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Tìm control trong một vùng cho trước
        /// </summary>
        /// <param name="form">Vùng chứa control cần tìm</param>
        /// <param name="nameOfControl">Tên control cần tìm</param>
        public static Control FindControl(Control form, string nameOfControl)
        {
            if (form != null)
            {
                nameOfControl = Until.NVL(nameOfControl).ToUpper();
                if ((nameOfControl == "") || (Until.NVL(form.ID).ToUpper() == nameOfControl))
                {
                    return null;
                }
                for (int i = 0; i < form.Controls.Count; i++)
                {
                    Control control = form.Controls[i];
                    if (Until.NVL(control.ID).ToUpper() == nameOfControl)
                    {
                        return control;
                    }
                    if (control.Controls.Count != 0)
                    {
                        Control control2 = FindControl(control, nameOfControl);
                        if (control2 != null)
                        {
                            return control2;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Đưa giá trị từ bản vào form, tên cột trùng tên control tương ứng
        /// </summary>
        /// <param name="ctr">Vùng control cần đưa dữ liệu</param>
        /// <param name="table">Bảng cần dưa dữ liệu</param>
        public static void SetValueForm(Control ctr, DataTable table)
        {
            for (int i = 0; i < table.Columns.Count; i++)
            {
                Control _ctr = FindControl(ctr, table.Columns[i].ColumnName);
                if (_ctr != null)
                {
                    if (_ctr is Lib.Controls.textBox)
                    {
                        Lib.Controls.textBox txt = (Lib.Controls.textBox)_ctr;
                        txt.Text = LibConvert.ObjectToString(table.Rows[0][i]);
                    }
                    else if (_ctr is Lib.Controls.drop)
                        ((Lib.Controls.drop)_ctr).SelectedValue = LibConvert.ObjectToString(table.Rows[0][i]);
                    else if (_ctr is Lib.Controls.checkBox)
                        ((Lib.Controls.checkBox)_ctr).Checked = LibConvert.ObjectToBool(table.Rows[0][i]);
                    else if(_ctr is HiddenField)
                        ((HiddenField)_ctr).Value = LibConvert.ObjectToString(table.Rows[0][i]);
                }
            }
        }
        /// <summary>
        /// Hiện thị thông báo
        /// </summary>
        /// <param name="formPage">Page</param>
        /// <param name="error">Tham số cần hiện thị</param>
        public static void Msg(Page formPage, string error)
        {
            error = GetError(error);
            if (error != "")
            {
                formPage.ClientScript.RegisterClientScriptBlock(formPage.GetType(), "Display Error", "alert('" + error + "');", true);
            }
        }

        public static void Msg(Page formPage, string nameErr, string error)
        {
            error = GetError(error);
            if (error != "")
            {
                if (nameErr != "")
                {
                    error = error + " " + nameErr;
                }
                formPage.ClientScript.RegisterClientScriptBlock(formPage.GetType(), "Cảnh báo Error", "alert('" + error + "');", true);
            }
        }
    }
}

