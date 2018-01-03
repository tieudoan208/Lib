using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using System.Collections.Generic;
using Lib.Constant;

namespace Lib.Controls
{
    public class Button : HtmlInputButton, IScriptControl
    {
        #region Private

        private ScriptManager sm;
        private string _onClientClick;
        private string _onClientBblClick;
        private string _cssClass;
        private string _with;
        private string _text;

        [DefaultValue("k-button")]
        [Description("Nhúng CSS và nút"), Bindable(true), Category("Options")]
        public string CssClass
        {
            get { return _cssClass; }
            set { _cssClass = value; }
        }

        [Description("Chiều dài của nút"), Category("Options"), Bindable(true)]
        public string Width
        {
            get
            {
                return _with;
            }
            set
            {
                _with = value;
            }
        }

        [Description("Text"), Bindable(true), Category("Options")]
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
            }
        }

        [Description("Sự kiện khi ấn nút"), Bindable(true), Category("Options")]
        public string onClientClick
        {
            get { return this._onClientClick; }
            set { this._onClientClick = value; }
        }
        [Description("Sự kiện khi double click lên nút"), Bindable(true), Category("Options")]
        public string onClientDblClick
        {
            get { return this._onClientBblClick; }
            set { this._onClientBblClick = value; }
        }
        [Description("Đường dẫn tới Icon"), Bindable(true), Category("Options")]
        public IconType Icon
        {
            get;
            set;
        }

        #endregion

        #region Method
        protected override void OnPreRender(EventArgs e)
        {
            if (!this.DesignMode)
            {
                sm = ScriptManager.GetCurrent(this.Page);
                if (sm == null) throw new HttpException("Cần khai báo thêm Scriptmanager vào control");
                sm.RegisterScriptControl(this);
            }

            base.OnPreRender(e);
            base.Style.Add(HtmlTextWriterStyle.Width, this._with);
            base.Style.Add(HtmlTextWriterStyle.Height, "30px");
            base.Attributes.Add("ICON", this.Icon.ToString());
            base.Attributes["class"] = (_cssClass == null || _cssClass == "" ? "buttion" : _cssClass);
            string imageUrl = "";
            if (Icon != IconType.NONE)
            {
                string Url = Lib.Constant.Base.GetImageURL(Icon);
                imageUrl = Page.ClientScript.GetWebResourceUrl(this.GetType(), Url);
            }
            base.Style.Add(HtmlTextWriterStyle.BackgroundImage, imageUrl);

        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!this.DesignMode)
                sm.RegisterScriptDescriptors(this);
            base.Render(writer);
        }

        #endregion

        #region Register Js
        protected virtual IEnumerable<ScriptReference> GetScriptReferences()
        {

            return new ScriptReference[] {
                new ScriptReference("Lib.Resources.button.js", "Lib")
            };
        }

        protected virtual IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            ScriptControlDescriptor descriptor = new ScriptControlDescriptor("Lib.button", this.ClientID);
            descriptor.AddProperty("onClientClick", this._onClientClick);
            descriptor.AddProperty("onClientDblClick", this._onClientBblClick);
            descriptor.AddProperty("width", this._with);
            descriptor.AddProperty("text", this._text);
            descriptor.AddProperty("cssClass", this._cssClass);
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
