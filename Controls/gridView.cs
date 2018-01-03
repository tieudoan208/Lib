using System;
using System.Web;
using System.Data;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Lib.Utilities;
using Lib.Constant;

namespace Lib.Controls
{
    [ParseChildren(true)]
    public class gridView : WebControl, IScriptControl
    {
        public gridView() : base(HtmlTextWriterTag.Div) { }

        #region PROPERTIES

        private ScriptManager sm;
        private List<Column> _Columns;
        private DataTable _DataSource;
        private List<Pager> _Pager;
        private bool _hidenHeader;
        private bool _hiddenRowHeader;


        [Description("DataSource"), Bindable(true), Category("Option")]
        public DataTable DataSource
        {
            get { return _DataSource; }
            set { _DataSource = value; }
        }


        [Description("Sự kiện xảy ra khi người sử dụng chọn 1 cell trên lưới"), Bindable(true), Category("Option")]
        public string onClientCellClick
        {
            get;
            set;
        }

        [Description("Sự kiện xảy ra khi người sử dụng update 1 cell trên lưới"), Bindable(true), Category("Option")]
        public string onClientUpdateCell
        {
            get;
            set;
        }
        [Description("Sự kiện xảy ra khi người sử dụng edit trên lưới"), Bindable(true), Category("Option")]
        public string onClientBeforEdit
        {
            get;
            set;
        }
        [Description("Sự kiện xảy ra khi người sử dụng doubclik trên lưới"), Bindable(true), Category("Option")]
        public string onClientDbClick
        {
            get;
            set;
        }
        [Description("Ẩn Header"), Bindable(true), Category("Option")]
        public bool HidenHeader
        {
            get { return _hidenHeader; }
            set
            {
                _hidenHeader = value;
            }
        }

        public bool HidenRowHeader
        {
            get { return _hiddenRowHeader; }
            set
            {
                _hiddenRowHeader = value;
            }
        }
        [Description("Kiểu phân trang trên lưới"), Bindable(true), Category("Option")]
        public TypePaging TypePaging
        {
            get;
            set;
        }


        [PersistenceMode(PersistenceMode.InnerProperty)]
        public List<Column> Columns
        {
            get
            {
                if (this._Columns == null)
                    this._Columns = new List<Column>();
                return this._Columns;
            }
        }
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public List<Pager> Pager
        {
            get
            {
                if (this._Pager == null)
                    this._Pager = new List<Pager>();
                return this._Pager;
            }
        }

        public int RowCount
        {
            get;
            set;
        }

        #endregion

        #region Method

        public int GetPageSize()
        {
            if (this._Pager == null || this._Pager.Count == 0)
                return 0;
            else
                return this._Pager[0].PageSize;
        }
        public int FindIndexColumn(string ColName)
        {
            int Locate = -1;
            for (int i = 0; i < Columns.Count; i++)
            {
                if (Columns[i].BaseColumn.ToUpper() == ColName.ToUpper())
                    return i;
            }
            return Locate;
        }

        #endregion

        #region RENDER
        protected override void OnPreRender(EventArgs e)
        {
            if (!this.DesignMode)
            {
                sm = ScriptManager.GetCurrent(this.Page);
                if (sm == null) throw new HttpException("Cần khai báo thêm Scriptmanager vào control");
                sm.RegisterScriptControl(this);
            }
            base.OnPreRender(e);
        }
        protected override void Render(HtmlTextWriter writer)
        {
            if (!this.DesignMode)
                sm.RegisterScriptDescriptors(this);
            base.Render(writer);
            this.Style.Add("width", this.Width.ToString());
            this.Style.Add("height", this.Height.ToString());
            if (this.Pager.Count > 0)
            {
                if (this.TypePaging == Constant.TypePaging.Client)
                    return;
                Pager _page = this.Pager[0];
                string onClientPaging = _page.OnPaging;

                writer.Write("<div id=\"" + this.ClientID + "_toolbar\"  class=\"toolbar\" style=\"width: " + this.Width + "; height: 30px; \">");
                writer.Write("<div class=\"toolbar-inside\">");
                writer.Write("<div class=\"class-view\">");
                writer.Write("<div id=\"" + this.ClientID + "_first\" class=\"page fist-page-disable\"></div></div>");
                writer.Write("<div class=\"class-view\">");
                writer.Write("<div id=\"" + this.ClientID + "_prev\" class=\"page page-prev-disabled\"></div></div>");
                writer.Write(" <div class=\"class-view\" style=\"width: 50px; margin-top: 0px !important;\"> ");
                writer.Write(" <input type=\"text\" style=\"width: 45px\" id=\"" + this.ClientID + "_display\" value=\"1\"  /></div>");
                writer.Write("<div class=\"class-view\" style=\"width: 5px !important; font-size: larger; margin-left:5px;\">/</div>");
                writer.Write("<div class=\"class-view\" style=\"width: 25px !important; font-size: larger; margin-left:5px;\">");
                writer.Write("<p id=\"" + this.ClientID + "_total\">" + Math.Round(LibConvert.ObjectToDouble(RowCount) / LibConvert.ObjectToDouble(_page.PageSize) + 0.5).ToString() + "</p></div>");
                writer.Write("<div class=\"class-view\">");
                writer.Write("<div id=\"" + this.ClientID + "_next\" class=\"page page-next\"></div></div>");
                writer.Write("<div class=\"class-view\">");
                writer.Write("<div id=\"" + this.ClientID + "_last\" class=\"page page-last\"></div></div>");
                writer.Write("</div>");
                writer.Write("</div>");
            }
        }

        #endregion

        #region Register Script
        protected virtual IEnumerable<ScriptReference> GetScriptReferences()
        {
            return new ScriptReference[] {
                new ScriptReference("Lib.Resources.gridView.js", "Lib")
            };
        }

        protected virtual IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            string colName = "";
            if (this._Columns != null)
            {
                if (_hiddenRowHeader)
                {
                    colName = colName + "{\"name\": \"\",";
                    colName = colName + "\"id\": \"#\",";
                    colName = colName + "\"cssClass\": \"cell-reorder dnd\",";
                    colName = colName + "\"width\": 40},";
                }
                foreach (var r in this._Columns)
                {
                    colName = colName + "{ \"field\": \"" + r.BaseColumn.ToUpper() + "\",";
                    colName = colName + "\"name\": \"" + r.Name + "\",";
                    colName = colName + "\"id\": \"" + r.BaseColumn.ToLower() + "\",";
                    colName = colName + "\"source\":  \"" + r.Source + "\",";
                    colName = colName + "\"width\": \"" + r.Width + "\",";
                    colName = colName + "\"hidden\": \"" + r.Hidden + "\"";
                    if (Until.NVL(r.CssClass) != "")
                        colName = colName + ",\"cssClass\": \"" + r.CssClass + "\"";
                    if (r.EditControlType != EditorType.None)
                    {
                        if(r.EditControlType==EditorType.DropGrid)
                        {
                            string colTemp = "";
                            foreach(var temp in r.ItemTemplate)
                            {
                                colTemp = colTemp + "{ \"field\": \"" + temp.BaseColumn.ToUpper() + "\",";
                                colTemp = colTemp + "\"name\": \"" + temp.Name + "\",";
                                colTemp = colTemp + "\"width\": \"" + temp.Width + "\"";
                                colTemp = colTemp + "},";
                            }
                            if (colTemp.Length > 0)
                            {
                                colTemp = colTemp.Substring(0, colTemp.Length - 1);
                                colTemp = "[" + colTemp + "]";
                            }
                            colName = colName + ",\"itemTemplate\": " + colTemp + "";
                        }
                        colName = colName + ",\"editor\": \"" + r.EditControlType.ToString() + "\"";
                    }
                    colName = colName + ",\"lke\": \"" + Until.NVL(r.lke) + "\"";
                    colName = colName + ",\"url\": \"" + Until.NVL(r.UrlReference) + "\"";
                    colName = colName + ",\"dataType\": \"" + Until.NVL(r.DataType.ToString()) + "\"";
                    colName = colName + ",\"maxLength\": \"" + Until.NVL(r.MaxLength.ToString()) + "\"";
                    colName = colName + ",\"upperCase\": \"" + Until.NVL(r.UpperCase.ToString()) + "\"";
                    colName = colName + ",\"dec\": \"" + Until.NVL(r.Dec.ToString()) + "\"";
                    colName = colName + ",\"isEditCell\": \"" + (r.EditControlType == EditorType.None ? r.IsEditCell.ToString() : "true") + "\"";
                    if (!string.IsNullOrEmpty(r.HeaderTemplate))
                        colName = colName + ",\"headerTemplate\": \"" + Until.NVL(r.HeaderTemplate) + "\"";
                    colName = colName + ",\"icon\": \"none\"";
                    if (r.Icon != IconType.NONE)
                    {
                        string url = Base.GetImageURL(r.Icon);
                        string imageUrl = Page.ClientScript.GetWebResourceUrl(this.GetType(), url);
                        colName = colName + ",\"icon\": \"" + imageUrl + "\"";
                    }
                    colName = colName + "},";
                }

            }
            if (colName.Length > 0)
            {
                colName = colName.Substring(0, colName.Length - 1);
                colName = "[" + colName + "]";
            }
            string dataJson = "";
            if (this._DataSource != null)
                dataJson = LibTable.TableToJson(this._DataSource);
            ScriptControlDescriptor descriptor = new ScriptControlDescriptor("Lib.gridView ", this.ClientID);
            descriptor.AddProperty("columns", colName);
            descriptor.AddProperty("dataSource", dataJson);
            descriptor.AddProperty("width", this.Width.ToString());
            descriptor.AddProperty("height", this.Height.ToString());
            descriptor.AddProperty("pageSize", this.GetPageSize().ToString());
            descriptor.AddProperty("onClientCellClick", this.onClientCellClick);
            descriptor.AddProperty("typePaging", this.TypePaging.ToString());
            descriptor.AddProperty("onClientUpdateCell", this.onClientUpdateCell);
            descriptor.AddProperty("onClientBeforEdit", this.onClientBeforEdit);
            descriptor.AddProperty("hidenHeader", this.HidenHeader);
            descriptor.AddProperty("onClientDbClick", this.onClientDbClick);

            return new ScriptDescriptor[] { descriptor };

        }
        IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
        {
            return GetScriptReferences();
        }
        IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
        {
            return GetScriptDescriptors();
        }
        #endregion
    }
}
