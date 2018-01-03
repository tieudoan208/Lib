using System.Web;
using System;

namespace Lib.Helper
{
    public class SessionHelper
    {
        public string ma_dvi, nsd, dbo, pas, ten, cap, ten_dvi, dchi_dvi, ma_thue, ten_gd, ten_ktt, ten_ct, phong, tso, md, vp, modun, cn, id_nsd, ns_id;
        public SessionHelper()
        {
            object b_obj = HttpContext.Current.Session["nsd"];
            if (b_obj != null)
            {
                SessionHelper b_se_nsd = (SessionHelper)b_obj;
                this.ma_dvi = b_se_nsd.ma_dvi; this.nsd = b_se_nsd.nsd; this.dbo = b_se_nsd.dbo;
                this.pas = b_se_nsd.pas; this.ten = b_se_nsd.ten; this.cap = b_se_nsd.cap; this.md = b_se_nsd.md;
                this.ten_dvi = b_se_nsd.ten_dvi; this.dchi_dvi = b_se_nsd.dchi_dvi; this.ma_thue = b_se_nsd.ma_thue;
                this.ten_gd = b_se_nsd.ten_gd; this.ten_ktt = b_se_nsd.ten_ktt; this.ten_ct = b_se_nsd.ten_ct;
                this.phong = b_se_nsd.phong; this.tso = b_se_nsd.tso;
                this.vp = b_se_nsd.vp;
                this.modun = b_se_nsd.modun; this.cn = b_se_nsd.cn;
                this.id_nsd = b_se_nsd.id_nsd;
                this.ns_id = b_se_nsd.ns_id;
            }
            else this.nsd = "";
        }
        /// <summary>
        /// Xóa session
        /// </summary>
        /// <param name="sessionName">Tên session xóa</param>
        public static void RemoveSession(string sessionName)
        {
            HttpContext.Current.Session.Remove(sessionName);
        }
        /// <summary>
        /// 
        /// </summary>
        public static SessionHelper GetSessionUser()
        {
            object b_obj = HttpContext.Current.Session["nsd"];
            if (b_obj == null)
                throw new Exception("Lỗi kết nối. Đăng nhập lại");
            return (SessionHelper)b_obj;
        }
        public static object GetSession(string nameSession)
        {
            return HttpContext.Current.Session[nameSession];
        }
        public static void SaveSession(object se, string nameSession)
        {
            HttpContext.Current.Session[nameSession]=se;
        }
    }
}
