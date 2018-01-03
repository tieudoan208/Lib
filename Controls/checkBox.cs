using System;
using System.Web;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Collections.Generic;

namespace Lib.Controls
{

    public class checkBox : CheckBox, IScriptControl
    {

        #region Peivate

        private ScriptManager sm;
        private string _onClientClick;

        #endregion

        #region Public pro

        [Browsable(true)]
        [Description("Sự kiện xẩy ra khi "), Category("Options"), Bindable(true)]
        public string onClientClick
        {
            get { return this._onClientClick; }
            set { _onClientClick = value; }
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
        [Description("Tên gốc control"), Category("Options"), Bindable(true)]
        public string Name
        {
            get { return this.ID; }
        }
        [Browsable(true)]
        [DefaultValue(true)]
        [Description("Xoa trắng giá trị hay không"), Category("Options"), Bindable(true)]
        public bool IsReset
        {
            get;
            set;
        }

        #endregion

        #region base
        protected override void OnPreRender(EventArgs e)
        {
            if (!this.DesignMode)
            {
                sm = ScriptManager.GetCurrent(this.Page);
                if (sm == null) throw new HttpException("Cần khai báo thêm Scriptmanager vào control");
                sm.RegisterScriptControl(this);
            }
            base.OnPreRender(e);
            this.IsInputData = true;
        }
        protected override void Render(HtmlTextWriter writer)
        {
            if (!this.DesignMode)
                sm.RegisterScriptDescriptors(this);
            base.Render(writer);
        }
        #endregion

        #region Register Script
        protected virtual IEnumerable<ScriptReference> GetScriptReferences()
        {
            return new ScriptReference[] {
                new ScriptReference("Lib.Resources.checkBox.js", "Lib")
            };
        }

        protected virtual IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            ScriptControlDescriptor descriptor = new ScriptControlDescriptor("Lib.checkBox", this.ClientID);
            descriptor.AddProperty("onClientClick", this._onClientClick);
            descriptor.AddProperty("name", this.Name);
            descriptor.AddProperty("isInputData", this.IsInputData.ToString());
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
    }
}
