using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using Lib.Utilities;

namespace Lib.Data
{
    public class DataSql
    {
        static string _connecString;
        static string _store;
        static SqlParameter[] _pramater;
        static SqlCommand _command;


        public static string ConnecString { get { return _connecString; } set { _connecString = value; } }
        public SqlParameter[] Pramater
        {
            get { return _pramater; }
            set { _pramater = value; }
        }
        public SqlCommand Command
        {
            get { return _command; }
            set { _command = value; }
        }

        public static SqlConnection GetConnection()
        {
            try
            {
                SqlConnection cnn = new SqlConnection(_connecString);
                cnn.Open();
                return cnn;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
        public static SqlConnection GetConnection(string connec)
        {
            try
            {
                SqlConnection cnn = new SqlConnection(connec);
                _connecString = connec;
                cnn.Open();
                return cnn;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }

        }
        // Kiểm tra chuỗi kết nối
        public static bool CheckConnec(string b_ddan)
        {
            try
            {
                SqlConnection cnn = new SqlConnection(b_ddan);
                if (cnn.State == ConnectionState.Open)
                    return true;
                else return false;
            }
            catch (Exception) { return false; }
        }
        public static SqlParameter[] getPramater(SqlCommand sqlcommand, bool getValue)
        {
            SqlCommandBuilder.DeriveParameters(sqlcommand);
            if (!getValue)
            {
                sqlcommand.Parameters.RemoveAt(0);
            }
            SqlParameter[] _paramter = new SqlParameter[sqlcommand.Parameters.Count];
            sqlcommand.Parameters.CopyTo(_paramter, 0);

            return _paramter;
        }
        public static SqlParameter[] getPramater(SqlConnection sqlconnec, string b_procedure, bool getValue)
        {
            SqlCommand sqlcommand = new SqlCommand(b_procedure, sqlconnec);
            sqlcommand.CommandType = CommandType.StoredProcedure;

            return getPramater(sqlcommand, getValue);
        }
        public static void AttachParamater(SqlParameter[] paramater, object values)
        {
            if (values == null) throw new Exception("Lỗi truyền giá trị");
            for (int i = 0; i < paramater.Length; i++)
            {
                if (i == 0)
                {
                    if (values is IDbDataParameter)
                    {
                        IDbDataParameter paramInstance = (IDbDataParameter)values;
                        if (paramInstance.Value == null)
                        {
                            paramater[i].Value = DBNull.Value;
                        }
                        else
                        {
                            paramater[i].Value = paramInstance.Value;
                        }
                    }
                    else if (values == null)
                    {
                        paramater[i].Value = DBNull.Value;
                    }
                    else
                    {
                        if (paramater[i].TypeName != "")
                        {
                            int startIndex = paramater[i].TypeName.IndexOf("dbo");
                            string typeName = paramater[i].TypeName.Substring(startIndex);
                            paramater[i].TypeName = typeName;
                        }
                        paramater[i].Value = values;
                    }
                }
                else
                    paramater[i].Value = DBNull.Value;
            }
        }

        public static void AttachParamater(SqlParameter[] paramater, object[] values)
        {
            if (values == null) throw new Exception("Lỗi truyền giá trị");
            for (int i = 0; i < paramater.Length; i++)
            {
                if (values[i] is IDbDataParameter)
                {
                    IDbDataParameter paramInstance = (IDbDataParameter)values[i];
                    if (paramInstance.Value == null)
                    {
                        paramater[i].Value = DBNull.Value;
                    }
                    else
                    {
                        paramater[i].Value = paramInstance.Value;
                    }
                }
                else if (values[i] == null)
                {
                    paramater[i].Value = DBNull.Value;
                }
                else
                {
                    if (paramater[i].TypeName != "")
                    {
                        int startIndex = paramater[i].TypeName.IndexOf("dbo");
                        string typeName = paramater[i].TypeName.Substring(startIndex);
                        paramater[i].TypeName = typeName;
                    }
                    paramater[i].Value = values[i];
                }
            }
        }
        /// <summary>
        /// Thực thi thủ tục có giá trị trả về
        /// </summary>
        /// <param name="paramater">Mảng trả vê</param>
        /// <param name="b_return">Giá trị</param>
        /// <param name="values"></param>
        public static void AttachParamater(SqlParameter[] paramater, object b_return, object[] values)
        {
            if (values == null) throw new Exception("Lỗi truyền giá trị");
            for (int i = 0; i < paramater.Length; i++)
            {
                if (values[i] is IDbDataParameter)
                {
                    IDbDataParameter paramInstance = (IDbDataParameter)values[i];
                    if (paramInstance.Value == null)
                    {
                        paramater[i].Value = DBNull.Value;
                    }
                    else
                    {
                        paramater[i].Value = paramInstance.Value;
                    }
                }
                else if (values[i] == null)
                {
                    paramater[i].Value = DBNull.Value;
                }
                else
                {
                    if (paramater[i].TypeName != "")
                    {
                        int startIndex = paramater[i].TypeName.IndexOf("dbo");
                        string typeName = paramater[i].TypeName.Substring(startIndex);
                        paramater[i].TypeName = typeName;
                    }
                    paramater[i].Value = values[i];
                    if (b_return.ToString().IndexOf(paramater[i].ParameterName.ToString()) >= 0)
                    {
                        paramater[i].Direction = ParameterDirection.InputOutput;
                    }

                }
            }
        }

        //Return SqlCommand
        public static SqlCommand CreateCommand(SqlConnection cn, string procedureName)
        {
            _command = new SqlCommand();
            _command.Connection = cn;
            _command.CommandType = CommandType.StoredProcedure;
            _command.CommandText = procedureName;
            return _command;
        }

        public static SqlCommand CreateCommand(SqlConnection cn, object values, string procedure)
        {
            _command = new SqlCommand(procedure, cn);
            _command.CommandType = CommandType.StoredProcedure;

            SqlParameter[] _paramater = getPramater(_command, false);
            AttachParamater(_paramater, values);
            return _command;
        }

        public static SqlCommand CreateCommand(SqlConnection cn, object[] values, string procedure)
        {
            _command = new SqlCommand(procedure, cn);
            _command.CommandType = CommandType.StoredProcedure;

            SqlParameter[] _paramater = getPramater(_command, false);

            AttachParamater(_paramater, values);
            return _command;
        }

        public static SqlCommand CreateCommand(SqlConnection cn, object[] values, object b_return, string procedure)
        {
            _command = new SqlCommand(procedure, cn);
            _command.CommandType = CommandType.StoredProcedure;

            SqlParameter[] _paramater = getPramater(_command, false);

            AttachParamater(_paramater, b_return, values);
            return _command;
        }

        //Liệt kê ko tham số 
        public static DataTable GetData(string procedureName)
        {
            DataTable b_dt = new DataTable();
            SqlConnection cn = GetConnection();
            SqlCommand cm = CreateCommand(cn, procedureName);
            SqlDataAdapter ad = new SqlDataAdapter(cm);
            ad.Fill(b_dt);
            cn.Close();
            return b_dt;
        }
        public static DataTable GetData(object value, string procedureName)
        {
            DataTable b_dt = new DataTable();
            SqlConnection cn = GetConnection();
            SqlCommand cm = CreateCommand(cn, value, procedureName);

            SqlDataAdapter ad = new SqlDataAdapter(cm);
            ad.Fill(b_dt);
            cm.Clone();
            cn.Close();
            return b_dt;
        }
        //lấy dữ liệu trả vè là object. Datatable và giá trị trả về
        public static object[] GetData(object[] value, object b_return, string procedureName)
        {
            DataTable b_dt = new DataTable();
            SqlConnection cn = GetConnection();
            SqlCommand cm = CreateCommand(cn, value, b_return, procedureName);

            SqlDataAdapter ad = new SqlDataAdapter(cm);
            ad.Fill(b_dt);
            object[] a_value = new object[2];
            a_value[0] = (object)b_dt;
            a_value[1] = cm.Parameters[b_return.ToString()].Value;
            cm.Clone();
            cn.Close();
            return a_value;
        }
        public static DataTable GetData(object[] value, string procedureName)
        {
            DataTable b_dt = new DataTable();
            SqlConnection cn = GetConnection();
            SqlCommand cm = CreateCommand(cn, value, procedureName);

            SqlDataAdapter ad = new SqlDataAdapter(cm);
            ad.Fill(b_dt);
            cm.Parameters.Clear();
            cm.Clone();
            cn.Close();
            return b_dt;
        }
        /// <summary>
        /// Trả lại nhiều con trỏ data
        /// </summary>
        /// <param name="procedureName">Tên thủ tục</param>
        public static DataSet GetMutilData(string procedureName)
        {
            DataSet b_ds = new DataSet();
            SqlConnection cn = GetConnection();
            SqlCommand cm = CreateCommand(cn, procedureName);

            SqlDataAdapter ad = new SqlDataAdapter(cm);
            ad.Fill(b_ds);
            cm.Parameters.Clear();
            cm.Clone();
            cn.Close();
            return b_ds;
        }
        /// <summary>
        /// Trả lại nhiều con trỏ data
        /// </summary>
        /// <param name="value">Mảng giá trị</param>
        /// <param name="procedureName">Tên thủ tục</param>
        public static DataSet GetMutilData(object[] value, string procedureName)
        {
            DataSet b_ds = new DataSet();
            SqlConnection cn = GetConnection();
            SqlCommand cm = CreateCommand(cn, value, procedureName);

            SqlDataAdapter ad = new SqlDataAdapter(cm);
            ad.Fill(b_ds);
            cm.Parameters.Clear();
            cm.Clone();
            cn.Close();
            return b_ds;
        }
        /// <summary>
        /// thực thi một thủ tục truyên vào
        /// </summary>
        /// <param name="pramater">các Parameter</param>
        /// <param name="b_value">Giá trị</param>
        /// <param name="storeName">Tên thủ tục</param>
        public static void ExecuteProcedure(object b_value, string storeName)
        {
            SqlConnection cn = GetConnection();
            SqlCommand cm = CreateCommand(cn, b_value, storeName);
            cm.ExecuteNonQuery();
            cm.Parameters.Clear();
            cm.Clone();
            cn.Close();
        }
        /// <summary>
        /// thực thi một thủ tục truyên vào
        /// </summary>
        /// <param name="pramater">Mảng các Parameter</param>
        /// <param name="b_value">Mảng các giá trị</param>
        /// <param name="storeName">Tên thủ tục</param>
        public static void ExecuteProcedure(object[] b_value, string storeName)
        {
            SqlConnection cn = GetConnection();
            SqlCommand cm = CreateCommand(cn, b_value, storeName);
            cm.ExecuteNonQuery();
            cm.Parameters.Clear();
            cm.Clone();
            cn.Close();
        }
        /// <summary>
        /// Thực thi thủ tục có dãy tham số và có một giá trị trả về
        /// </summary>
        /// <param name="b_value">Mảng giá trị</param>
        /// <param name="b_return">Biến cần trả về</param>
        /// <param name="storeName"></param>
        public static object ExecuteProcedure(object[] b_value, object b_return, string storeName)
        {
            SqlConnection cn = GetConnection();
            SqlCommand cm = CreateCommand(cn, b_value, b_return, storeName);
            cm.ExecuteNonQuery();
            object objValue = new object[1];
            objValue = cm.Parameters[b_return.ToString()].Value;
            cm.Parameters.Clear();
            cm.Clone();
            cn.Close();
            return objValue;
        }

        //Cui: Nhap mang
        public static void ExecuteProcedure(DataTable b_dt, string storeName)
        {
            SqlConnection cn = GetConnection();
            for (int i = 0; i < b_dt.Rows.Count; i++)
            {
                object[] a_pra = new object[b_dt.Columns.Count];
                for (int j = 0; j < b_dt.Columns.Count; j++)
                {
                    a_pra[j] = LibConvert.ObjectToString(b_dt.Rows[i][j]);
                }
                SqlCommand cm = CreateCommand(cn, a_pra, storeName);
                cm.ExecuteNonQuery();
                cm.Parameters.Clear();
                cm.Clone();
            }
            cn.Close();
        }

        //GỌI FUNTION
        /// <summary>
        /// THực thi fuction
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static object ExecuteScalar(object[] values, string functionName)
        {
            SqlConnection cn = GetConnection();
            SqlCommand cmd = new SqlCommand();
            SqlCommand cm = CreateCommand(cn, values, functionName);
            object retval = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return retval;
        }
        
    }
}
