using System;
using System.Web;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Lib.Constant;

namespace Lib.Controls
{
    public class textBox : TextBox, IScriptControl
    {
        private ScriptManager sm;
        private DataType _dataType;
        private bool _upperCase;
        private string _lke;
        private TypeControl _typeControl;
        private string _msg;
        private string _urlReference;
        private bool _checkDate;
        private bool _isReset = true;

        //Event
        private string _onClientChange;
        private string _onClientBlur;
        private string _onClientClick;
        private string _onClinetDblClick;



        [Browsable(true)]
        [Description("Kiểu text"), Category("Options"), Bindable(true)]
        public DataType Type
        {
            get { return _dataType; }
            set { _dataType = (value.ToString() == "" ? DataType.String : value); }
        }
        [Browsable(true)]
        [Description("Độ rộng tôi đa ký tự "), Category("Options"), Bindable(true)]
        public int MaxLength
        {
            get;
            set;
        }
        [Browsable(true)]
        [DefaultValue(false)]
        [Description("Viết hoa hay không"), Category("Options"), Bindable(true)]
        public bool UpperCase
        {
            get { return _upperCase; }
            set { _upperCase = value; }
        }

        [Browsable(true)]
        [DefaultValue("")]
        [Description("Kiểu liệt kê"), Category("Options"), Bindable(true)]
        public string lke
        {
            get { return _lke; }
            set { _lke = value; }
        }
        [Browsable(true)]
        [DefaultValue(TypeControl.TextBox)]
        [Description("Kiểu control"), Category("Options"), Bindable(true)]
        public TypeControl TypeControl
        {
            get { return _typeControl; }
            set { _typeControl = value; }
        }
        [Browsable(true)]
        [DefaultValue("")]
        [Description("Nội dung thông báo khi kiểm tra"), Category("Options"), Bindable(true)]
        public string Msg
        {
            get { return _msg; }
            set { _msg = value; }
        }
        [Browsable(true)]
        [Description("Ấ F1 Brow đến form khác"), Category("Options"), Bindable(true)]
        public string UrlReference
        {
            get { return _urlReference; }
            set { _urlReference = value; }
        }
        [Browsable(true)]
        [Description("Kiểm tra hợp lệ ngày, tháng, năm"), Category("Options"), Bindable(true)]
        public bool CheckDate
        {
            get { return _checkDate; }
            set { _checkDate = value; }
        }
        [Browsable(true)]
        [Description("Tên gốc control"), Category("Options"), Bindable(true)]
        public string Name
        {
            get { return this.ID; }
        }
        [Browsable(true)]
        [Description("Kiểu lưu dữ liệu"), Category("Options"), Bindable(true)]
        public SaveType SaveTypeData
        {
            get;
            set;
        }
        [Browsable(true)]
        [DefaultValue(true)]
        [Description("Có lấy số liệu hay không"), Category("Options"), Bindable(true)]
        public bool IsInputData
        {
            get;
            set;
        }
        [Browsable(true)]
        [Description("Số phẩy phần thập phân"), Category("Options"), Bindable(true)]
        public int Decimal
        {
            get;
            set;
        }
        [Browsable(true)]
        [DefaultValue(true)]
        [Description("Xoa trắng giá trị hay không"), Category("Options"), Bindable(true)]
        public bool IsReset
        {
            get
            {
                return _isReset;
            }
            set
            {
                if (value == null)
                    _isReset = true;
                else _isReset = value;
            }
        }
        //Event
        [Browsable(true)]
        [Description("Event khi có sự thay đổi giá trị trên text"), Category("Options"), Bindable(true)]
        public string onClientChange
        {
            get { return _onClientChange; }
            set { _onClientChange = value; }
        }
        [Browsable(true)]
        [Description("Event khi rơi khỏi control"), Category("Options"), Bindable(true)]
        public string onClientBlur
        {
            get { return _onClientBlur; }
            set { _onClientBlur = value; }
        }
        [Browsable(true)]
        [Description("Event khi click vào textBox"), Category("Options"), Bindable(true)]
        public string onClientClick
        {
            get { return _onClientClick; }
            set { _onClientClick = value; }
        }
        [Browsable(true)]
        [Description("Event khi DblClick"), Category("Options"), Bindable(true)]
        public string onClientDblClick
        {
            get { return _onClinetDblClick; }
            set { _onClinetDblClick = value; }
        }

        #region Thuộc tính

        #endregion

        protected override void OnPreRender(EventArgs e)
        {
            this.EnableViewState = false;

            if (!this.DesignMode)
            {
                sm = ScriptManager.GetCurrent(this.Page);
                if (sm == null) throw new HttpException("Cần khai báo thêm Scriptmanager vào control");
                sm.RegisterScriptControl(this);
            }
            base.OnPreRender(e);

            if (this._upperCase)
                this.Text = this.Text.ToUpper();
            if (!string.IsNullOrEmpty(this._lke))
            {
                //this.Width = 20;
                this._upperCase = true;
                this.Style.Add(HtmlTextWriterStyle.TextAlign, "center");
            }

            if (this._typeControl == TypeControl.TextArea)
            {
                if (this.Rows == 0)
                    this.Rows = 4;
                this.TextMode = TextBoxMode.MultiLine;
                this.Style.Add(" font-family", "Times New Roman, Arial");
                this.Style.Add(" font-size", "14px");
            }
            else
                this.Height = 18;
            this.CssClass = "lib-textbox txt";
            if (this._dataType == DataType.Date)
                this.Style.Add(HtmlTextWriterStyle.TextAlign, "center");
            this.IsInputData = true;
        }
        protected override void Render(HtmlTextWriter writer)
        {
            if (!this.DesignMode)
                sm.RegisterScriptDescriptors(this);
            base.Render(writer);
        }

        #region Register Script for MÃ

        protected virtual IEnumerable<ScriptReference> GetScriptReferences()
        {
            return new ScriptReference[] {
                new ScriptReference("Lib.Resources.textBox.js", "Lib")
            };
        }
        protected virtual IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            ScriptControlDescriptor descriptor = new ScriptControlDescriptor("Lib.textBox", this.ClientID);
            descriptor.AddProperty("typeData", this._dataType.ToString());
            descriptor.AddProperty("clientChange", this._onClientChange);
            descriptor.AddProperty("upperCase", this._upperCase);
            descriptor.AddProperty("lke", this._lke);
            descriptor.AddProperty("onClientBlur", this._onClientBlur);
            descriptor.AddProperty("onClientClick", this._onClientClick);
            descriptor.AddProperty("onClinetDblClick", this._onClinetDblClick);
            descriptor.AddProperty("urlReference", this._urlReference);
            descriptor.AddProperty("checkDate", this._checkDate);
            descriptor.AddProperty("msg", this._msg);
            descriptor.AddProperty("name", this.Name);
            descriptor.AddProperty("isInputData", this.IsInputData.ToString());
            descriptor.AddProperty("saveTypeData", this.SaveTypeData.ToString());
            descriptor.AddProperty("decimal", this.Decimal.ToString());
            descriptor.AddProperty("isReset", this.IsReset.ToString());
            descriptor.AddProperty("maxLength", this.MaxLength.ToString());
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
