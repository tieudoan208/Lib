using System;
using System.Web;
using System.Data;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using Lib.Utilities;
using Lib.Constant;

namespace Lib.Controls
{
    [ToolboxData("<{0}:ColorPicker runat=server></{0}:ColorPicker>")]
    [System.Drawing.ToolboxBitmap(typeof(ColorPicker), "Images.combo_arrow.png")]
    public class DropDowGrid : WebControl, IScriptControl
    {
        private ScriptManager sm;
        private List<Column> _Columns;
        private List<Frame> _Frame;
        private DataTable _DataSource;
        private bool _hidenHeader;
        private int _widthGrid;
        private bool _multiCheck;


        [Description("DataSource"), Bindable(true), Category("Option")]
        public DataTable DataSource
        {
            get { return _DataSource; }
            set { _DataSource = value; }
        }
        [Description("HidenHeader"), Bindable(true), Category("Option")]
        public bool HidenHeader
        {
            get { return _hidenHeader; }
            set { _hidenHeader = value; }
        }

        [Browsable(true)]
        [Description("Tên gốc control"), Category("Options"), Bindable(true)]
        public string Name
        {
            get { return this.ID; }
        }
        [Browsable(true)]
        [Description("Cho phép edit trên textbox hay không"), Category("Options"), Bindable(true)]
        public bool IsEdit
        {
            get;
            set;
        }
        [Browsable(true)]
        [Description("Lấy giá trị hiện thị"), Category("Options"), Bindable(true)]
        public string DataValue
        {
            get;
            set;
        }
        [Browsable(true)]
        [Description("Giá trị hiện thị"), Category("Options"), Bindable(true)]
        public string DisplayValue
        {
            get;
            set;
        }
        [Browsable(true)]
        [Description("Cho phép chọn nhiều items, khi cick ngoài vùng sẽ close"), Category("Options"), Bindable(true)]
        public bool MultiCheck
        {
            get { return _multiCheck; }
            set { _multiCheck = value; }
        }
        [Browsable(true)]
        [Description("Sự kiện xây ra khi click vào một Items trong lưới hiện thị"), Category("Options"), Bindable(true)]
        public string OnClientSelectItem
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
        public List<Frame> Frame
        {
            get
            {
                if (this._Frame == null)
                    this._Frame = new List<Frame>();
                return this._Frame;
            }
        }
        public int GetWidthGrid
        {
            get
            {
                 return  _Frame[0].Width;
            }
        }
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

            HtmlGenericControl span = new HtmlGenericControl("SPAN");
            span.Attributes.Add("id", this.ClientID);
            span.Style.Add(HtmlTextWriterStyle.Height, this.Height.ToString());
            span.Style.Add(HtmlTextWriterStyle.Width, this.Width.ToString());
            if (!string.IsNullOrEmpty(this.CssClass))
                span.Attributes.Add("class", this.CssClass);
            else
                span.Attributes.Add("class", "combo-grid combo-grid-text");
            //Text
            HtmlGenericControl txt = new HtmlGenericControl("INPUT");
            txt.Attributes.Add("id", this.ClientID + "_txt");
            txt.Attributes.Add("class", "textbox-text  validatebox-text textbox-prompt");
            txt.Style.Add(HtmlTextWriterStyle.Width, (this.Width.Value - 20).ToString() + "px");
            txt.Style.Add(HtmlTextWriterStyle.MarginLeft, "0px");
            txt.Style.Add(HtmlTextWriterStyle.MarginRight, "18px");
            txt.Style.Add(HtmlTextWriterStyle.PaddingTop, "4px");
            txt.Style.Add(HtmlTextWriterStyle.PaddingBottom, "1px");

            HtmlGenericControl span2 = new HtmlGenericControl("SPAN");
            span2.Attributes.Add("class", "combo-grid-text-addon");
            span2.Style.Add("right", "0px");

            HtmlGenericControl a = new HtmlGenericControl("A");
            a.Attributes.Add("class", "combo-grid-text-addon-icon combo-grid-arrow");
            a.Style.Add(HtmlTextWriterStyle.Width, "18px");
            a.Style.Add("border", "0px");
            a.Attributes.Add("id", this.ClientID + "_arrow");
            a.Style.Add(HtmlTextWriterStyle.Height, "20px");
            a.Attributes.Add("tabindex", "-1");

            HtmlGenericControl val = new HtmlGenericControl("hidden");
            val.Attributes.Add("values", "");
            val.Attributes.Add("id", this.ClientID + "_value");
            span.Controls.Add(txt);

            HtmlGenericControl grid= new HtmlGenericControl("DIV");
            grid.Style.Add("margin-bottom", "5px");
            grid.Style.Add(HtmlTextWriterStyle.Width, _Frame[0].Width.ToString()+"px");
            grid.Style.Add(HtmlTextWriterStyle.Height, _Frame[0].Height.ToString()+"px");
            grid.Attributes.Add("id", this.ClientID + "_grid");
            span2.Controls.Add(a);
            span.Controls.Add(span2);
            span.Controls.Add(val);
            span.RenderControl(writer);
            grid.RenderControl(writer);
           // base.Render(writer);
        }

        #region Register Script for MÃ

        protected virtual IEnumerable<ScriptReference> GetScriptReferences()
        {
            return new ScriptReference[] {
                new ScriptReference("Lib.Resources.dropdowGrid.js", "Lib")
            };
        }
        protected virtual IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            string colName = "";
            if (this._Columns != null)
            {
                if (_hidenHeader)
                {
                    colName = colName + "{\"name\": \"\",";
                    colName = colName + "\"id\": \"#\",";
                    colName = colName + "\"cssClass\": \"cell-reorder dnd\",";
                    colName = colName + "\"width\": 40},";
                }
                foreach (var r in this._Columns)
                {
                    _widthGrid = _widthGrid + r.Width;
                    colName = colName + "{ \"field\": \"" + r.BaseColumn.ToUpper() + "\",";
                    colName = colName + "\"name\": \"" + r.Name + "\",";
                    colName = colName + "\"id\": \"" + r.BaseColumn.ToLower() + "\",";
                    colName = colName + "\"source\":  \"" + r.Source + "\",";
                    colName = colName + "\"width\": \"" + r.Width + "\",";
                    colName = colName + "\"hidden\": \"" + r.Hidden + "\"";
                    if (Until.NVL(r.CssClass) != "")
                        colName = colName + ",\"cssClass\": \"" + r.CssClass + "\"";
                    if (r.EditControlType != EditorType.None)
                        colName = colName + ",\"editor\": \"" + r.EditControlType.ToString() + "\"";

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

            ScriptControlDescriptor descriptor = new ScriptControlDescriptor("Lib.dropdowGrid", this.ClientID);
            descriptor.AddProperty("Name", this.Name.ToString());
            descriptor.AddProperty("columns", colName);
            descriptor.AddProperty("dataSource", dataJson);
            descriptor.AddProperty("multiCheck", _multiCheck.ToString());
            descriptor.AddProperty("pageSize", _Frame[0].PageSize.ToString());
            descriptor.AddProperty("dataValue", DataValue);
            descriptor.AddProperty("displayValue", DisplayValue);
            descriptor.AddProperty("onClientSelectItem", OnClientSelectItem);
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
