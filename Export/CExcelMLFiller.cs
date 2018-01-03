using System;
using System.Data;
using System.Collections;
using System.Xml;
using System.Globalization;

namespace Lib.Export
{
    /// <summary>
    /// Summary description for CExcelMLFiller.
    /// </summary>
    public class CExcelMLFiller
    {
        private DataSet dsData;
        private string templateContent;
        private XmlDocument xmlTemplateDoc;
        //private XmlDocument xmlDataset;
        private XmlNamespaceManager nsmgr = new XmlNamespaceManager(new NameTable());
        private ArrayList errorList = new ArrayList();
        private bool bOperationFailed = false;
        private static string excelmlPrefix = "urn:schemas-microsoft-com:office:excel";
        private static string spreadsheetPrefix = "urn:schemas-microsoft-com:office:spreadsheet";
        private const string repeatAttribute = "SMLRepeat";
        private XmlNodeList templateRowsColl;
        private XmlNode tableNode;

        private string languageName;
        private CultureInfo ci;
        private string dateTimeFormat;
        private NumberFormatInfo numberFormat;
        private string numberDecimalSeparator;
        private string numberGroupSeparator;
        private int numberDecimalDigits;

        public CExcelMLFiller(DataSet dsData, string templateContent)
        {
            this.dsData = dsData;
            this.templateContent = templateContent;
            LoadTemplate();
        }

        public ArrayList ErrorList
        {
            get
            {
                return errorList;
            }
        }

        public bool OperationFailed
        {
            get
            {
                return bOperationFailed;
            }
        }

        public XmlDocument ExcelMLDocument
        {
            get
            {
                return xmlTemplateDoc;
            }
        }

        private void LoadTemplate()
        {
            errorList = new ArrayList();
            bOperationFailed = false;
            try
            {
                xmlTemplateDoc = new XmlDocument();
                xmlTemplateDoc.LoadXml(this.templateContent);
                //nsmgr.AddNamespace(string.Empty, spreadsheetPrefix);
                nsmgr.AddNamespace("x", excelmlPrefix);
                nsmgr.AddNamespace("ss", spreadsheetPrefix);

                XmlNode varNode = xmlTemplateDoc.SelectSingleNode("/ss:Workbook/ss:Names/ss:NamedRange[@ss:Name='LanguageName']/@ss:RefersTo", nsmgr);
                if (varNode != null)
                {
                    languageName = varNode.Value;
                    languageName = languageName.Replace("=", string.Empty);
                    languageName = languageName.Replace("\"", string.Empty);
                    try
                    {
                        ci = new CultureInfo(languageName, false);
                        numberFormat = ci.NumberFormat;
                    }
                    catch
                    {
                        languageName = null;
                        numberFormat = null;
                    }
                }
                else
                {
                    languageName = null;
                }
                varNode = xmlTemplateDoc.SelectSingleNode("/ss:Workbook/ss:Names/ss:NamedRange[@ss:Name='DateTimeFormat']/@ss:RefersTo", nsmgr);
                if (varNode != null)
                {
                    dateTimeFormat = varNode.Value;
                    dateTimeFormat = dateTimeFormat.Replace("=", string.Empty);
                    dateTimeFormat = dateTimeFormat.Replace("\"", string.Empty);
                }
                else
                {
                    dateTimeFormat = null;
                }
                varNode = xmlTemplateDoc.SelectSingleNode("/ss:Workbook/ss:Names/ss:NamedRange[@ss:Name='NumberDecimalSeparator']/@ss:RefersTo", nsmgr);
                if (varNode != null)
                {
                    numberDecimalSeparator = varNode.Value;
                    numberDecimalSeparator = numberDecimalSeparator.Replace("=", string.Empty);
                    numberDecimalSeparator = numberDecimalSeparator.Replace("\"", string.Empty);
                    if (numberFormat == null) numberFormat = new NumberFormatInfo();
                    numberFormat.NumberDecimalSeparator = numberDecimalSeparator.Trim() == string.Empty ? "." : numberDecimalSeparator;
                }
                else
                {
                    numberDecimalSeparator = null;
                }
                varNode = xmlTemplateDoc.SelectSingleNode("/ss:Workbook/ss:Names/ss:NamedRange[@ss:Name='NumberGroupSeparator']/@ss:RefersTo", nsmgr);
                if (varNode != null)
                {
                    numberGroupSeparator = varNode.Value;
                    numberGroupSeparator = numberGroupSeparator.Replace("=", string.Empty);
                    numberGroupSeparator = numberGroupSeparator.Replace("\"", string.Empty);
                    if (numberFormat == null) numberFormat = new NumberFormatInfo();
                    numberFormat.NumberGroupSeparator = numberGroupSeparator.Trim() == string.Empty ? "," : numberGroupSeparator;
                }
                else
                {
                    numberGroupSeparator = null;
                }
                varNode = xmlTemplateDoc.SelectSingleNode("/ss:Workbook/ss:Names/ss:NamedRange[@ss:Name='NumberDecimalDigits']/@ss:RefersTo", nsmgr);
                if (varNode != null)
                {
                    string sNumberDecimalDigits = varNode.Value;
                    sNumberDecimalDigits = sNumberDecimalDigits.Replace("=", string.Empty);
                    sNumberDecimalDigits = sNumberDecimalDigits.Replace("\"", string.Empty);
                    try
                    {
                        numberDecimalDigits = int.Parse(sNumberDecimalDigits);
                    }
                    catch
                    {
                        numberDecimalDigits = -1;
                    }
                    if (numberFormat == null) numberFormat = new NumberFormatInfo();
                    numberFormat.NumberDecimalDigits = numberDecimalDigits == -1 ? 2 : numberDecimalDigits;
                }
                else
                {
                    numberDecimalDigits = -1;
                }

            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    errorList.Add(ex.Message);
                    ex = ex.InnerException;
                }
                bOperationFailed = true;
            }
        }

        public void Transform()
        {
            try
            {
                tableNode = xmlTemplateDoc.SelectSingleNode("/ss:Workbook/ss:Worksheet/ss:Table", nsmgr);
                templateRowsColl = xmlTemplateDoc.SelectNodes("/ss:Workbook/ss:Worksheet/ss:Table/ss:Row", nsmgr);
                foreach (DataTable dt in dsData.Tables)
                {
                    TransformTemplateRows(dt);
                }
                int i = templateRowsColl.Count - 1;
                while (i >= 0)
                {
                    XmlNode repeatNode = templateRowsColl[i].SelectSingleNode("ss:Cell[contains(@ss:Formula, '" + repeatAttribute + "')]", nsmgr);
                    bool bIsRepeat = repeatNode != null;
                    if (bIsRepeat)
                    {
                        tableNode.RemoveChild(templateRowsColl[i]);
                    }
                    i--;
                }
                ((XmlElement)tableNode).RemoveAttribute("ss:ExpandedRowCount");
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    errorList.Add(ex.Message);
                    ex = ex.InnerException;
                }
                bOperationFailed = true;
            }
        }

        private void TransformTemplateRows(DataTable dt)
        {
            foreach (XmlNode rowNode in templateRowsColl)
            {
                bool bIsRepeat;
                XmlNode repeatNode = rowNode.SelectSingleNode("ss:Cell[contains(@ss:Formula, '=" + dt.TableName + repeatAttribute + "')]", nsmgr);
                bIsRepeat = repeatNode != null;
                string templateFieldName;
                if (bIsRepeat)
                {
                    tableNode = rowNode.ParentNode;
                    foreach (DataRow dr in dt.Rows)
                    {
                        XmlNode newRowNode = rowNode.Clone();
                        ((XmlElement)newRowNode).RemoveAttribute("ss:Index");
                        repeatNode = newRowNode.SelectSingleNode("ss:Cell[contains(@ss:Formula, '=" + dt.TableName + repeatAttribute + "')]", nsmgr);
                        ((XmlElement)repeatNode).RemoveAttribute("ss:Formula");
                        repeatNode.FirstChild.InnerText = string.Empty;
                        tableNode.InsertBefore(newRowNode, rowNode);
                        for (int i = 0; i < dr.ItemArray.Length; i++)
                        {
                            templateFieldName = dt.TableName + dt.Columns[i].ColumnName;
                            ReplaceFieldData(newRowNode, templateFieldName, dr[i].ToString(), dt.Columns[i].DataType);
                        }
                    }
                }
                else
                {
                    if (dt.Rows.Count > 0)
                    {
                        DataRow firstRow = dt.Rows[0];
                        for (int i = 0; i < firstRow.ItemArray.Length; i++)
                        {
                            templateFieldName = dt.TableName + dt.Columns[i].ColumnName;
                            ReplaceFieldData(rowNode, templateFieldName, firstRow[i].ToString(), dt.Columns[i].DataType);
                        }
                    }
                }
            }
        }

        private void ReplaceFieldData(XmlNode baseNode, string fieldName, string data, Type colType)
        {
            errorList = new ArrayList();
            bOperationFailed = false;
            XmlNode dataNode;
            XmlNodeList oColl;
            oColl = baseNode.SelectNodes("ss:Cell[@ss:Formula='=" + fieldName + "']", nsmgr);
            foreach (XmlNode fieldNode in oColl)
            {
                dataNode = fieldNode.SelectSingleNode("ss:Data", nsmgr);
                if (dataNode == null)
                {
                    errorList.Add("The field data is selected from the fields definition data source or merge document is corrupted!");
                    bOperationFailed = true;
                    return;
                }
                ((XmlElement)fieldNode).RemoveAttribute("ss:Formula");
                if (colType == typeof(DateTime))
                {
                    if (dateTimeFormat != null)
                    {
                        DateTime dt = DateTime.Parse(data);
                        dataNode.InnerText = dt.ToString(dateTimeFormat);
                    }
                    else
                    {
                        dataNode.InnerText = data;
                    }
                    //((XmlElement)dataNode).SetAttribute("ss:Type", "DateTime");
                }
                else if (colType == typeof(int)
                    || colType == typeof(short)
                    || colType == typeof(long)
                    )
                {
                    if (numberFormat != null)
                    {
                        int i = int.Parse(data);
                        dataNode.InnerText = i.ToString(numberFormat);
                    }
                    else
                    {
                        dataNode.InnerText = data;
                    }
                    ((XmlElement)dataNode).SetAttribute("ss:Type", "Number");
                }
                else if (colType == typeof(decimal)
                    || colType == typeof(float)
                    || colType == typeof(double)
                    )
                {
                    if (numberFormat != null)
                    {
                        decimal d = decimal.Parse(data);
                        dataNode.InnerText = d.ToString("N", numberFormat);
                    }
                    else
                    {

                        dataNode.InnerText = data;
                    }
                    if (data == "")
                        ((XmlElement)dataNode).SetAttribute("ss:Type", "String");
                    else
                        ((XmlElement)dataNode).SetAttribute("ss:Type", "Number");
                }
                else
                {
                    dataNode.InnerText = data;
                    ((XmlElement)dataNode).SetAttribute("ss:Type", "String");
                }
            }
        }


    }
}
