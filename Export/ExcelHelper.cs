
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Linq;
using Lib.Utilities;

namespace Lib.Export
{
    class ExcelHelper
    {
        private const int rowLimit = 0xfde8;

        private static string getCell(Type type, object cellData)
        {
            object obj2 = (cellData is DBNull) ? "" : cellData;
            if ((type.Name.Contains("Int") || type.Name.Contains("Double")) || type.Name.Contains("Decimal"))
            {
                return string.Format("<Cell><Data ss:Type=\"Number\">{0}</Data></Cell>", obj2);
            }
            if (type.Name.Contains("Date") && (obj2.ToString() != string.Empty))
            {
                return string.Format("<Cell ss:StyleID=\"s63\"><Data ss:Type=\"DateTime\">{0}</Data></Cell>", Convert.ToDateTime(obj2).ToString("yyyy-MM-dd"));
            }
            return string.Format("<Cell><Data ss:Type=\"String\">{0}</Data></Cell>", replaceXmlChar(obj2.ToString()));
        }

        public static string GetExcelXml(DataSet dsInput, string filename)
        {
            string format = getWorkbookTemplate();
            string str2 = getWorksheets(dsInput);
            return string.Format(format, str2);
        }
        public static string GetExcelXml(DataTable dtInput, string filename, string b_truong)
        {
            string format = getWorkbookTemplate();
            DataSet source = new DataSet();
            source.Tables.Add(dtInput.Copy());
            string str2 = getWorksheets(dtInput, b_truong);
            return string.Format(format, str2);
        }
        public static string GetExcelXml(DataTable dtInput, string filename)
        {
            string format = getWorkbookTemplate();
            DataSet source = new DataSet();
            source.Tables.Add(dtInput.Copy());
            string str2 = getWorksheets(source);
            return string.Format(format, str2);
        }

        private static string getWorkbookTemplate()
        {
            StringBuilder builder = new StringBuilder(0x332);
            builder.AppendFormat("<?xml version=\"1.0\"?>{0}", Environment.NewLine);
            builder.AppendFormat("<?mso-application progid=\"Excel.Sheet\"?>{0}", Environment.NewLine);
            builder.AppendFormat("<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"{0}", Environment.NewLine);
            builder.AppendFormat(" xmlns:o=\"urn:schemas-microsoft-com:office:office\"{0}", Environment.NewLine);
            builder.AppendFormat(" xmlns:x=\"urn:schemas-microsoft-com:office:excel\"{0}", Environment.NewLine);
            builder.AppendFormat(" xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\"{0}", Environment.NewLine);
            builder.AppendFormat(" xmlns:html=\"http://www.w3.org/TR/REC-html40\">{0}", Environment.NewLine);
            builder.AppendFormat(" <Styles>{0}", Environment.NewLine);
            builder.AppendFormat("  <Style ss:ID=\"Default\" ss:Name=\"Normal\">{0}", Environment.NewLine);
            builder.AppendFormat("   <Alignment ss:Vertical=\"Bottom\"/>{0}", Environment.NewLine);
            builder.AppendFormat("   <Borders/>{0}", Environment.NewLine);
            builder.AppendFormat("   <Font ss:FontName=\"Calibri\" x:Family=\"Swiss\" ss:Size=\"11\" ss:Color=\"#000000\"/>{0}", Environment.NewLine);
            builder.AppendFormat("   <Interior/>{0}", Environment.NewLine);
            builder.AppendFormat("   <NumberFormat/>{0}", Environment.NewLine);
            builder.AppendFormat("   <Protection/>{0}", Environment.NewLine);
            builder.AppendFormat("  </Style>{0}", Environment.NewLine);
            builder.AppendFormat("  <Style ss:ID=\"s62\">{0}", Environment.NewLine);
            builder.AppendFormat("   <Font ss:FontName=\"Calibri\" x:Family=\"Swiss\" ss:Size=\"11\" ss:Color=\"#000000\"/>{0}", Environment.NewLine);
            builder.AppendFormat("   <Borders/>{0}", Environment.NewLine);
            builder.AppendFormat("  </Style>{0}", Environment.NewLine);

            builder.AppendFormat("  <Style ss:ID=\"m284322144\">{0}", Environment.NewLine);
            builder.AppendFormat("      <Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Center\" ss:WrapText=\"1\"/>{0}", Environment.NewLine);
            builder.AppendFormat("   <Font ss:FontName=\"Calibri\" x:Family=\"Swiss\" ss:Size=\"11\" ss:Color=\"#000000\"/>{0}", Environment.NewLine);
            builder.AppendFormat("      <Borders>{0}", Environment.NewLine);
            builder.AppendFormat("      <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>{0}", Environment.NewLine);
            builder.AppendFormat("      <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>{0}", Environment.NewLine);
            builder.AppendFormat("      <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>{0}", Environment.NewLine);
            builder.AppendFormat("      <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>{0}", Environment.NewLine);
            builder.AppendFormat("      </Borders>{0}", Environment.NewLine);
            builder.AppendFormat("  </Style>{0}", Environment.NewLine);

            builder.AppendFormat("  <Style ss:ID=\"s63\">{0}", Environment.NewLine);
            builder.AppendFormat("   <NumberFormat ss:Format=\"Short Date\"/>{0}", Environment.NewLine);

            builder.AppendFormat("      <Borders>{0}", Environment.NewLine);
            builder.AppendFormat("      <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>{0}", Environment.NewLine);
            builder.AppendFormat("      <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>{0}", Environment.NewLine);
            builder.AppendFormat("      <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>{0}", Environment.NewLine);
            builder.AppendFormat("      <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>{0}", Environment.NewLine);
            builder.AppendFormat("      </Borders>{0}", Environment.NewLine);
            builder.AppendFormat("  </Style>{0}", Environment.NewLine);
            builder.AppendFormat(" </Styles>{0}", Environment.NewLine);
            builder.Append(@"{0}\r\n</Workbook>");
            return builder.ToString();
        }

        private static string getWorksheets(DataSet source)
        {
            StringWriter writer = new StringWriter();
            if ((source == null) || (source.Tables.Count == 0))
            {
                writer.Write("<Worksheet ss:Name=\"Sheet1\">\r\n<Table>\r\n<Row><Cell><Data ss:Type=\"String\"></Data></Cell></Row>\r\n</Table>\r\n</Worksheet>");
                return writer.ToString();
            }
            foreach (DataTable table in source.Tables)
            {
                if (table.Rows.Count == 0)
                {
                    writer.Write("<Worksheet ss:Name=\"" + replaceXmlChar(table.TableName) + "\">\r\n<Table>\r\n<Row><Cell  ss:StyleID=\"s62\"><Data ss:Type=\"String\"></Data></Cell></Row>\r\n</Table>\r\n</Worksheet>");
                }
                else
                {
                    int num = 0;
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        if ((i % 0xfde8) == 0)
                        {
                            if ((i / 0xfde8) > num)
                            {
                                writer.Write("\r\n</Table>\r\n</Worksheet>");
                                num = i / 0xfde8;
                            }
                            writer.Write("\r\n<Worksheet ss:Name=\"" + replaceXmlChar(table.TableName) + (((i / 0xfde8) == 0) ? "" : Convert.ToString((int)(i / 0xfde8))) + "\">\r\n<Table>");
                            writer.Write("\r\n<Row>");
                            foreach (DataColumn column in table.Columns)
                            {
                                writer.Write(string.Format("<Cell ss:StyleID=\"s62\"><Data ss:Type=\"String\">{0}</Data></Cell>", replaceXmlChar(column.ColumnName)));
                            }
                            writer.Write("</Row>");
                        }
                        writer.Write("\r\n<Row>");
                        foreach (DataColumn column2 in table.Columns)
                        {
                            writer.Write(getCell(column2.DataType, table.Rows[i][column2.ColumnName]));
                        }
                        writer.Write("</Row>");
                    }
                    writer.Write("\r\n</Table>\r\n</Worksheet>");
                }
            }
            return writer.ToString();
        }
        /// <summary>
        ///Quannm: Đưa file excel theo group
        /// </summary>
        /// <param name="b_dt"></param>
        /// <param name="b_truong"></param>
        /// <returns></returns>
        private static string getWorksheets(DataTable b_dt, string b_truong)
        {
            var b_qr = from p in b_dt.AsEnumerable()
                       group p by new
                       {
                           ma_dt = p.Field<string>("ma_dt"),
                           ten_dt = p.Field<string>("ten_dt")
                       };
            StringWriter writer = new StringWriter();
            if ((b_dt == null) || (b_dt.Rows.Count == 0))
            {
                writer.Write("<Worksheet ss:Name=\"Sheet1\">\r\n<Table>\r\n<Row><Cell><Data ss:Type=\"String\"></Data></Cell></Row>\r\n</Table>\r\n</Worksheet>");
                return writer.ToString();
            }
            int num = 0;
            string b_gtri = string.Empty;
            bool b_okie = true;

            b_gtri = LibConvert.ObjectToString(b_dt.Rows[0]["ten_dvi"]);
            for (int i = 0; i < b_dt.Rows.Count; i++)
            {
                if ((i % 65000) == 0)
                {
                    if ((i / 65000) > num)
                    {
                        writer.Write("\r\n</Table>\r\n</Worksheet>");
                        num = i / 65000;
                    }
                    writer.Write("\r\n<Worksheet ss:Name=\"" + replaceXmlChar(b_dt.TableName) + (((i / 65000) == 0) ? "" : Convert.ToString((int)(i / 65000))) + "\">\r\n<Table>");

                    foreach (var items in b_qr)
                    {
                        writer.Write("<Column ss:StyleID=\"s62\" ss:AutoFitWidth=\"0\" ss:Width=\"100\" />");
                    }
                    writer.Write("\r\n<Row>");
                    //Lặp tiêu đề khi vượt quá 65000 dòng
                    writer.Write("<Cell ss:StyleID=\"m284322144\" ><Data ss:Type=\"String\">Tên đơn vị</Data></Cell>");
                    foreach (var items in b_qr)
                    {
                        writer.Write(string.Format("<Cell ss:StyleID=\"m284322144\" ><Data ss:Type=\"String\">{0}</Data></Cell>", MergeValue(items.Key.ten_dt, items.Key.ma_dt)));
                    }
                    writer.Write("</Row>");
                }
            //Ghi data
            tiep:
                if (b_okie)
                {
                    writer.Write("\r\n<Row>");
                    writer.Write(getCell(b_dt.Rows[i]["ten_dvi"].GetType(), b_dt.Rows[i]["ten_dvi"]));
                }
                if (b_gtri == LibConvert.ObjectToString(b_dt.Rows[i]["ten_dvi"]))
                {
                    if (b_qr.Select(o => o.Key.ma_dt == LibConvert.ObjectToString(b_dt.Rows[i]["ma_dt"])).Count() > 0)
                    {
                        b_okie = false;
                        writer.Write(getCell(b_dt.Rows[i][b_truong].GetType(), b_dt.Rows[i][b_truong]));
                    }
                }
                else
                {
                    b_gtri = LibConvert.ObjectToString(b_dt.Rows[i]["ten_dvi"]);
                    writer.Write("</Row>");
                    b_okie = true;
                    goto tiep;
                }
            }
            writer.Write("</Row>");
            writer.Write("\r\n</Table>\r\n</Worksheet>");
            return writer.ToString();
        }
        static string MergeValue(string value, string values1)
        {
            return value + " (" + values1 + ")";
        }
        private static string replaceXmlChar(string input)
        {
            input = input.Replace("&", "&amp");
            input = input.Replace("<", "&lt;");
            input = input.Replace(">", "&gt;");
            input = input.Replace("\"", "&quot;");
            input = input.Replace("'", "&apos;");
            return input;
        }
    }

}