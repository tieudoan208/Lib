using System;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Web.UI;
using System.Web;
using System.Collections.Generic;

namespace Lib.Controls
{

    public class dao : Label, IScriptControl
    {
        private ScriptManager sm;

        [Browsable(true)]
        [Description("Tên gốc control"), Category("Options"), Bindable(true)]
        public string Name
        {
            get { return this.ID; }
        }
        [Browsable(true)]
        [Description("CHuỗi truyền giá trị"), Category("Options"), Bindable(true)]
        public string lke
        {
            get;
            set;
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
            base.Render(writer);
        }

        #region Register Script for MÃ

        protected virtual IEnumerable<ScriptReference> GetScriptReferences()
        {
            return new ScriptReference[] {
                new ScriptReference("Lib.Resources.dao.js", "Lib")
            };
        }
        protected virtual IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            ScriptControlDescriptor descriptor = new ScriptControlDescriptor("Lib.dao", this.ClientID);
            descriptor.AddProperty("Name", this.Name.ToString());
            descriptor.AddProperty("lke", this.lke);
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
