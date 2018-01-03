using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Lib.Utilities;
using Lib.Constant;

namespace Lib.Helper
{

    public class RegisterResouce
    {

        public static void Include_JS(ClientScriptManager manager, bool late = false)
        {
            int nam = DateTime.Now.Year;
            int thang = DateTime.Now.Month;
          
            IncludeJavaScript(manager, Base.NAME_JQUERY, late);
            IncludeJavaScript(manager, Base.NAME_COMMON, late);
            IncludeJavaScript(manager, Base.NAME_UNTIL, late);
            IncludeJavaScript(manager, Base.NAME_EVENT_DRAP, late);
            IncludeJavaScript(manager, Base.NAME_SLICK_CORE, late);
            IncludeJavaScript(manager, Base.NAME_ROW_SLECTMODE, late);
            IncludeJavaScript(manager, Base.NAME_CELL_SELECTION, late);
            IncludeJavaScript(manager, Base.NAME_GRID_EDITOR, late);
            IncludeJavaScript(manager, Base.NAME_SLICK_GRID, late);
            IncludeJavaScript(manager, Base.NAME_JQUERY_UI, late);
            IncludeJavaScript(manager, Base.NAME_GRID, late);
        }

        private static void IncludeJavaScript(ClientScriptManager manager, string resourceName, bool late)
        {
            var type = typeof(RegisterResouce);
            if (!manager.IsStartupScriptRegistered(type, resourceName))
            {
                if (late)
                {
                    var url = manager.GetWebResourceUrl(type, resourceName);
                    var scriptBlock = string.Format(Base.TEMPLATE_SCRIPT, HttpUtility.HtmlEncode(url));
                    manager.RegisterStartupScript(type, resourceName, scriptBlock);
                }
                else
                {
                    manager.RegisterClientScriptResource(type, resourceName);
                    manager.RegisterStartupScript(type, resourceName, string.Empty);
                }
            }
        }
        public static void Include_CSS(Page _page, Control target)
        {
            RegisterCSSInclude(target, _page, "BASE_CSS", Base.NAME_CSS_BASE);
            RegisterCSSInclude(target, _page, "COLOR_CSS", Base.NAME_CSS_COLOR);
            RegisterCSSInclude(target, _page, "COLOR_GRID_SLICK", Base.NAME_CSS_SLICK_GRID);
            RegisterCSSInclude(target, _page, "COLOR_GRID", Base.NAME_CSS_GRID);
        }

        private static void RegisterCSSInclude(Control target, Page _page, string resourceName, string pathResource)
        {
            var type = typeof(RegisterResouce);
            bool linkIncluded = false;
            foreach (Control c in target.Controls)
            {
                if (c.ID == resourceName)
                {
                    linkIncluded = true;
                }
            }
            if (!linkIncluded)
            {
                HtmlGenericControl csslink = new HtmlGenericControl("link");
                csslink.ID = resourceName;
                csslink.Attributes.Add("href", _page.ClientScript.GetWebResourceUrl(type, pathResource));
                csslink.Attributes.Add("type", "text/css");
                csslink.Attributes.Add("rel", "stylesheet");
                csslink.EnableViewState = false;
                target.Controls.Add(csslink);
            }
        }


        private static void IncludeExternalJavaScript(Page page, string key, string httpUrl, string httpsUrl, bool late)
        {
            var manager = page.ClientScript;
            var type = typeof(RegisterResouce);
            bool isStartupRegistered = manager.IsStartupScriptRegistered(type, key);
            bool isScriptRegistered = manager.IsClientScriptIncludeRegistered(type, key);
            if (!(isStartupRegistered || isScriptRegistered))
            {
                string url;
                if (page.Request.Url.Scheme.ToLower() == "http")
                {
                    url = httpUrl;
                }
                else
                {
                    url = httpsUrl;
                }
                if (late)
                {
                    manager.RegisterStartupScript(type, key, string.Format(Base.TEMPLATE_SCRIPT, HttpUtility.HtmlEncode(url)));
                }
                else
                {
                    manager.RegisterClientScriptInclude(type, key, url);
                }
            }
        }

        private static void ExcludeJavaScript(ClientScriptManager manager, string key)
        {
            var type = typeof(RegisterResouce);
            var url = manager.GetWebResourceUrl(type, "");
            manager.RegisterStartupScript(type, key, string.Empty);
            manager.RegisterClientScriptInclude(type, key, url);
        }
    }
}