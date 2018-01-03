using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using Lib.Helper;
using Lib.Utilities;

namespace Lib.Data
{
    public class DataOra
    {
        private static string _connecString;
        public static string _dbo;
        //private static OracleCommand _Oracommand;


        public string connecString
        {
            get { return _connecString; }
            set { _connecString = value; }
        }
        public string dbo
        {
            get { return _dbo; }
            set { _dbo = value; }
        }
        public static OracleDbType TypeDbOra(char type)
        {
            switch (type)
            {
                case 'D':
                    return OracleDbType.Date;

                case 'I':
                    return OracleDbType.Int32;

                case 'R':
                    return OracleDbType.RefCursor;

                case 'T':
                    return OracleDbType.TimeStamp;

                case 'U':
                    return OracleDbType.NVarchar2;

                case 'N':
                    return OracleDbType.Double;
            }
            return OracleDbType.Varchar2;
        }
        public static OracleConnection GetOracleConnection()
        {
            if (_connecString == "")
                throw new Exception("loi:Chưa khai báo mã kết nối:loi");
            OracleConnection _connecStrong = new OracleConnection(_connecString);
            _connecStrong.Open();
            return _connecStrong;
        }
        /// <summary>
        /// Đưa pramater vào đối tượng Oracle Pramater
        /// </summary>
        /// <param name="oracleCommand">Đối tượng Command</param>
        /// <param name="cusor">Cusor</param>
        /// <param name="type">Loại dữ liệu đưa vào</param>
        public static void AttachParamater(ref OracleCommand oracleCommand, string cusor, OracleDbType type)
        {
            OracleParameter param = new OracleParameter(cusor, type, ParameterDirection.Output);
            if ((type == OracleDbType.Varchar2) || (type == OracleDbType.NVarchar2))
            {
                param.Size = 4000;
            }
            oracleCommand.Parameters.Add(param);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="procedure"></param>
        /// <returns></returns>
        public static DataTable GetOracleData(string procedure)
        {
            DataTable table = new DataTable();
            SessionHelper _se = new SessionHelper();
            if (_se == null) throw new Exception("loi: Mất kết nối. Đăng nhập lại:loi");
            OracleConnection cnn = GetOracleConnection();

            try
            {
                OracleCommand command = new OracleCommand
                {
                    Connection = cnn
                };

                AttachParamater(ref command, "cs1", OracleDbType.RefCursor);
                command.CommandText = "Begin " + _se.dbo + "." + procedure + "(" + _se.tso + ",:cs1); end;";
                try
                {
                    new OracleDataAdapter(command).Fill(table);
                }
                finally
                {
                    command.Parameters.Clear();
                }
            }
            finally
            {
                cnn.Close();
            }

            return table;
        }
        /// <summary>
        /// Gọi thu tục với tham số truyền vào là môt giá trị object
        /// </summary>
        /// <param name="obj">Biến Objet truyền vào</param>
        /// <param name="procedure">Tên thủ tục truyền vào</param>
        /// <returns>Trả về dạng table</returns>
        public static DataTable GetOracleData(object obj, string procedure)
        {
            DataTable table = new DataTable();
            SessionHelper _se = new SessionHelper();
            if (_se == null) throw new Exception("loi: Mất kết nối. Đăng nhập lại:loi");
            OracleConnection cnn = GetOracleConnection();

            try
            {
                OracleCommand command = new OracleCommand
                {
                    Connection = cnn
                };

                AttachParamater(ref command, "cs1", OracleDbType.RefCursor);
                string str = "," + LibConvert.ObjectToChar(obj) + ",:cs1";
                command.CommandText = "Begin " + _se.dbo + "." + procedure + "(" + _se.tso + str + ",:cs1); end;";
                try
                {
                    new OracleDataAdapter(command).Fill(table);
                }
                finally
                {
                    command.Parameters.Clear();
                }
            }
            finally
            {
                cnn.Close();
            }

            return table;
        }
        /// <summary>
        ///gọi thủ tục với tham số truyền vào là mảng Object
        /// </summary>
        /// <param name="obj">Mảng tham số Object truyền vào</param>
        /// <param name="procedure">Tên thủ tục</param>
        /// <returns>Trả lại là kiểu dataTable</returns>
        public static DataTable GetOracleData(object[] obj, string procedure)
        {
            DataTable table = new DataTable();
            SessionHelper _se = new SessionHelper();
            if (_se == null) throw new Exception("loi: Mất kết nối. Đăng nhập lại:loi");
            OracleConnection cnn = GetOracleConnection();

            try
            {
                OracleCommand command = new OracleCommand
                {
                    Connection = cnn
                };

                AttachParamater(ref command, "cs1", OracleDbType.RefCursor);

                string str = "";
                for (int i = 0; i < obj.Length; i++)
                {
                    str = str + "," + LibConvert.ObjectToChar(obj[i]);
                }
                str = str + ",:cs1";
                if (!string.IsNullOrEmpty(_se.tso))
                    str = _se.tso + str;
                command.CommandText = "Begin " + _se.dbo + "." + procedure + "(" + str + "); end;";
                try
                {
                    new OracleDataAdapter(command).Fill(table);
                }
                finally
                {
                    command.Parameters.Clear();
                }
            }
            finally
            {
                cnn.Close();
            }

            return table;
        }
        /// <summary>
        /// Lấy dữ liệu có pramater trả về
        /// </summary>
        /// <param name="TypeDataOutPut">Kiểu dữ liệu trả về</param>
        /// <param name="procedure">Tên thủ tục</param>
        /// <returns>Trả về dạng mảng</returns>
        public static object[] GetOracleDataObject(string TypeDataOutPut, string procedure)
        {
            char[] type = TypeDataOutPut.ToCharArray();
            object[] objectReturn = new object[type.Length];

            SessionHelper se = SessionHelper.GetSessionUser();
            OracleConnection conn = GetOracleConnection();
            OracleCommand command = new OracleCommand
            {
                Connection = conn
            };

            try
            {
                string pramater = "";
                for (int i = 0; i < type.Length; i++)
                {
                    string pare = type[i].ToString().Trim();
                    OracleDbType typeDb = TypeDbOra(type[i]);
                    AttachParamater(ref command, "return" + i.ToString(), typeDb);
                    pramater = pramater + ",:return" + i.ToString();
                }
                command.CommandText = "Begin " + se.dbo + "." + procedure + "(" + se.tso + pramater + "); end;";

                OracleDataAdapter oraAdapter = new OracleDataAdapter(command);
                DataSet set = new DataSet();
                oraAdapter.Fill(set);

                int countCusor = 0;
                for (int j = 0; j < type.Length; j++)
                {
                    switch (type[j])
                    {
                        case 'R':
                            {
                                objectReturn[j] = set.Tables[countCusor];
                                countCusor++;
                                break;
                            }
                        case 'N':
                            objectReturn[j] = command.Parameters["return" + j.ToString().Trim()].Value.ToString();
                            break;
                        default:
                            objectReturn[j] = command.Parameters["return" + j.ToString().Trim()].Value.ToString();
                            break;
                    }
                }
            }
            finally
            {
                command.Parameters.Clear();
                conn.Close();
            }
            return objectReturn;
        }
        /// <summary>
        /// Lấy dữ liệu trả về là mảng Object
        /// </summary>
        /// <param name="obj">Biến truyền vào</param>
        /// <param name="TypeDataOutPut">Kiểu dữ liệu trả về</param>
        /// <param name="procedure">Tên thủ tục</param>
        public static object[] GetOracleDataObject(object obj, string TypeDataOutPut, string procedure)
        {
            char[] type = TypeDataOutPut.ToCharArray();
            object[] objectReturn = new object[type.Length];

            SessionHelper se = SessionHelper.GetSessionUser();
            OracleConnection conn = GetOracleConnection();
            OracleCommand command = new OracleCommand
            {
                Connection = conn
            };

            try
            {
                string pramater = "";
                pramater = "," + LibConvert.ObjectToChar(obj);

                for (int i = 0; i < type.Length; i++)
                {
                    string pare = type[i].ToString().Trim();
                    OracleDbType typeDb = TypeDbOra(type[i]);
                    AttachParamater(ref command, "return" + i.ToString(), typeDb);
                    pramater = pramater + ",:return" + i.ToString();
                }
                command.CommandText = "Begin " + se.dbo + "." + procedure + "(" + se.tso + pramater + "); end;";

                OracleDataAdapter oraAdapter = new OracleDataAdapter(command);
                DataSet set = new DataSet();
                oraAdapter.Fill(set);

                int countCusor = 0;
                for (int j = 0; j < type.Length; j++)
                {
                    switch (type[j])
                    {
                        case 'R':
                            {
                                objectReturn[j] = set.Tables[countCusor];
                                countCusor++;
                                break;
                            }
                        case 'N':
                            objectReturn[j] = command.Parameters["return" + j.ToString().Trim()].Value.ToString();
                            break;
                        default:
                            objectReturn[j] = command.Parameters["return" + j.ToString().Trim()].Value.ToString();
                            break;
                    }
                }
            }
            finally
            {
                command.Parameters.Clear();
                conn.Close();
            }
            return objectReturn;
        }
        /// <summary>
        /// Tra dữ liệu dạng mảng Object
        /// </summary>
        /// <param name="obj">Mảng truyền vào</param>
        /// <param name="TypeDataOutPut">Kiểu dữ liệu trả về</param>
        /// <param name="procedure">Tên thủ tục</param>
        public static object[] GetOracleDataObject(object[] obj, string TypeDataOutPut, string procedure)
        {
            char[] type = TypeDataOutPut.ToCharArray();
            object[] objectReturn = new object[type.Length];

            SessionHelper se = SessionHelper.GetSessionUser();
            OracleConnection conn = GetOracleConnection();
            OracleCommand command = new OracleCommand
            {
                Connection = conn
            };

            try
            {
                string pramater = "";
                for (int k = 0; k < obj.Length; k++)
                    pramater = pramater + "," + LibConvert.ObjectToChar(obj[k]);

                for (int i = 0; i < type.Length; i++)
                {
                    string pare = type[i].ToString().Trim();
                    OracleDbType typeDb = TypeDbOra(type[i]);
                    AttachParamater(ref command, "return" + i.ToString(), typeDb);
                    pramater = pramater + ",:return" + i.ToString();
                }
                command.CommandText = "Begin " + se.dbo + "." + procedure + "(" + se.tso + pramater + "); end;";

                OracleDataAdapter oraAdapter = new OracleDataAdapter(command);
                DataSet set = new DataSet();
                oraAdapter.Fill(set);

                int countCusor = 0;
                for (int j = 0; j < type.Length; j++)
                {
                    switch (type[j])
                    {
                        case 'R':
                            {
                                objectReturn[j] = set.Tables[countCusor];
                                countCusor++;
                                break;
                            }
                        case 'N':
                            objectReturn[j] = command.Parameters["return" + j.ToString().Trim()].Value.ToString();
                            break;
                        default:
                            objectReturn[j] = command.Parameters["return" + j.ToString().Trim()].Value.ToString();
                            break;
                    }
                }
            }
            finally
            {
                command.Parameters.Clear();
                conn.Close();
            }
            return objectReturn;
        }
        /// <summary>
        /// Thực thi thủ tục
        /// </summary>
        /// <param name="procedure">Tên thủ tục</param>
        public static void Execute(string procedure)
        {
            SessionHelper se = SessionHelper.GetSessionUser();
            OracleConnection conn = GetOracleConnection();

            try
            {
                OracleCommand command = new OracleCommand
                {
                    Connection = conn
                };
                command.CommandText = "Begin " + se.dbo + "." + procedure + "(" + se.tso + "); end;";
                command.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }
        }
        /// <summary>
        /// thực thi một thủ tục với tham số truyền vào là bject
        /// </summary>
        /// <param name="obj">Tham số truyền vào</param>
        /// <param name="procedure">Tên thủ tục</param>
        public static void Execute(object obj, string procedure)
        {
            SessionHelper se = SessionHelper.GetSessionUser();
            OracleConnection conn = GetOracleConnection();

            try
            {
                OracleCommand command = new OracleCommand
                {
                    Connection = conn
                };
                string str = "," + LibConvert.ObjectToChar(obj);
                command.CommandText = "Begin " + se.dbo + "." + procedure + "(" + se.tso + str + "); end;";
                command.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }
        }
        /// <summary>
        /// Thực thi thủ tục với tham số truyền vào là một mảng object
        /// </summary>
        /// <param name="obj">Mảng truyền vào</param>
        /// <param name="procedure">Tên thủ tục</param>
        public static void Execute(object[] obj, string procedure)
        {
            SessionHelper se = SessionHelper.GetSessionUser();
            OracleConnection conn = GetOracleConnection();

            try
            {
                OracleCommand command = new OracleCommand
                {
                    Connection = conn
                };
                string str = "";
                for (int i = 0; i < obj.Length; i++)
                {
                    str = str + "," + LibConvert.ObjectToChar(obj[i]);
                }
                command.CommandText = "Begin " + se.dbo + "." + procedure + "(" + se.tso + str + "); end;";
                command.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }
        }

    }
}
