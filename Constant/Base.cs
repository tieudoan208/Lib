
using Oracle.DataAccess.Client;

namespace Lib.Constant
{
    public enum DataType
    {
        String,
        Date,
        Month,
        Number,
        Money,
        YesNo,
        Unicode
    }
    public enum TypeControl
    {
        TextBox,
        TextArea
    }

    public enum SaveType
    {
        String,
        Number,
        Date
    }

    public enum TypePaging
    {
        Client,
        Server
    }
    public enum EditorType
    {
        None,
        TextBox,
        CheckBox,
        Drop,
        DropGrid
    }

    public enum IconType
    {
        NONE,
        ADD,
        NEW,
        DEL,
        INFO,
        OPTION,
        FILE,
        FIND,
        PRINT,
        COPY,
        EXCEL,
        WORD,
        PDF,
        LIST,
        OK,
        EDIT,
        CANCEL,
        HOME,
        PERSON
    }

    public class Base
    {
        //JS
        public const string TEMPLATE_SCRIPT = "<script type=\"text/javascript\" src=\"{0}\"></script>\r\n";
        public const string NAME_COMMON = "Lib.Resources.Common.js";
        public const string NAME_FORM = "Lib.Resources.From.js";
        public const string NAME_JQUERY = "Lib.Resources.jquery.js";
        public const string NAME_JQUERY_UI = "Lib.Resources.Jquery-ui-1.10.4.js";
        public const string NAME_CHECKBOX = " Lib.Resources.checkBox.js";
        public const string NAME_UNTIL = "Lib.Resources.Until.js";
        public const string NAME_EVENT_DRAP = "Lib.Resources.Slick.jquery.event.drag-2.2.js";
        public const string NAME_CELL_SELECTION = "Lib.Resources.Slick.slick.cellselectionmodel.js";
        public const string NAME_SLICK_CORE = "Lib.Resources.Slick.slick.core.js";
        public const string NAME_SLICK_GRID = "Lib.Resources.Slick.slick.grid.js";
        public const string NAME_ROW_SLECTMODE = "Lib.Resources.Slick.slick.rowselectionmodel.js";
        public const string NAME_GRID_EDITOR = "Lib.Resources.Slick.slick.editors.js";
        public const string NAME_JQUERY_EASY = "Lib.Resources.jquery.easyui.min.js";
        public const string NAME_GRID = "Lib.Resources.grid.js";

        //CSS
        public const string NAME_CSS_BASE = "Lib.Style.base.css";
        public const string NAME_CSS_COLOR = "Lib.Style.ColorPicker.css";
        public const string NAME_CSS_SLICK_GRID = "Lib.Style.slick.grid.css";
        public const string NAME_CSS_GRID = "Lib.Style.grid.css";


        ///
        public static string URL_TIMEOUT = "";

        public static bool IsRedirect;

        //excel
        public const string FONT_NAME = "Times New Roman";
        public const short FONT_SIZE = 11;
        public const short DEFAULT_HEIGHT = 300;

        public static string GetImageURL(IconType Icon)
        {
            string imageUrl = "";
            switch (Icon)
            {
                case IconType.ADD:
                    imageUrl = "Lib.Images.add.gif";
                    break;
                case IconType.NEW:
                    imageUrl = "Lib.Images.refesh.png";
                    break;
                case IconType.DEL:
                    imageUrl = "Lib.Images.delete.gif";
                    break;
                case IconType.INFO:
                    imageUrl = "Lib.Images.information.png";
                    break;
                case IconType.OPTION:
                    imageUrl = "Lib.Images.option.png";
                    break;
                case IconType.FILE:
                    imageUrl = "Lib.Images.file.png";
                    break;
                case IconType.FIND:
                    imageUrl = "Lib.Images.find.png";
                    break;
                case IconType.PRINT:
                    imageUrl = "Lib.Images.print.png";
                    break;
                case IconType.COPY:
                    imageUrl = "Lib.Images.copy.png";
                    break;
                case IconType.EXCEL:
                    imageUrl ="Lib.Images.excel.png";
                    break;
                case IconType.WORD:
                    imageUrl = "Lib.Images.word.png";
                    break;
                case IconType.PDF:
                    imageUrl =  "Lib.Images.pdf.png";
                    break;
                case IconType.LIST:
                    imageUrl ="Lib.Images.list.png";
                    break;
                case IconType.OK:
                    imageUrl =  "Lib.Images.ok.png";
                    break;
                case IconType.EDIT:
                    imageUrl = "Lib.Images.edit.png";
                    break;
                case IconType.CANCEL:
                    imageUrl =  "Lib.Images.cancel.png";
                    break;
                case IconType.HOME:
                    imageUrl = "Lib.Images.home.png";
                    break;
                case IconType.PERSON:
                    imageUrl = "Lib.Images.user.ico";
                    break;
            }
            return imageUrl;
        }
    }
}
