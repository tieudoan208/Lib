using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Xml;
using System.Xml.Linq;
using System.Data.OleDb;

namespace Lib.Export
{
    public class Excel
    {
        public static void ExportToExcel(string templateUrl, ref string outUrl, string template, DataSet data)
        {
            string filename = HttpContext.Current.Server.MapPath(templateUrl + template + ".xml");
            outUrl = outUrl + template + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".xls";
            string str2 = HttpContext.Current.Server.MapPath(outUrl);
            XmlDocument document = new XmlDocument();
            document.Load(filename);
            CExcelMLFiller filler = new CExcelMLFiller(data, document.OuterXml);
            if (!filler.OperationFailed)
            {
                filler.Transform();
                if (!filler.OperationFailed)
                {
                    goto Label_00FC;
                }
                IEnumerator enumerator = filler.ErrorList.GetEnumerator();
                {
                    while (enumerator.MoveNext())
                    {
                        string current = (string)enumerator.Current;
                        return;
                    }
                    goto Label_00FC;
                }
            }
            IEnumerator enumerator2 = filler.ErrorList.GetEnumerator();
            {
                while (enumerator2.MoveNext())
                {
                    string text2 = (string)enumerator2.Current;
                    return;
                }
            }
        Label_00FC:
            filler.ExcelMLDocument.Save(str2);
        }
        public static void ExportToExcel(DataTable dsInput, string filename, HttpResponse response, string b_loai)
        {
            string excelXml = ExcelHelper.GetExcelXml(dsInput, filename, b_loai);
            response.Clear();
            response.AppendHeader("Content-Type", "application/vnd.ms-excel");
            response.AppendHeader("Content-disposition", "attachment;filename=" + filename);
            response.Write(excelXml);
            response.Flush();
            response.End();
        }

        public static void ToExcel(DataSet dsInput, string filename, HttpResponse response)
        {
            string excelXml = ExcelHelper.GetExcelXml(dsInput, filename);
            response.Clear();
            response.AppendHeader("Content-Type", "application/vnd.ms-excel");
            response.AppendHeader("Content-disposition", "attachment;filename=" + filename);
            response.Write(excelXml);
            response.Flush();
            response.End();
        }
        public static void ToExcel(DataTable dtInput, string filename, HttpResponse response)
        {
            DataSet dsInput = new DataSet();
            dsInput.Tables.Add(dtInput.Copy());
            ToExcel(dsInput, filename, response);
        }
        public static DataTable ReadExcelData(string path, string[] param)
        {
            OleDbConnection oledbConn = null;
            try
            {
                if (Path.GetExtension(path) == ".xls")
                {
                    oledbConn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"");
                }
                else if (Path.GetExtension(path) == ".xlsx")
                {
                    oledbConn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';");
                }
                if (oledbConn == null) return null;
                oledbConn.Open();
                OleDbCommand cmd = new OleDbCommand();
                OleDbDataAdapter oleda = new OleDbDataAdapter();
                DataSet ds = new DataSet();

                cmd.Connection = oledbConn;
                cmd.CommandType = CommandType.Text;
                string a = "";
                if (param.Length > 0)
                {
                    for (int i = 0; i < param.Length; i++)
                    {
                        a += "[" + param[i] + "]" + ",";
                    }
                    a = a.Remove(a.Length - 1);
                }
                else
                    a = " * ";

                cmd.CommandText = "SELECT " + a + " FROM [Sheet1$]";
                oleda = new OleDbDataAdapter(cmd);
                oleda.Fill(ds);
                return ds.Tables[0];

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                oledbConn.Close();
            }
        }
    }
}
