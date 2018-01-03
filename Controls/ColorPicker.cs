using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Specialized;

namespace Lib.Controls
{
    public enum PopupPosition
    {
        BottomRight=0,
        BottomLeft,
        TopRight,
        TopLeft
    }

    [DefaultProperty("Text")]
    [ToolboxData("<{0}:ColorPicker runat=server></{0}:ColorPicker>")]
    [System.Drawing.ToolboxBitmap(typeof(ColorPicker),"Images.ColorPickerIcon.jpg")]
    public class ColorPicker : WebControl, IPostBackDataHandler
    {
        #region Events

        public event EventHandler ColorChanged;

        #endregion        

        #region Public Properties

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("#000000")]
        [Localizable(true)]
        public string Color
        {
            get
            {                
                return _color;
            }

            set
            {
                _color = value;
            }
        }

        [Bindable(true)]
        [Category("Behaviour")]
        [DefaultValue("false")]
        [Localizable(true)]
        public bool AutoPostBack
        {
            get
            {
                return (bool)(ViewState["AutoPostBack"] ?? false);
            }

            set
            {
                ViewState["AutoPostBack"] = value;
            }
        }

        [Bindable(true)]
        [Category("Behaviour")]
        [DefaultValue("0")]
        [Localizable(true)]
        public PopupPosition PopupPosition
        {
            get
            {
                return (PopupPosition)(ViewState["PopupPosition"] ?? PopupPosition.BottomRight);
            }

            set
            {
                ViewState["PopupPosition"] = value;
            }
        }

        #endregion

        #region Web.Control implementation

        protected override void OnInit(EventArgs e)
        {            
            // Javascript
            string colorFunctions = Page.ClientScript.GetWebResourceUrl(typeof(ColorPicker), "Lib.Resources.ColorPicker.js");
            Page.ClientScript.RegisterClientScriptInclude("ColorPicker.js", colorFunctions);            

            // Create ColorPicker object
            string script = string.Format(@"
var colorPicker = new ColorPicker({{
FormWidgetAmountSliderHandleImage : '{0}',
TabRightActiveImage : '{1}',
TabRightInactiveImage : '{2}',
TabLeftActiveImage : '{3}',
TabLeftInactiveImage : '{4}',
AutoPostBack : {5},
AutoPostBackReference : ""{6}"",
PopupPosition : {7}
}});            
            ", Page.ClientScript.GetWebResourceUrl(typeof(ColorPicker), "Lib.Images.SliderHandle.gif")
             , Page.ClientScript.GetWebResourceUrl(typeof(ColorPicker), "Lib.Images.TabRightActive.gif")
             , Page.ClientScript.GetWebResourceUrl(typeof(ColorPicker), "Lib.Images.TabRightInactive.gif")
             , Page.ClientScript.GetWebResourceUrl(typeof(ColorPicker), "Lib.Images.TabLeftActive.gif")
             , Page.ClientScript.GetWebResourceUrl(typeof(ColorPicker), "Lib.Images.TabLeftInactive.gif")             
             , AutoPostBack?"true":"false"
             , Page.ClientScript.GetPostBackEventReference(this,"")
             , (int)PopupPosition
             );

            Page.ClientScript.RegisterStartupScript(Page.GetType(), "InitColorPicker", script, true);
            if (!DesignMode && Page.Header != null)
            {
               // RegisterCSSInclude(Page.Header);
            }          
            
        }

        protected override void LoadViewState(object savedState)
        {
            Color = (string)savedState;
        }

        protected override object SaveViewState()
        {
            return (object)Color;
        }

      

        protected override void Render(HtmlTextWriter output)
        {
            PlaceHolder plh = new PlaceHolder();
            Table table = new Table();              
            table.Rows.Add(new TableRow());
            table.Rows[0].Cells.Add(new TableCell());
            table.Rows[0].Cells.Add(new TableCell());            
            HtmlGenericControl txt = new HtmlGenericControl("input");
            txt.EnableViewState = false;
            txt.Attributes.CssStyle.Add(HtmlTextWriterStyle.Color, Color);
            txt.Attributes.Add("maxlength","15");
            txt.Attributes.Add("size", "15");
            txt.Attributes.Add("value", Color);                        
            txt.Attributes.Add("id",ColorInputControlClientId);
            txt.Attributes.Add("name",this.UniqueID);
            table.Rows[0].Cells[0].Controls.Add(txt);            
            HtmlInputImage btn = new HtmlInputImage();
            btn.Src = Page.ClientScript.GetWebResourceUrl(typeof(ColorPicker), "Lib.Images.ColorPickerIcon.jpg");
            btn.Attributes.Add("onclick", string.Format("colorPicker.ShowColorPicker(this,document.getElementById('{0}'));return false;", ColorInputControlClientId));
            HtmlGenericControl container = new HtmlGenericControl("div");
            container.EnableViewState = false;
            container.Controls.Add(btn);
            container.Attributes.CssStyle.Add(HtmlTextWriterStyle.Position, "relative");
            container.Attributes.CssStyle.Add(HtmlTextWriterStyle.Display, "block");
            table.Rows[0].Cells[1].Controls.Add(container);
            plh.Controls.Add(table);
            plh.RenderControl(output);            
        }               

        #endregion

        #region IPostBackDataHandler

        public bool LoadPostData(string postDataKey,NameValueCollection postCollection)
        {
            String presentValue = Color;
            String postedValue = postCollection[postDataKey];

            if (presentValue == null || !presentValue.Equals(postedValue))
            {
                Color = postedValue;
                return true;
            }
            return false;
        }

        public virtual void RaisePostDataChangedEvent()
        {
            OnColorChanged(EventArgs.Empty);
        }

        public void OnColorChanged(EventArgs e)
        {
            if (ColorChanged != null)
                ColorChanged(this, e);
        }

        #endregion

        #region Public static methods

        public static System.Drawing.Color StringToColor(string colorString)
        {
            System.Drawing.Color color;
            if (colorString[0] == '#' && colorString.Length < 8)
            {
                string s = colorString.Substring(1);
                while (s.Length != 6)
                {
                    s = string.Concat("0", s);
                }
                int red = Convert.ToInt32(s.Substring(0, 2), 16);
                int green = Convert.ToInt32(s.Substring(2, 2), 16);
                int blue = Convert.ToInt32(s.Substring(4, 2), 16);
                color = System.Drawing.Color.FromArgb(red, green, blue);
            }
            else
            {
                color = System.Drawing.Color.FromName(colorString);
            }
            return color;
        }
        public static string ColorToString(System.Drawing.Color color)
        {
            string result;
            if (color.IsKnownColor || color.IsNamedColor || color.IsSystemColor)
            {
                result = color.Name;
            }
            else
            {
                result = string.Concat("#", color.ToArgb().ToString("X").Substring(2));
            }
            return result;
        }

        #endregion

        #region Private properties

        private string ColorInputControlClientId
        {
            get { return string.Concat(ID, "Input"); }
        }        

        #endregion

        #region Private variables

        string _color = "#000000";

        #endregion
    }
}
