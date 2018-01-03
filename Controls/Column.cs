using Lib.Constant;
using System.Web.UI;
using System.Collections.Generic;

namespace Lib.Controls
{
    public class Column
    {
        private List<ItemTemplate> _itemTemplate;
        private List<Frame> _footerTemplate;

        public Column() : base() { ;}
        /// <summary>
        /// Tên hiện thị cột
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// ID cột
        /// </summary>
        public string BaseColumn
        {
            get;
            set;
        }
        /// <summary>
        /// Chiều cao của Cột
        /// </summary>
        public int Width
        {
            get;
            set;
        }
        /// <summary>
        /// Gán css vào từ Cell
        /// </summary>
        public string CssClass
        {
            get;
            set;
        }
        /// <summary>
        /// Ẩn cột hay không
        /// </summary>
        public bool Hidden
        {
            get;
            set;
        }

        /// <summary>
        /// Loại Edit control
        /// </summary>
        public EditorType EditControlType
        {
            get;
            set;
        }

        /// <summary>
        /// Tham khảo tới đường Link
        /// </summary>
        public string UrlReference
        {
            get;
            set;
        }

        /// <summary>
        /// Kiểu dữ liệu
        /// </summary>
        public DataType DataType
        {
            get;
            set;
        }

        /// <summary>
        /// Liệt kê danh sách
        /// </summary>
        public string lke
        {
            get;
            set;
        }

        /// <summary>
        /// Độ dài chuỗi nhập
        /// </summary>
        public int MaxLength
        {
            get;
            set;
        }

        /// <summary>
        /// Viết hoa hay không
        /// </summary>
        public bool UpperCase
        {
            get;
            set;
        }

        /// <summary>
        /// Xác định phần thập phân
        /// </summary>
        public int Dec
        {
            get;
            set;
        }

        public string HeaderTemplate
        {
            get;
            set;
        }
        public string Source
        {
            get;
            set;
        }
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public List<ItemTemplate> ItemTemplate
        {
            get
            {
                if (this._itemTemplate == null)
                    this._itemTemplate = new List<ItemTemplate>();
                return this._itemTemplate;
            }
        }
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public List<Frame> FooterTemplate
        {
            get
            {
                if (this._footerTemplate == null)
                    this._footerTemplate = new List<Frame>();
                return this._footerTemplate;
            }
        }
        //EditCell hay khong
        public bool IsEditCell
        {
            get;
            set;
        }
        //Hiện ảnh
        public IconType Icon
        {
            get;
            set;
        }

    }
}
