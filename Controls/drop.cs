using System;
using System.Web;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Lib.Constant;

namespace Lib.Controls
{
    public class drop : DropDownList, IScriptControl
    {
        #region private
        private ScriptManager sm;
        private string _onClientBlur;
        private string _onClientChange;
        private SaveType _saveType;
        private bool _isInputdata;

        #endregion

        #region public
        [Browsable(true)]
        [Description("Sự kiện xẩy ra khi con trỏ dời khỏi drop"), Category("Option"), Bindable(true)]
        public string onClientBlur
        {
            get { return this._onClientBlur; }
            set { _onClientBlur = value; }
        }
        [Browsable(true)]
        [Description("Sự kiện xẩy ra khi con trỏ dời khỏi drop, dữ liệu có sự thay đổi"), Category("Option"), Bindable(true)]
        public string onClientChange
        {
            get { return _onClientChange; }
            set { _onClientChange = value; }
        }
        [Browsable(true)]
        [DefaultValue(SaveType.String)]
        [Description("Kiểu lưu"), Category("Option"), Bindable(true)]
        public SaveType saveType
        {
            get { return _saveType; }
            set { _saveType = value; }
        }
        [Browsable(true)]
        [DefaultValue(true)]
        [Description("Có lấy số liệu hay không"), Category("Options"), Bindable(true)]
        public bool IsInputData
        {
            get { return _isInputdata; }
            set
            {
                if (value == null)
                    _isInputdata = true;
                else _isInputdata = value;
            }
        }

        [Browsable(true)]
        [Description("Tên gốc control"), Category("Option"), Bindable(true)]
        public string Name
        {
            get { return this.ID; }
        }

        [Browsable(true)]
        [DefaultValue(true)]
        [Description("Xoa trắng giá trị hay không"), Category("Option"), Bindable(true)]
        public bool IsReset
        {
            get;
            set;
        }

        #endregion

        #region Đăng ký
        protected virtual IEnumerable<ScriptReference> GetScriptReferences()
        {
            return new ScriptReference[] {
                new ScriptReference("Lib.Resources.Drop.js", "Lib")
            };
        }

        protected virtual IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            ScriptControlDescriptor descriptor = new ScriptControlDescriptor("Lib.drop", this.ClientID);
            descriptor.AddProperty("onClientBlur", this._onClientBlur);
            descriptor.AddProperty("onClientChange", this._onClientChange);
            descriptor.AddProperty("saveType", this._saveType.ToString());
            descriptor.AddProperty("dataValue", this.DataValueField.ToString());
            descriptor.AddProperty("dataText", this.DataTextField.ToString());
            descriptor.AddProperty("name", this.Name.ToString());
            descriptor.AddProperty("isInputData", this._isInputdata.ToString());
            descriptor.AddProperty("isReset", this.IsReset.ToString());
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

        #region Override

        protected override void OnPreRender(EventArgs e)
        {  //Đăng ký script contro;
            if (!this.DesignMode)
            {
                sm = ScriptManager.GetCurrent(this.Page);
                if (sm == null) throw new HttpException("Cần khai báo thêm Scriptmanager vào control");
                sm.RegisterScriptControl(this);
            }
            base.OnPreRender(e);
            base.Attributes.Add("ten_goc", this.ID);
            _isInputdata = true;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!this.DesignMode)
                sm.RegisterScriptDescriptors(this);

            base.Render(writer);
        }

        #endregion
    }
}
