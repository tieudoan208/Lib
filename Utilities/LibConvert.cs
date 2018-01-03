
using System;

namespace Lib.Utilities
{
    public class LibConvert
    {
        //public static string CH_CSO(string b_so, int b_tp)
        //{
        //    try
        //    {
        //        return SO_CSO(double.Parse(b_so), b_tp);
        //    }
        //    catch
        //    {
        //        return "0";
        //    }
        //}


        /// <summary>
        /// Chuyển một chuỗi dạng ngày sang chuỗi dạng số dang yyyyMMdd
        /// </summary>
        /// <param name="stringDate">Chuỗi ngày</param>
        /// <returns></returns>
        public static string SDateToSNumber(string stringDate)
        {
            try
            {
                string[] date = stringDate.Split('/');
                string sdate = (date[0].Length == 0 ? "0" + date[0] : date[0]);
                string sMonth = (date[1].Length == 0 ? "0" + date[1] : date[1]);
                return (stringDate.Substring(6, 4) + sMonth + sdate);
            }
            catch
            {
                return "30000101";
            }
        }
        /// <summary>
        /// Chuyển một chuỗi từ ngày sang ngày
        /// </summary>
        /// <param name="stringDate">Chuỗi cần chuyển</param>
        public static DateTime SDateToDate(string stringDate)
        {
            DateTime time = new DateTime(0xbb8, 1, 1);
            DateTime now = DateTime.Now;
            try
            {
                if (string.IsNullOrEmpty(stringDate))
                    return time;
                int index = 0;
                int startIndex = 3;
                int num3 = 6;
                num3 = int.Parse(stringDate.Substring(num3, 4));
                startIndex = int.Parse(stringDate.Substring(startIndex, 2));
                if (startIndex < 1)
                {
                    startIndex = 1;
                }
                else if (startIndex > 12)
                {
                    startIndex = 12;
                }
                index = int.Parse(stringDate.Substring(index, 2));
                if (index < 1)
                {
                    index = 1;
                }
                else
                {
                    int num4 = DateTime.DaysInMonth(num3, startIndex);
                    if (index > num4)
                    {
                        index = num4;
                    }
                }
                now = new DateTime(num3, startIndex, index);
                if (now > time)
                {
                    return time;
                }
            }
            catch
            {
                return time;
            }
            return now;
        }

        public static string DateToSDate(DateTime b_ngay)
        {
            return ((b_ngay.Year >= 3000) ? "" : b_ngay.ToString("dd/MM/yyyy"));
        }

        /// <summary>
        /// chuyển ngày sang chuỗi tháng
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string DateToSMonth(DateTime date)
        {
            return date.ToString("MM/yyyy");
        }
        public static int SDateToNumber(string b_ngay)
        {
            try
            {
                return int.Parse(SDateToSNumber(b_ngay));
            }
            catch
            {
                return 0x1c9c3e5;
            }
        }
        public static int SMonthToNumber(string thang)
        {
            try
            {
                string[] date = thang.Split('/');
                string sMonth = (date[0].Length == 0 ? "0" + date[0] : date[0]);
                return  ObjectToInt(date[1] + sMonth);
            }
            catch
            {
                return 30000101;
            }
        }
        public static string ObjectToString(object Val)
        {
            string outVal = Until.NVL(Val.ToString());
            return outVal;
        }
        public static DateTime ObjecToDate(object b_obj)
        {
            if ((b_obj != null) && !(b_obj is DBNull))
            {
                return (DateTime)b_obj;
            }
            return new DateTime(3000, 1, 1);
        }
        public static string ObjectToChar(object obj)
        {
            string str = "";
            if (obj is DBNull)
            {
                return "''";
            }
            if (obj is DateTime)
            {
                return ("'" + ObjecToDate(obj).ToString("dd-MMM-yyyy") + "'");
            }
            if (obj is bool)
            {
                return (ObjectToBool(obj) ? "true" : "false");
            }
            if (obj is string)
            {
                str = obj.ToString();
                if ((str.Length > 1) && (str.Substring(0, 2) == "N'"))
                {
                    return ("N'" + str.Substring(2).Replace("'", "") + "'");
                }
                str = str.Replace("'", "");
                return ("'" + str + "'");
            }
            return ObjectToDouble(obj).ToString();
        }
        public static bool ObjectToBool(object b_obj)
        {
            return (((b_obj != null) && !(b_obj is DBNull)) && (b_obj.ToString().ToUpper() == "TRUE"));
        }

        public static double ObjectToDouble(object b_obj)
        {
            if ((b_obj != null) && !(b_obj is DBNull))
            {
                return double.Parse(Until.NVL(b_obj.ToString(), "0"));
            }
            return 0.0;
        }
        public static float ObjectToFloat(object b_obj)
        {
            if ((b_obj != null) && !(b_obj is DBNull))
            {
                return float.Parse(Until.NVL(b_obj.ToString(), "0"));
            }
            return 0;
        }

        public static int ObjectToInt (object obj)
        {
            return int.Parse(obj.ToString());
        }

        public static string[] ArrObjectToArrString(object[] arrObj)
        {
            string[] arrStr = new string[arrObj.Length];
            for (int i = 0; i < arrObj.Length; i++)
            {
                arrStr[i] = ObjectToString(arrObj[i]);
            }
            return arrStr;
        }

        /*
       public static string CSO_CH(string b_chu)
       {
           string str = "";
           try
           {
               str = kytu.C_NVL(b_chu).Replace(",", "").Replace(".", ".");
               if (((str.Length != 0) && !(str == "-")) && (!(str == ".") && !(str == "-.")))
               {
                   return str;
               }
               return "0";
           }
           catch
           {
               return "0";
           }
       }

       public static string CSO_CNG(string b_so)
       {
           string str = new DateTime(0xbb8, 1, 1).ToString("dd/MM/yyyy");
           try
           {
               if (b_so == null)
               {
                   return "";
               }
               str = "dd/MM/yyyy";
               return str.Replace("dd", b_so.Substring(6, 2)).Replace("MM", b_so.Substring(4, 2)).Replace("yyyy", b_so.Substring(0, 4));
           }
           catch
           {
               DateTime time2 = new DateTime(0xbb8, 1, 1);
               return time2.ToString("dd/MM/yyyy");
           }
       }

       public static string CSO_CTH(string b_so)
       {
           string str = new DateTime(0xbb8, 1, 1).ToString("MM/yyyy");
           try
           {
               if (b_so == null)
               {
                   return "";
               }
               str = "MM/yyyy";
               return str.Replace("MM", b_so.Substring(4, 2)).Replace("yyyy", b_so.Substring(0, 4));
           }
           catch
           {
               DateTime time2 = new DateTime(0xbb8, 1, 1);
               return time2.ToString("MM/yyyy");
           }
       }

       public static double CSO_SO(string b_chu)
       {
           try
           {
               return double.Parse(CSO_CH(b_chu));
           }
           catch
           {
               return 0.0;
           }
       }

       public static string CTH_CSO(string b_thang)
       {
           string str = "300001";
           if (khac.Fb_NGAY_TRANG(b_thang))
           {
               return str;
           }
           try
           {
               string str2 = "MM/yyyy";
               int index = str2.IndexOf("M");
               int startIndex = str2.IndexOf("y");
               return (b_thang.Substring(startIndex, 4) + b_thang.Substring(index, 2));
           }
           catch
           {
               return "300001";
           }
       }

       public static int CTH_SO(string b_thang)
       {
           try
           {
               return int.Parse(CTH_CSO(b_thang));
           }
           catch
           {
               return 0x493e1;
           }
       }

       public static string CTH_TRANG()
       {
           string str = "MM/yyyy";
           return str.Replace("M", " ").Replace("y", " ");
       }


       public static object[] Fobj_OBJ(bool[] a_goc)
       {
           object[] objArray = new object[a_goc.Length];
           for (int i = 0; i < a_goc.Length; i++)
           {
               objArray[i] = a_goc[i];
           }
           return objArray;
       }

       public static object[] Fobj_OBJ(DateTime[] a_goc)
       {
           object[] objArray = new object[a_goc.Length];
           for (int i = 0; i < a_goc.Length; i++)
           {
               objArray[i] = a_goc[i];
           }
           return objArray;
       }

       public static object[] Fobj_OBJ(double[] a_goc)
       {
           object[] objArray = new object[a_goc.Length];
           for (int i = 0; i < a_goc.Length; i++)
           {
               objArray[i] = a_goc[i];
           }
           return objArray;
       }

       public static object[] Fobj_OBJ(int[] a_goc)
       {
           object[] objArray = new object[a_goc.Length];
           for (int i = 0; i < a_goc.Length; i++)
           {
               objArray[i] = a_goc[i];
           }
           return objArray;
       }

       public static object[] Fobj_OBJ(string[] a_goc)
       {
           object[] objArray = new object[a_goc.Length];
           for (int i = 0; i < a_goc.Length; i++)
           {
               objArray[i] = a_goc[i];
           }
           return objArray;
       }


       public static string NG_CTH(DateTime b_ngay)
       {
           return ((b_ngay.Year >= 0xbb8) ? CTH_TRANG() : b_ngay.ToString("MM/yyyy"));
       }

       public static string NG_NGC(DateTime b_ngay)
       {
           if (b_ngay.Year >= 0xbb8)
           {
               b_ngay = new DateTime(0xbb8, 1, 1);
           }
           return b_ngay.ToString("dd-MMM-yyyy");
       }

       public static int NG_SO(DateTime d_ngay)
       {
           return int.Parse(d_ngay.ToString("yyyyMMdd"));
       }

      

      

       public static string OBJ_C(object b_obj, string b_gtri, string b_moi)
       {
           string str = OBJ_C(b_obj);
           if (str == b_gtri)
           {
               str = b_moi;
           }
           return str;
       }

      


      
       public static string OBJ_S(object b_obj)
       {
           if (b_obj != null)
           {
               return kytu.C_NVL(b_obj.ToString());
           }
           return "";
       }

       public static string OBJ_S(object b_obj, string b_out)
       {
           if (b_obj != null)
           {
               return kytu.C_NVL(b_obj.ToString(), b_out);
           }
           return b_out;
       }

       public static string SO_CNG(int b_so)
       {
           if (b_so >= 0x1c9c380)
           {
               return CNG_TRANG();
           }
           return CSO_CNG(b_so.ToString());
       }

       public static string SO_CSO(double b_so, int b_tp)
       {
           string str = "0";
           try
           {
               string format = "###,###,###,###,###,##0".Replace(",", ",");
               if (b_tp > 0)
               {
                   format = format + "." + new string('#', b_tp);
               }
               str = b_so.ToString(format);
               str.Replace('.', 'z');
               str.Replace(",", ",");
               str.Replace("z", ".");
               if (str.Length == 0)
               {
                   str = "0";
               }
           }
           catch
           {
               str = "0";
           }
           return str;
       }

       public static string SO_CTH(int b_so)
       {
           if (b_so >= 0x493e0)
           {
               return CTH_TRANG();
           }
           return CSO_CTH(b_so.ToString());
       }

       public static DateTime SO_NG(int b_so)
       {
           return CNG_NG(SO_CNG(b_so));
       }

       public static int TH_SO(DateTime b_ngay)
       {
           return int.Parse(b_ngay.ToString("yyyyMM"));
       }
        */
    }
}