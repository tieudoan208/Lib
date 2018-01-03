using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Constant;

namespace Lib.Controls
{
    public class ItemTemplate
    {
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
        /// Kiểu dữ liệu
        /// </summary>
        public DataType DataType
        {
            get;
            set;
        }
    }
}
