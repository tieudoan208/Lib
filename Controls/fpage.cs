using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Resources;
using System.Web.UI.HtmlControls;
using Lib.Helper;
using Lib.Utilities;

namespace Lib.Controls
{

    public class fpage : Page
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            RegisterResouce.Include_JS(this.Page.ClientScript);
            RegisterResouce.Include_CSS(this.Page, this.Header);
           
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "document_f1", "document.onhelp=function() {return false;};", true);
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "window_f1", "window.onhelp=function() {return false;};", true);
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "window_ten", "window.name='" + this.Title + "';", true);
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "p_goidong", "window.onunload=function() {CloseForm('" + this.Title + "',null);};", true);
            Control control = LibForm.FindControl(this, "kthuoc");
            if (control != null)
            {
                string[] strArray = Until.NVL((control as HiddenField).Value).Split(new char[] { ',' });
                ClientScript.RegisterClientScriptBlock(base.GetType(), "p_kthuoc", "window.onload=function() {" + ("FormSize(" + strArray[0] + "," + strArray[1] + ");") + "};", true);
            }
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (!Constant.Base.IsRedirect)
                return;
            if(!string.IsNullOrEmpty(Constant.Base.URL_TIMEOUT))
            {
                string[] aUrl = Constant.Base.URL_TIMEOUT.Split('/');
                string nameForm = aUrl[aUrl.Length - 1].Replace(".aspx", "");
                if (System.Web.HttpContext.Current.Session["nsd"] == null && nameForm.ToUpper()!=this.Title.ToUpper())
                {
                    string url = this.ResolveClientUrl(Constant.Base.URL_TIMEOUT);
                    Response.Redirect(url);
                }
            }
        }
        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            this.Dispose();
            GC.Collect();
        }
    }
}
