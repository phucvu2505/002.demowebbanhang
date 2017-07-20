using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Data;
using System.Configuration;
using System.Text.RegularExpressions;
using System.IO;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.Data.Common;
using System.Web.UI;
using System.Web.Security;
using System.Text;
using System.Security.Cryptography;
using System.Globalization;
using System.Collections.Specialized;
using System.Net;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Web.Caching;
using System.Collections;
using Newtonsoft.Json;

/// <summary>
/// Summary description for OsPortal
/// </summary>

namespace OsPortal
{
    #region CLASS O_DATA_HELPER
    public class oDataHelper
    {
        // Fields
        public static DateTime DateTimeMinValue = DateTime.MinValue;
        public static int IntMinValue = -2147483648;

        // Methods
        protected void AddSqlParameter(DbCommand dbCommand, SqlParameter[] parameters)
        {
            if (parameters != null)
            {
                dbCommand.Parameters.Clear();
                foreach (SqlParameter parameter in parameters)
                {
                    if ((parameter.Value != null) && (parameter.Value != DBNull.Value))
                    {
                        switch (parameter.DbType)
                        {
                            case DbType.AnsiString:
                            case DbType.String:
                                {
                                    dbCommand.Parameters.Add(new SqlParameter(parameter.ParameterName, EscapeQuoteReg(parameter.Value.ToString())));
                                    continue;
                                }
                        }
                        dbCommand.Parameters.Add(new SqlParameter(parameter.ParameterName, parameter.Value));
                    }
                }
            }
        }

        protected void AddSqlParameter(ParameterCollection ParamCol, SqlParameter[] parameters)
        {
            if (parameters != null)
            {
                ParamCol.Clear();
                foreach (SqlParameter parameter in parameters)
                {
                    if ((parameter.Value != null) && (parameter.Value != DBNull.Value))
                    {
                        switch (parameter.DbType)
                        {
                            case DbType.AnsiString:
                            case DbType.String:
                                {
                                    ParamCol.Add(parameter.ParameterName, EscapeQuoteReg(parameter.Value.ToString()));
                                    continue;
                                }
                        }
                        ParamCol.Add(parameter.ParameterName, parameter.Value.ToString());
                    }
                }
            }
        }

        public static string AppSettingsWebConfig(string strKey)
        {
            string str = ConfigurationManager.AppSettings[strKey];
            if ((str == null) || (str.Length == 0))
            {
                return string.Empty;
            }
            return str.Trim();
        }

        public static string ConnectionStringWebConfig(string strKey)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[strKey].ConnectionString;
            if ((connectionString == null) || (connectionString.Length == 0))
            {
                return string.Empty;
            }
            return connectionString.Trim();
        }

        public static string EscapeName(string S)
        {
            if (S.IndexOfAny(new char[] { '[', ']', '*', '.', ' ' }) != -1)
            {
                return S;
            }
            return string.Format("[{0}]", S);
        }

        public static string EscapeQuoteReg(string S)
        {
            return Regex.Replace(S.Replace('\'', '"'), @"(?:delete|select|drop|create|xp_)\s", string.Empty, RegexOptions.IgnoreCase);
        }

        public static DataSet GetDataSetFromRss(string Path)
        {
            DataSet set2;
            try
            {
                using (DataSet set = new DataSet())
                {
                    using (Stream stream = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (XmlTextReader reader = new XmlTextReader(stream))
                        {
                            set.ReadXml(reader);
                            set2 = set;
                        }
                    }
                }
            }
            catch
            {
                set2 = null;
            }
            return set2;
        }

        public static DataTable GetDataTableFromRssCache(string Path)
        {
            DataTable table = GetDataSetFromRss(Path).Tables[0];
            if (table.Rows.Count > 0)
            {
                return table;
            }
            return null;
        }

        public static string InsertQuote(string S)
        {
            return string.Format("'{0}'", EscapeQuoteReg(S));
        }

        public static bool isnull(object obValue)
        {
            return ((obValue == null) || (obValue == DBNull.Value));
        }

        public static int IsNull(object obValue, int intDefault)
        {
            return (isnull(obValue) ? intDefault : ((int)obValue));
        }

        public static object IsNullIfEqual0(int Param)
        {
            if (Param == 0)
            {
                return null;
            }
            return Param;
        }

        protected bool IsStoredProcedure(string strQuerry)
        {
            return !Regex.IsMatch(strQuerry.Trim(), @"(?:\;|\s)?(select|insert|update|delete|drop)\s", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        protected void SetCommand(SqlCommand command, string strQuerry, SqlParameter[] parameters)
        {
            command.CommandTimeout = 300;
            if (this.IsStoredProcedure(strQuerry))
            {
                command.CommandType = CommandType.StoredProcedure;
            }
            command.Parameters.Clear();
            if (parameters != null)
            {
                foreach (SqlParameter parameter in parameters)
                {
                    if (parameter.Value != null)
                    {
                        if (parameter.Value == DBNull.Value)
                        {
                            command.Parameters.Add(parameter).Value = DBNull.Value;
                        }
                        else
                        {
                            switch (parameter.DbType)
                            {
                                case DbType.AnsiString:
                                case DbType.String:
                                    {
                                        command.Parameters.Add(new SqlParameter(parameter.ParameterName, EscapeQuoteReg(parameter.Value.ToString())));
                                        continue;
                                    }
                                case DbType.Binary:
                                    {
                                        command.Parameters.Add(parameter.ParameterName, SqlDbType.Image).Value = parameter.Value;
                                        continue;
                                    }
                            }
                            command.Parameters.Add(parameter);
                        }
                    }
                }
            }
            command.CommandText = strQuerry;
        }

        /// <summary>
        /// Lọc dữ liệu
        /// </summary>
        /// <param name="strwhere">Câu điều kiện</param>
        /// <returns></returns>
        public static DataTable filterData(DataTable dt, string strwhere)
        {
            /*------------------Filter--------------------*/
            DataRow[] drs = dt.Select(strwhere);
            DataTable dt2 = dt.Clone();

            foreach (DataRow d in drs)
            {
                dt2.ImportRow(d);
            }
            return dt2;
        }
    }
    #endregion

    #region CLASS O_HTML_HELPER
    public class oHtmlHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="queryString"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetQueryString(HttpRequest request, string queryString, string defaultValue)
        {
            var value = defaultValue;
            if (request.QueryString[queryString] != null)
            {
                value = request.QueryString[queryString];
            }
            return value;
        }
        /// <summary>
        /// Function Name: Đặt sự kiện mặc định khi ấn nút Enter cho textbox
        /// Create Date: 2012
        /// Update Date:
        /// Author: Thuannh
        /// </summary>
        /// <param name="txtID">IDtextbox</param>
        /// <param name="btnID">IDbutton</param>
        public static void DefaultEnterKey(TextBox txtID, Button btnID) //Default event Enter for button
        {

            try
            {
                string strJSC = null;

                strJSC = "";
                strJSC += "if(event.which || event.keyCode)";
                strJSC += "{";
                strJSC += " if ((event.which == 13) || (event.keyCode == 13)) ";
                strJSC += " {";

                strJSC += " if (document.getElementById('" + txtID.ID + "').value!='')";
                strJSC += " {";
                strJSC += "   document.getElementById('" + btnID.ID + "').click();return false;";
                strJSC += " }";

                strJSC += " }";
                strJSC += "} ";
                strJSC += "else ";
                strJSC += "{";
                strJSC += " return true;";
                strJSC += "}; ";

                txtID.Attributes.Add("onkeydown", strJSC);

            }
            catch (Exception ex) { }

        }

        // Methods
        public static void Alert(string Message, Page Page)
        {
            if (Message != null)
            {
                Message = oStringHelper.RemoveCRLF(Message);
                Script("AlertScript", string.Format("alert({0})", oDataHelper.InsertQuote(Message)), Page);
            }
        }

        /// <summary>
        /// Thông báo và focus và control
        /// </summary>
        /// <param name="Message">Message</param>
        /// <param name="ctlID">ID control</param>
        /// <param name="Page"></param>
        public static void AlertAndFocus(string Message, Control ctlID, Page Page)
        {
            if (Message != null)
            {
                Message = oStringHelper.RemoveCRLF(Message);
                Script("AlertRederectScript", string.Format("alert({0})", oDataHelper.InsertQuote(Message)) + "; document.getElementById('" + ctlID.ClientID + "').focus();", Page);
            }
        }

        /// <summary>
        /// Chuyen trang
        /// </summary>
        /// <param name="Message">Chuoi thong bao</param>
        /// <param name="strLink">nTrang can chuyen</param>
        /// <param name="Page"></param>
        public static void RederectNewTab(string strLink, Page Page)
        {
            Script("AlertRederectScript", "window.open='" + strLink + "', '_blank';", Page);
        }

        /// <summary>
        /// Thog bao va chuyen trang
        /// </summary>
        /// <param name="Message">Chuoi thong bao</param>
        /// <param name="strLink">nTrang can chuyen</param>
        /// <param name="Page"></param>
        public static void AlertAndRederect(string Message, string strLink, Page Page)
        {
            if (Message != null)
            {
                Message = oStringHelper.RemoveCRLF(Message);
                Script("AlertRederectScript", string.Format("alert({0})", oDataHelper.InsertQuote(Message)) + "; window.location.href='" + strLink + "'", Page);
            }
        }

        public static void Back(int Step, Page Page)
        {
            Script("BackScript", "history.go(" + Step + ")", Page);
        }

        public static void Confirm(string Message, string ifTrue, string ifFalse, Page Page)
        {
            if (Message != null)
            {
                string script = string.Format("\r\nif (window.confirm({0}))\r\n{{{1}}}\r\nelse\r\n{{{2}}}\r\n", oDataHelper.InsertQuote(Message), ifTrue, ifFalse);
                Script("ConfirmScript", script, Page);
            }
        }

        public static DateTime GetDateFromRequestString(string RequestString, DateTime DefaultDate)
        {
            RequestString = HttpContext.Current.Request.QueryString[RequestString];
            if (RequestString != null)
            {
                try
                {
                    DateTime time = DateTime.Parse(RequestString);
                    if (time < DefaultDate)
                    {
                        DefaultDate = time;
                    }
                }
                catch
                {
                }
            }
            return DefaultDate.Date;
        }


        public static decimal GetDecimalFromRequestString(string RequestString, decimal DefaultID)
        {
            RequestString = HttpContext.Current.Request.QueryString[RequestString];
            if (RequestString != null)
            {
                decimal num = 0;
                try
                {
                    num = decimal.Parse(RequestString);
                }
                catch
                {
                }
                return num;
            }
            return DefaultID;
        }

        public static int GetIntFromRequestString(string RequestString, int DefaultID)
        {
            RequestString = HttpContext.Current.Request.QueryString[RequestString];
            if ((RequestString != null) && (RequestString != string.Empty))
            {
                int num = 0;
                try
                {
                    num = int.Parse(RequestString);
                }
                catch
                {
                }
                return num;
            }
            return DefaultID;
        }

        public static string GetRequestString(string RequestString, string DefaultString)
        {
            RequestString = HttpContext.Current.Request.QueryString[RequestString];
            if ((RequestString != null) && (RequestString != string.Empty))
            {
                return RequestString;
            }
            return DefaultString;
        }

        public static void Navigate(string Href, Page Page)
        {
            Script("NavigateScript", "location.href='" + Href, Page);
        }

        public static void RegisterClientScriptBlock(string key, string script, Page Page)
        {
            if ((script != null) && (script != string.Empty))
            {
                script = script.Replace("''", "|");
                Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), key, script);
            }
        }

        public static void Script(string scKey, string script, Page Page)
        {
            if (script != null)
            {
                script = script.Replace("''", "|");
                string str = string.Format("<script language=\"javascript\" type=\"text/javascript\">{0}</script>", script);
                Page.ClientScript.RegisterStartupScript(Page.GetType(), scKey, str);
            }
        }
    }
    #endregion

    #region O_SQL_DATA_HELPER
    public class oSqlDataHelper : oDataHelper
    {
        // Fields
        protected string connectionString_;

        // Methods
        public oSqlDataHelper()
        {
            this.connectionString_ = oDataHelper.ConnectionStringWebConfig("ConnectionString");
        }

        public oSqlDataHelper(string ConnectionString)
        {
            this.connectionString_ = ConnectionString;
        }

        public int Execute(string strQuerry)
        {
            return this.Execute(strQuerry, null);
        }

        public int Execute(string strQuerry, SqlParameter[] parameters)
        {
            int num;
            SqlConnection connection = new SqlConnection(this.connectionString_);
            connection.Open();
            try
            {
                using (SqlCommand command = new SqlCommand())
                {
                    base.SetCommand(command, strQuerry, parameters);
                    command.Connection = connection;
                    return command.ExecuteNonQuery();
                }
            }
            catch (Exception exception)
            {
                throw new Exception(string.Format("oSqlDataHelper ERROR <{0}>", exception.Message));
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return num;
        }

        public DataSet ExecuteDataSet(string strQuerry)
        {
            return this.ExecuteDataSet(strQuerry, null);
        }

        public DataSet ExecuteDataSet(string strQuerry, SqlParameter[] parameters)
        {
            DataSet set;
            SqlConnection connection = new SqlConnection(this.connectionString_);
            connection.Open();
            try
            {
                using (SqlCommand command = new SqlCommand())
                {
                    base.SetCommand(command, strQuerry, parameters);
                    command.Connection = connection;
                    using (SqlDataAdapter adapter = new SqlDataAdapter())
                    {
                        adapter.SelectCommand = command;
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);
                        return dataSet;
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(string.Format("oSqlDataHelper ERROR <{0}>", exception.Message));
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return set;
        }

        public DataTable ExecuteDataTable(string strQuerry)
        {
            return this.ExecuteDataTable(strQuerry, null);
        }

        public DataTable ExecuteDataTable(string strQuerry, SqlParameter[] parameters)
        {
            DataTable table;
            SqlConnection connection = new SqlConnection(this.connectionString_);
            connection.Open();
            try
            {
                using (SqlCommand command = new SqlCommand())
                {
                    base.SetCommand(command, strQuerry, parameters);
                    command.Connection = connection;
                    using (SqlDataAdapter adapter = new SqlDataAdapter())
                    {
                        adapter.SelectCommand = command;
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(string.Format("oSqlDataHelper ERROR <{0}>", exception.Message));
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return table;
        }

        public SqlDataReader ExecuteReader(string strQuerry)
        {
            return this.ExecuteReader(strQuerry, null);
        }

        public SqlDataReader ExecuteReader(string strQuerry, SqlParameter[] parameters)
        {
            SqlDataReader reader;
            try
            {
                SqlConnection connection = new SqlConnection(this.connectionString_);
                SqlCommand command = new SqlCommand();
                base.SetCommand(command, strQuerry, parameters);
                command.Connection = connection;
                connection.Open();
                reader = command.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception exception)
            {
                throw new Exception(string.Format("oSqlDataHelper ERROR <{0}>", exception.Message));
            }
            return reader;
        }

        public object ExecuteScalar(string strQuerry)
        {
            return this.ExecuteScalar(strQuerry, null);
        }

        public object ExecuteScalar(string strQuerry, SqlParameter[] parameters)
        {
            SqlConnection connection = new SqlConnection(this.connectionString_);
            try
            {
                using (SqlCommand command = new SqlCommand())
                {
                    base.SetCommand(command, strQuerry, parameters);
                    command.Connection = connection;
                    connection.Open();
                    object obj2 = command.ExecuteScalar();
                    if ((obj2 != null) && (obj2 != DBNull.Value))
                    {
                        return obj2;
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(string.Format("oSqlDataHelper ERROR <{0}>", exception.Message));
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return null;
        }

        public XmlReader ExecuteXmlReader(string strQuerry)
        {
            return this.ExecuteXmlReader(strQuerry, null);
        }

        public XmlReader ExecuteXmlReader(string strQuerry, SqlParameter[] parameters)
        {
            XmlReader reader;
            try
            {
                SqlConnection connection = new SqlConnection(this.connectionString_);
                SqlCommand command = new SqlCommand();
                base.SetCommand(command, strQuerry, parameters);
                command.Connection = connection;
                connection.Open();
                reader = command.ExecuteXmlReader();
            }
            catch (Exception exception)
            {
                throw new Exception(string.Format("oSqlDataHelper ERROR <{0}>", exception.Message));
            }
            return reader;
        }

        public object Insert(string tableName, SqlParameter[] valueParameters)
        {
            return this.Insert(tableName, valueParameters, false);
        }

        public object Insert(string tableName, SqlParameter[] valueParameters, bool SelectSCOPE_IDENTITY)
        {
            if (valueParameters == null)
            {
                return 0;
            }
            string strT = string.Empty;
            string str2 = string.Empty;
            foreach (SqlParameter parameter in valueParameters)
            {
                if (parameter.Value != null)
                {
                    strT = strT + string.Format("[{0}],", parameter.ParameterName);
                    str2 = str2 + string.Format("@{0},", parameter.ParameterName);
                }
            }
            strT = oStringHelper.StringCut(strT);
            str2 = oStringHelper.StringCut(str2);
            string strQuerry = string.Format("INSERT INTO [{0}] ({1}) VALUES ({2});", tableName, strT, str2);
            if (SelectSCOPE_IDENTITY)
            {
                return this.ExecuteScalar(strQuerry + "Select SCOPE_IDENTITY();", valueParameters);
            }
            return this.Execute(strQuerry, valueParameters);
        }

        public DataSet SelectDataSet(string tableName)
        {
            return this.SelectDataSet(null, tableName);
        }

        public DataSet SelectDataSet(string[] fields, string tableName)
        {
            return this.SelectDataSet(fields, tableName, null);
        }

        public DataSet SelectDataSet(string[] fields, string tableName, SqlParameter[] whereParameters)
        {
            return this.SelectDataSet(fields, tableName, whereParameters, null);
        }

        public DataSet SelectDataSet(string[] fields, string tableName, string strwhere, string orderby)
        {
            string strQuerry = this.SelectSytax(fields, tableName, strwhere, orderby);
            return this.ExecuteDataSet(strQuerry);
        }

        public DataSet SelectDataSet(string[] fields, string tableName, SqlParameter[] whereParameters, string orderby)
        {
            string strQuerry = this.SelectSytax(fields, tableName, whereParameters, orderby);
            return this.ExecuteDataSet(strQuerry, whereParameters);
        }

        public DataTable SelectDataTable(string tableName)
        {
            return this.SelectDataTable(null, tableName);
        }

        public DataTable SelectDataTable(string[] fields, string tableName)
        {
            return this.SelectDataTable(fields, tableName, null);
        }

        public DataTable SelectDataTable(string[] fields, string tableName, SqlParameter[] whereParameters)
        {
            return this.SelectDataTable(fields, tableName, whereParameters, null);
        }

        public DataTable SelectDataTable(string[] fields, string tableName, SqlParameter[] whereParameters, string orderby)
        {
            string strQuerry = this.SelectSytax(fields, tableName, whereParameters, orderby);
            return this.ExecuteDataTable(strQuerry, whereParameters);
        }

        public DataTable SelectDataTable(string[] fields, string tableName, string strwhere, string orderby)
        {
            string strQuerry = this.SelectSytax(fields, tableName, strwhere, orderby);
            return this.ExecuteDataTable(strQuerry);
        }

        public SqlDataReader SelectReader(string tableName)
        {
            return this.SelectReader(null, tableName);
        }

        public SqlDataReader SelectReader(string[] fields, string tableName)
        {
            return this.SelectReader(fields, tableName, null);
        }

        public SqlDataReader SelectReader(string[] fields, string tableName, SqlParameter[] whereParameters)
        {
            return this.SelectReader(fields, tableName, whereParameters, null);
        }

        public SqlDataReader SelectReader(string[] fields, string tableName, string strwhere, string orderby)
        {
            string strQuerry = this.SelectSytax(fields, tableName, strwhere, orderby);
            return this.ExecuteReader(strQuerry);
        }

        public SqlDataReader SelectReader(string[] fields, string tableName, SqlParameter[] whereParameters, string orderby)
        {
            string strQuerry = this.SelectSytax(fields, tableName, whereParameters, orderby);
            return this.ExecuteReader(strQuerry, whereParameters);
        }

        public decimal SelectScalarDecimal(string strQuerry)
        {
            return this.SelectScalarDecimal(strQuerry, null);
        }

        public decimal SelectScalarDecimal(string strQuerry, SqlParameter[] parameters)
        {
            object obj2 = this.ExecuteScalar(strQuerry, parameters);
            if (obj2 == null)
            {
                return -79228162514264337593543950335M;
            }
            return (decimal)obj2;
        }

        public int SelectScalarInt(string strQuerry)
        {
            return this.SelectScalarInt(strQuerry, null);
        }

        public int SelectScalarInt(string strQuerry, SqlParameter[] parameters)
        {
            object obj2 = this.ExecuteScalar(strQuerry, parameters);
            if (obj2 == null)
            {
                return oDataHelper.IntMinValue;
            }
            return (int)obj2;
        }

        public string SelectScalarString(string strQuerry)
        {
            return this.SelectScalarString(strQuerry, null);
        }

        public string SelectScalarString(string strQuerry, SqlParameter[] parameters)
        {
            object obj2 = this.ExecuteScalar(strQuerry, parameters);
            if (obj2 == null)
            {
                return string.Empty;
            }
            return obj2.ToString();
        }

        private string SelectSytax(string[] fields, string tableName, string strwhere, string orderby)
        {
            return this.SelectSytax(fields, tableName, null, strwhere, orderby);
        }

        private string SelectSytax(string[] fields, string tableName, SqlParameter[] whereParameters, string orderby)
        {
            return this.SelectSytax(fields, tableName, whereParameters, null, orderby);
        }

        private string SelectSytax(string[] fields, string tableName, SqlParameter[] whereParameters, string strwhere, string orderby)
        {
            string strT = "SELECT ";
            if (fields != null)
            {
                foreach (string str2 in fields)
                {
                    if (str2.IndexOf('(') == str2.IndexOf('['))
                    {
                        strT = strT + string.Format("[{0}],", str2);
                    }
                    else
                    {
                        strT = strT + string.Format("{0},", str2);
                    }
                }
                strT = oStringHelper.StringCut(strT);
            }
            else
            {
                strT = strT + "*";
            }
            strT = strT + string.Format(" FROM [{0}]", tableName);
            if (whereParameters != null)
            {
                strT = strT + " WHERE ";
                foreach (SqlParameter parameter in whereParameters)
                {
                    if ((parameter.Value != null) && (parameter.Value != DBNull.Value))
                    {
                        strT = strT + string.Format("[{0}]=@{0} AND ", parameter.ParameterName);
                    }
                }
                strT = strT + "2>1";
            }
            else if (strwhere != null)
            {
                strT = strT + string.Format(" WHERE {0}", strwhere);
            }
            if ((orderby != null) && (orderby != string.Empty))
            {
                strT = strT + " ORDER BY " + orderby;
            }
            return strT;
        }

        public static int sExecute(string strQuerry)
        {
            return sExecute(strQuerry, null);
        }

        public static int sExecute(string strQuerry, SqlParameter[] parameters)
        {
            return new oSqlDataHelper().Execute(strQuerry, parameters);
        }

        public static DataSet sExecuteDataSet(string strQuerry)
        {
            return sExecuteDataSet(strQuerry, null);
        }

        public static DataSet sExecuteDataSet(string strQuerry, SqlParameter[] parameters)
        {
            return new oSqlDataHelper().ExecuteDataSet(strQuerry, parameters);
        }

        public static DataTable sExecuteDataTable(string strQuerry)
        {
            return sExecuteDataTable(strQuerry, null);
        }

        public static DataTable sExecuteDataTable(string strQuerry, SqlParameter[] parameters)
        {
            return new oSqlDataHelper().ExecuteDataTable(strQuerry, parameters);
        }

        public static SqlDataReader sExecuteReader(string strQuerry)
        {
            return sExecuteReader(strQuerry, null);
        }

        public static SqlDataReader sExecuteReader(string strQuerry, SqlParameter[] parameters)
        {
            return new oSqlDataHelper().ExecuteReader(strQuerry, parameters);
        }

        public static object sExecuteScalar(string strQuerry)
        {
            return sExecuteScalar(strQuerry, null);
        }

        public static object sExecuteScalar(string strQuerry, SqlParameter[] parameters)
        {
            return new oSqlDataHelper().ExecuteScalar(strQuerry, parameters);
        }

        public static XmlReader sExecuteXmlReader(string strQuerry)
        {
            return sExecuteXmlReader(strQuerry, null);
        }

        public static XmlReader sExecuteXmlReader(string strQuerry, SqlParameter[] parameters)
        {
            return new oSqlDataHelper().ExecuteXmlReader(strQuerry, parameters);
        }

        public static object sInsert(string tableName, SqlParameter[] valueParameters)
        {
            return new oSqlDataHelper().Insert(tableName, valueParameters);
        }

        public static object sInsert(string tableName, SqlParameter[] valueParameters, bool SelectSCOPE_IDENTITY)
        {
            return new oSqlDataHelper().Insert(tableName, valueParameters, SelectSCOPE_IDENTITY);
        }

        public static DataSet sSelectDataSet(string tableName)
        {
            return sSelectDataSet(null, tableName);
        }

        public static DataSet sSelectDataSet(string[] fields, string tableName)
        {
            return sSelectDataSet(fields, tableName, null);
        }

        public static DataSet sSelectDataSet(string[] fields, string tableName, SqlParameter[] whereParameters)
        {
            return sSelectDataSet(fields, tableName, whereParameters, null);
        }

        public static DataSet sSelectDataSet(string[] fields, string tableName, string strwhere, string orderby)
        {
            return new oSqlDataHelper().SelectDataSet(fields, tableName, strwhere, orderby);
        }

        public static DataSet sSelectDataSet(string[] fields, string tableName, SqlParameter[] whereParameters, string orderby)
        {
            return new oSqlDataHelper().SelectDataSet(fields, tableName, whereParameters, orderby);
        }

        public static DataTable sSelectDataTable(string tableName)
        {
            return sSelectDataTable(null, tableName);
        }

        public static DataTable sSelectDataTable(string[] fields, string tableName)
        {
            return sSelectDataTable(fields, tableName, null);
        }

        public static DataTable sSelectDataTable(string[] fields, string tableName, SqlParameter[] whereParameters)
        {
            return sSelectDataTable(fields, tableName, whereParameters, null);
        }

        public static DataTable sSelectDataTable(string[] fields, string tableName, string strwhere, string orderby)
        {
            return new oSqlDataHelper().SelectDataTable(fields, tableName, strwhere, orderby);
        }

        public static DataTable sSelectDataTable(string[] fields, string tableName, SqlParameter[] whereParameters, string orderby)
        {
            return new oSqlDataHelper().SelectDataTable(fields, tableName, whereParameters, orderby);
        }

        public static SqlDataReader sSelectReader(string tableName)
        {
            return sSelectReader(null, tableName);
        }

        public static SqlDataReader sSelectReader(string[] fields, string tableName)
        {
            return sSelectReader(fields, tableName, null);
        }

        public static SqlDataReader sSelectReader(string[] fields, string tableName, SqlParameter[] whereParameters)
        {
            return sSelectReader(fields, tableName, whereParameters, null);
        }

        public static SqlDataReader sSelectReader(string[] fields, string tableName, string strwhere, string orderby)
        {
            return new oSqlDataHelper().SelectReader(fields, tableName, strwhere, orderby);
        }

        public static SqlDataReader sSelectReader(string[] fields, string tableName, SqlParameter[] whereParameters, string orderby)
        {
            return new oSqlDataHelper().SelectReader(fields, tableName, whereParameters, orderby);
        }

        public static decimal sSelectScalarDecimal(string strQuerry)
        {
            return new oSqlDataHelper().SelectScalarDecimal(strQuerry);
        }

        public static decimal sSelectScalarDecimal(string strQuerry, SqlParameter[] parameters)
        {
            return new oSqlDataHelper().SelectScalarDecimal(strQuerry, parameters);
        }

        public static int sSelectScalarInt(string strQuerry)
        {
            return new oSqlDataHelper().SelectScalarInt(strQuerry);
        }

        public static int sSelectScalarInt(string strQuerry, SqlParameter[] parameters)
        {
            return new oSqlDataHelper().SelectScalarInt(strQuerry, parameters);
        }

        public static string sSelectScalarString(string strQuerry)
        {
            return new oSqlDataHelper().SelectScalarString(strQuerry);
        }

        public static string sSelectScalarString(string strQuerry, SqlParameter[] parameters)
        {
            return new oSqlDataHelper().SelectScalarString(strQuerry, parameters);
        }

        public static int sUpdate(string tableName, SqlParameter[] valueParameters)
        {
            return new oSqlDataHelper().Update(tableName, valueParameters);
        }

        public static int sUpdate(string tableName, SqlParameter[] valueParameters, SqlParameter[] whereParameters)
        {
            return new oSqlDataHelper().Update(tableName, valueParameters, whereParameters);
        }

        public int Update(string tableName, SqlParameter[] valueParameters)
        {
            return this.Update(tableName, valueParameters, null);
        }

        public int Update(string tableName, SqlParameter[] valueParameters, SqlParameter[] whereParameters)
        {
            if (valueParameters == null)
            {
                return 0;
            }
            string strT = "SET ";
            foreach (SqlParameter parameter in valueParameters)
            {
                if (parameter.Value != null)
                {
                    strT = strT + string.Format("[{0}]=@{0},", parameter.ParameterName);
                }
            }
            strT = oStringHelper.StringCut(strT);
            List<SqlParameter> list = new List<SqlParameter>();
            list.AddRange(valueParameters);
            string str2 = string.Empty;
            if (whereParameters != null)
            {
                str2 = " WHERE ";
                foreach (SqlParameter parameter2 in whereParameters)
                {
                    if (parameter2.Value != null)
                    {
                        str2 = str2 + string.Format("[{0}]=@{0} AND ", parameter2.ParameterName);
                    }
                }
                str2 = str2 + "2>1";
                list.AddRange(whereParameters);
            }
            string strQuerry = string.Format("UPDATE [{0}] {1}{2};", tableName, strT, str2);
            return this.Execute(strQuerry, list.ToArray());
        }

        // Properties
        public string ConnectionString
        {
            get
            {
                return this.connectionString_;
            }
            set
            {
                this.connectionString_ = value;
            }
        }
    }
    #endregion

    #region O_STRING_HELPER
    public class oStringHelper
    {

        /// <summary>
        /// Convert DataTable to JSON
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static string DataTableToJSON(DataTable table)
        {
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(table);
            return JSONString;
        }

        /// <summary>
        /// Convert Dataset to Json
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static string DataSetToJSON(DataSet ds)
        {
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(ds, Newtonsoft.Json.Formatting.Indented);
            return JSONString;
        }

        /// <summary>
        /// Tạo tiền tố
        /// </summary>
        /// <param name="strChuoi">Chuỗi</param>
        /// <param name="size">Số lương ký tự</param>
        /// <returns></returns>
        public static string fncCreatePrefix(string strChuoi, int size)
        {
            string res = "";
            string strChuoiKhongDau = RemoveSign4VietnameseString(strChuoi); //Lọc bỏ dấu tiếng việt

            if (strChuoiKhongDau.Length <= size) //Nếu số lượng ký tự quá ít VD: GIAY
            {
                return strChuoiKhongDau.ToUpper();
            }
            else
            {
                string[] tu = strChuoiKhongDau.Split(' ');
                if (tu.Length == 1) //Nếu chỉ có 1 từ thì lấy [Size] ký tự đầu tiên
                {
                    return tu[0].Substring(0, size).ToUpper();
                }
                else
                {
                    int iSize = size;
                    if (tu.Length < size) iSize = tu.Length;
                    for (int i = 0; i < iSize; i++)
                    {
                        res += tu[i].Substring(0, 1);
                    }
                }
            }

            return res.ToUpper();
        }

        /// <summary>
        /// Convert DataTable to JSON
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static string DataTableToJSONWithStringBuilder(DataTable table)
        {
            var JSONString = new StringBuilder();
            if (table.Rows.Count > 0)
            {
                JSONString.Append("[");
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    JSONString.Append("{");
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        if (j < table.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\",");
                        }
                        else if (j == table.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\"");
                        }
                    }
                    if (i == table.Rows.Count - 1)
                    {
                        JSONString.Append("}");
                    }
                    else
                    {
                        JSONString.Append("},");
                    }
                }
                JSONString.Append("]");
            }
            return JSONString.ToString();
        }

        /// <summary>
        /// Tạo Mã Ngẫu Nhiên
        /// </summary>
        /// <param name="length">độ dài của chuỗi</param>
        /// <param name="random"></param>
        /// <returns></returns>
        public static string GenerateCoupon(int length, Random random)
        {
            try
            {
                string characters = "123456789ABCDEFGHIJKLMNPQRSTUVWXYZ";
                StringBuilder result = new StringBuilder(length);
                for (int i = 0; i < length; i++)
                {
                    result.Append(characters[random.Next(characters.Length)]);
                }
                return result.ToString();
            }
            catch (Exception ex)
            {
                OsPortal.oFileHelper.WriteLogErr("oStringHelper", "GenerateCoupon", ex.ToString());
                return "Error";
            }
        }

        /// <summary>
        /// Lấy thứ trong tuần
        /// </summary>
        /// <param name="stTime">ngày cần lấy</param>
        /// <returns></returns>
        public static string fncConvetDate2Thu(DateTime stTime)
        {
            int iDayOfWeek = (int)stTime.DayOfWeek;
            if (iDayOfWeek == 0) return "Chủ Nhật";
            else return "Thứ " + (iDayOfWeek + 1);
        }
        /// <summary>
        /// mã hóa capcha
        /// Author: ThuanNH
        /// Create: 04/09/2013
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string rpHash(string value)
        {
            int hash = 5381;
            value = value.ToUpper();
            for (int i = 0; i < value.Length; i++)
            {
                hash = ((hash << 5) + hash) + value[i];
            }
            return hash.ToString();
        }
        /// <summary>
        /// Trả về phần đầu của chuỗi và đảm bảo đủ từ
        /// </summary>
        /// <param name="s"></param>
        /// <param name="length">Số ký tự tối đa được cắt</param>
        /// <returns>Chuoi can cat</returns>
        public static string fncCatChuoi(string s, int length)
        {
            if (String.IsNullOrEmpty(s))
                return "";
            var words = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (words[0].Length > length)
                return s;
            var sb = new StringBuilder();
            foreach (var word in words)
            {
                if ((sb + word).Length > length)
                    return string.Format("{0}...", sb.ToString().TrimEnd(' '));
                sb.Append(word + " ");
            }
            return string.Format("{0}...", sb.ToString().TrimEnd(' '));
        }

        /*======================CONVERT PROSESING========================================*/
        //******************************************************************
        //　　　FUNCTION     : Check is numeric
        //　　　MEMO         : 無し 
        //　　　VALUE        : Boolean      Nullチェック済みの値
        //      PARAMS       : Object       値
        //      CREATE       : 2011/08/02   ThuanNH
        //      UPDATE       : 
        //******************************************************************
        public static bool IsNumeric(object vobjValue)
        {
            try
            {
                Convert.ToDouble(vobjValue);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //******************************************************************
        //　　　FUNCTION     : 空白値を確認 
        //　　　MEMO         : 無し 
        //　　　VALUE        : Boolean   True:空白　False:空白無い 
        //      PARAMS       : Object    確認したい値 
        //      CREATE       : 2009/08/28   ThuanNH 
        //      UPDATE       : 
        //******************************************************************
        public static bool fncIsBlankString(object vstrValue)
        {

            try
            {
                string strTmp = null;

                strTmp = fncCnvNullToString(vstrValue).Trim();


                if (string.IsNullOrEmpty(strTmp))
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                OsPortal.oFileHelper.WriteLogErr("clsCommon", "fncIsBlankString", ex.ToString());
            }

            return false;

        }

        //******************************************************************
        //　　　FUNCTION     : Nullの場合""にして返す 
        //　　　MEMO         : 無し 
        //　　　VALUE        : String Nullチェック済みの値 
        //      PARAMS       : Object 値 
        //      CREATE       : 2009/09/02   ThuanNH 
        //      UPDATE       : 
        //******************************************************************
        public static string fncCnvNullToString(object vobjValue)
        {
            string strRet = null;

            try
            {
                strRet = "";

                if ((vobjValue != null))
                {
                    strRet = Convert.ToString(vobjValue);
                }

                return strRet;
            }
            catch (Exception ex)
            {
                OsPortal.oFileHelper.WriteLogErr("clsCommon", "fncCnvNullToString", ex.ToString());
            }
            return strRet;

        }

        //******************************************************************
        //　　　FUNCTION     : Nullの場合 0 にして返す
        //　　　MEMO         : 無し 
        //　　　VALUE        : integer      Nullチェック済みの値
        //      PARAMS       : Object       値
        //      CREATE       : 2009/09/02   ThuanNH 
        //      UPDATE       : 
        //******************************************************************
        public static int fncCnvNullToInt(object vobjValue)
        {
            int intValue = 0;
            try
            {
                if (fncIsBlankString(vobjValue))
                    return 0;

                if (!IsNumeric(vobjValue))
                {
                    return 0;
                }
                else
                {
                    intValue = Convert.ToInt32(vobjValue);
                }

            }
            catch (Exception ex)
            {
                OsPortal.oFileHelper.WriteLogErr("clsCommon", "fncCnvNullToInt", ex.ToString());
            }

            return intValue;
        }
        //******************************************************************
        //　　　FUNCTION     : Nullの場合 0 にして返す
        //　　　MEMO         : 無し 
        //　　　VALUE        : double      Nullチェック済みの値
        //      PARAMS       : Object       値
        //      CREATE       : 2009/09/03   ThuanNH 
        //      UPDATE       : 
        //******************************************************************
        public static double fncCnvNullToDouble(object vobjValue)
        {
            double dbValue = 0;
            try
            {
                if (fncIsBlankString(vobjValue))
                    return 0;

                if (!IsNumeric(vobjValue))
                {
                    return 0;
                }
                else
                {
                    dbValue = Convert.ToDouble(vobjValue);
                }

            }
            catch (Exception ex)
            {
                OsPortal.oFileHelper.WriteLogErr("clsCommon", "fncCnvNullToDouble", ex.ToString());
            }

            return dbValue;
        }


        /*======================END CONVERT PROSESING========================================*/

        private static readonly string[] VietnameseSigns = new string[]
        {
        "aAeEoOuUiIdDyY",
        "áàạảãâấầậẩẫăắằặẳẵ",
        "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
        "éèẹẻẽêếềệểễ",
        "ÉÈẸẺẼÊẾỀỆỂỄ",
        "óòọỏõôốồộổỗơớờợởỡ",
        "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
        "úùụủũưứừựửữ",
        "ÚÙỤỦŨƯỨỪỰỬỮ",
        "íìịỉĩ",
        "ÍÌỊỈĨ",
        "đ",
        "Đ",
        "ýỳỵỷỹ",
        "ÝỲỴỶỸ"
        };

        /// <summary>
        /// Lọc bỏ dấu tiếng việt
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveSign4VietnameseString(string str)
        {

            //Tiến hành thay thế , lọc bỏ dấu cho chuỗi
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
            }
            return str;
        }


        /// <summary>
        /// Cắt tiêu đề không dấu
        /// Author: ThuanNH
        /// </summary>
        /// <param name="strValue">Chuỗi cần cắt</param>
        /// <param name="intNumber">số ký tự cần cắt</param>
        /// <returns></returns>
        public static string CatTieuDeKoDau(string strValue, int intNumber)
        {
            string strReturn = "";
            if (strValue.Length <= intNumber) return strValue;
            else
            {
                strReturn = strValue.Substring(0, intNumber);
                if (strReturn[strReturn.Length - 1] == '-')
                {
                    strReturn = strReturn.Substring(0, strReturn.Length - 1);
                }
                return strReturn;
            }
        }
        /// <summary>
        /// Tách chuỗi kèm dấu ,
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static string TachChuoi(string strValue)
        {
            string strNewValue = "";
            //strNewValue = CatChuoi(strValue, 9);
            //strNewValue = strNewValue.Replace("...", "");
            strNewValue = strValue.Replace("-", "");
            strNewValue = strNewValue.Replace("  ", " ");
            strNewValue = strNewValue.Replace(" ", ", ");
            strNewValue = strNewValue.Replace(",,", ",");
            return strNewValue;
        }

        //******************************************************************
        //　　　FUNCTION     : AppSettingsを実行し、結果を帰す  
        //                     見つからないときは ApplicationException 
        //                     を発生させる 
        //　　　MEMO         : 無し  
        //　　　VALUE        : 無し  
        //　　　PARAMS       : String    AppSettingsの実行結果 
        //　　　PARAMS       : String    キー名称 
        //　　　CREATE       : 2009/08/29 AKB Thuan
        //　　　UPDATE       : 
        //******************************************************************
        public static string fncGetAppSettings(string vstrKeyName)
        {
            //初期化  
            string strBuff = "";
            //文字列バッファ 
            try
            {
                //AppSettingsを実行 
                strBuff = System.Web.Configuration.WebConfigurationManager.AppSettings[vstrKeyName];

                //見つかったか？ 
                if ((strBuff == null) || strBuff.Length <= 0)
                {
                    throw new ApplicationException(vstrKeyName + "Không tồn tại trong 『web.config』");
                }

                //結果を帰す 
                return strBuff;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex);
            }

            return strBuff;
        }
        // Fields
        public const string CRLF = "\r\n";
        public static string strDe = "-+!@<}xGHId_MNOP']4XYZov#$%ABC)~.DJKLuq2>/EF^&*zTUV(,?tpigecayl1973jhfQn6805mk[{|+=b";
        public static string strEn = "^&*(]CDGH,?|+=1290a6bc-}[{+!@#$EF%dejkno)~fghi.B<78lm>/'pIJKLMNqtuv345xyz_AOPQTUVXYZ";

        // Methods

        /// <summary>
        /// Chuyển kiểu ngày tháng => chuỗi
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="type">1= mn:hh - dd/MM/yyy | </param>
        /// <returns></returns>
        public static string ConvertDateTime_to_String(DateTime dt, int type)
        {
            string strFullString = "";
            try
            {
                string strHouse = dt.Hour.ToString();
                string strMnute = dt.Minute.ToString();

                string strYear = dt.Year.ToString();
                string strMonth = dt.Month.ToString();
                string strDay = dt.Day.ToString();

                switch (type)
                {
                    case 1:
                        strFullString = strHouse + ":" + strMnute + " - " + strDay + "/" + strMonth + "/" + strYear;
                        break;
                }

            }
            catch { }
            return strFullString;
        }

        /// <summary>
        /// Xóa bỏ các thẻ html
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ClearTagHTML(string str)
        {
            return Regex.Replace(str, @"<(.|\n)*?>", string.Empty);
        }

        public static string CleanWordHtml(string html)
        {
            StringCollection strings = new StringCollection();
            strings.Add(@"<(/?span|!\[)[^>]*?>");
            strings.Add(@"<(/?div|!\[)[^>]*?>");
            strings.Add(@"<!--(\w|\W)+?-->");
            strings.Add(@"<title>(\w|\W)+?</title>");
            strings.Add(@"<(meta|link|/?o:|/?style|/?font|/?st\d|/?head|/?html|body|/?body|!\[)[^>]*?>");
            strings.Add("(<[^/][^(th|d)>]*>){1}(&nbsp;)*(</[^>]+>){1}");
            strings.Add("\\s+v:\\w+=\"[^\"]+\"");
            strings.Add("(" + Environment.NewLine + "){2,}");
            strings.Add("( ){2,}");
            foreach (string str in strings)
            {
                html = Regex.Replace(html, str, "", RegexOptions.IgnoreCase);
            }
            return html;
        }

        public static string ConvertDecoding(string ConvertStr)
        {
            if (ConvertStr.Length == 0)
            {
                return ConvertStr;
            }
            return ConvertEncodingDecoding(strEn, strDe, ConvertStr);
        }

        public static string ConvertEncoding(string ConvertStr)
        {
            if (ConvertStr.Length == 0)
            {
                return ConvertStr;
            }
            return ConvertEncodingDecoding(strDe, strEn, ConvertStr);
        }

        private static string ConvertEncodingDecoding(string _strCon1, string _strCon2, string _ConvertStr)
        {
            string str = string.Empty;
            for (int i = 0; i < _ConvertStr.Length; i++)
            {
                int index = _strCon1.IndexOf(_ConvertStr[i]);
                if (index >= 0)
                {
                    str = str + _strCon2[index];
                }
                else
                {
                    str = str + _ConvertStr[i];
                }
            }
            return str;
        }

        public static string ConvertNumberToString(double Value_)
        {
            string str2;
            try
            {
                string[] strArray = new string[] {
                " kh\x00f4ng", " một", " hai", " ba", " bốn", " năm", " s\x00e1u", " bảy", " t\x00e1m", " ch\x00edn", " mươi", " mười", " mốt", " linh", " trăm", " ngh\x00ecn",
                " triệu", " tỷ"
             };
                if (Value_ == 0.0)
                {
                    return strArray[0];
                }
                double num = Value_;
                int num2 = 0;
                for (int i = ((int)num) % 10; (i == 0) && (num != 0.0); i = ((int)num) % 10)
                {
                    num2++;
                    num = ((int)num) / 10;
                }
                int index = 0;
                string[] strArray2 = new string[Value_.ToString().Length + 1];
                while (Value_ != 0.0)
                {
                    int num5 = ((int)Value_) % 10;
                    strArray2[index] = strArray[num5];
                    index++;
                    Value_ = ((int)Value_) / 10;
                }
                string str = string.Empty;
                int num6 = index;
                while (((index - num2) != 0) && (index != 0))
                {
                    switch ((index % 3))
                    {
                        case 0:
                            if (((strArray2[index - 1] != strArray[0]) || (strArray2[index - 2] != strArray[0])) || (strArray2[index - 3] != strArray[0]))
                            {
                                str = str + strArray2[index - 1] + strArray[14];
                            }
                            goto Label_03D1;

                        case 1:
                            if (strArray2[index - 1] != strArray[0])
                            {
                                if (((!(strArray2[index] != strArray[1]) || !(strArray2[index - 1] == strArray[4])) || (num6 == 1)) || (num6 == index))
                                {
                                    break;
                                }
                                str = str + strArray[4];
                            }
                            goto Label_03D1;

                        case 2:
                            if ((((strArray2[index - 1] != strArray[1]) || (num6 != index)) && ((strArray2[index - 1] != strArray[1]) || (strArray2[index] != strArray[0]))) && !(strArray2[index - 1] == strArray[12]))
                            {
                                goto Label_032F;
                            }
                            str = str + strArray[11];
                            goto Label_03D1;

                        default:
                            goto Label_03D1;
                    }
                    if ((((strArray2[index] != strArray[0]) && (strArray2[index - 1] == strArray[5])) && (num6 != 1)) && (num6 != index))
                    {
                        str = str + strArray2[index - 1];
                    }
                    else if ((index == 1) && (strArray2[index - 1] == strArray[1]))
                    {
                        str = str + strArray[12];
                    }
                    else
                    {
                        str = str + strArray2[index - 1];
                    }
                    goto Label_03D1;
                    Label_032F:
                    if ((strArray2[index - 1] == strArray[0]) && (strArray2[index - 2] != strArray[0]))
                    {
                        str = str + " linh";
                    }
                    else if ((strArray2[index - 1] != strArray[0]) || (strArray2[index - 2] != strArray[0]))
                    {
                        if (strArray2[index - 1] != strArray[1])
                        {
                            str = str + strArray2[index - 1] + strArray[10];
                        }
                        else
                        {
                            str = str + strArray[11];
                        }
                    }
                    Label_03D1:
                    switch (index)
                    {
                        case 4:
                            if (((strArray2[index - 1] != strArray[0]) || (strArray2[index] != strArray[0])) || (strArray2[index + 1] != strArray[0]))
                            {
                                str = str + strArray[15];
                            }
                            break;

                        case 7:
                            if (((strArray2[index - 1] != strArray[0]) || (strArray2[index] != strArray[0])) || (strArray2[index + 1] != strArray[0]))
                            {
                                str = str + strArray[0x10];
                            }
                            break;

                        case 10:
                            if (((strArray2[index - 1] != strArray[0]) || (strArray2[index] != strArray[0])) || (strArray2[index + 1] != strArray[0]))
                            {
                                str = str + strArray[0x11];
                            }
                            break;
                    }
                    index--;
                }
                switch (num2)
                {
                    case 4:
                    case 5:
                        str = str + strArray[15];
                        break;

                    case 7:
                    case 8:
                        str = str + strArray[0x10];
                        break;

                    case 10:
                    case 11:
                        str = str + strArray[0x11];
                        break;
                }
                str2 = str.Substring(1);
            }
            catch (Exception exception)
            {
                throw new Exception("Error StringHelper! - " + exception.Message);
            }
            return str2;
        }

        public static string ConvertObjectToNumberString(object dec)
        {
            NumberFormatInfo numberFormat = new CultureInfo("en-US", false).NumberFormat;
            numberFormat.NumberDecimalDigits = 0;
            numberFormat.NumberDecimalSeparator = ",";
            numberFormat.NumberGroupSeparator = ".";
            if (dec is decimal)
            {
                decimal num = (decimal)dec;
                return num.ToString("N", numberFormat);
            }
            if (dec is int)
            {
                int num2 = (int)dec;
                return num2.ToString("N", numberFormat);
            }
            if (dec is float)
            {
                float num3 = (float)dec;
                return num3.ToString("N", numberFormat);
            }
            return dec.ToString();
        }

        public static string ConvertTcvn3ToUnicode(string ConvertStr)
        {
            return ConvertTcvn3ToUnicode(ConvertStr);
        }

        public static string ConvertUnicodeToTcvn3(string ConvertStr)
        {
            return ConvertUnicodeToTcvn3(ConvertStr);
        }

        public static string EncodePasswordHashed(string pass, string salt)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(pass);
            byte[] src = Convert.FromBase64String(salt);
            byte[] dst = new byte[src.Length + bytes.Length];
            Buffer.BlockCopy(src, 0, dst, 0, src.Length);
            Buffer.BlockCopy(bytes, 0, dst, src.Length, bytes.Length);
            return Convert.ToBase64String(HashAlgorithm.Create(Membership.HashAlgorithmType).ComputeHash(dst));
        }

        public static string EncodingHTML(string ConvertStr)
        {
            return EncodingHTML(ConvertStr, new string[] { "&aacute;", "&agrave;", "&atilde;", "&acirc;", "&oacute;", "&ograve;", "&otilde;", "&ocirc;", "&uacute;", "&ugrave;", "&eacute;", "&egrave;", "&ecirc;", "&iacute;", "&igrave;", "&yacute;" }, new string[] { "\x00e1", "\x00e0", "\x00e3", "\x00e2", "\x00f3", "\x00f2", "\x00f5", "\x00f4", "\x00fa", "\x00f9", "\x00e9", "\x00e8", "\x00ea", "\x00ed", "\x00ec", "\x00fd" });
        }

        public static string EncodingHTML(string ConvertStr, string[] StrOld, string[] StrNew)
        {
            string str = ConvertStr;
            for (int i = 0; i < StrOld.Length; i++)
            {
                str = str.Replace(StrOld[i], StrNew[i]);
            }
            return str;
        }

        public static string EncryptStringMD5(string text)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(text, "MD5").ToLower();
        }

        public static string EncryptStringSHA1(string text)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(text, "SHA1").ToLower();
        }

        public static string RemoveAllTag(string strCont)
        {
            return Regex.Replace(strCont, "<[^>]+>", string.Empty);
        }

        public static string RemoveCRLF(string strCont)
        {
            return Regex.Replace(strCont, @"\s{2}", " ");
        }

        public static string StringCut(string strT)
        {
            return strT.Substring(0, strT.Length - 1);
        }

        public static string StringLeft(string strT, int Left)
        {
            return strT.Substring(Left, strT.Length);
        }

        public static string StringRight(string strT, int Right)
        {
            return strT.Substring(0, strT.Length - Right);
        }

        /// <summary>
        /// Trả về phần đầu của chuỗi và đảm bảo đủ từ
        /// </summary>
        /// <param name="s"></param>
        /// <param name="length">Số ký tự tối đa được cắt</param>
        /// <returns>Chuoi can cat</returns>
        public static string CatChuoi(string s, int length)
        {
            if (String.IsNullOrEmpty(s))
                return "";
            var words = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (words[0].Length > length)
                return s;
            var sb = new StringBuilder();
            foreach (var word in words)
            {
                if ((sb + word).Length > length)
                    return string.Format("{0}...", sb.ToString().TrimEnd(' '));
                sb.Append(word + " ");
            }
            return string.Format("{0}...", sb.ToString().TrimEnd(' '));
        }

        /// <summary>
        /// Chuyển sang định dạng ngày tháng Việt Nam
        /// </summary>
        /// <param name="dtDatetime"></param>
        /// <param name="intType">
        ///     1: Hom nay: Thu...ngay...thang...nam...
        ///     2: Thu...ngay...thang...nam...
        ///     3: Thu...ngay/thang/nam
        ///     4: nam/thang/ngay
        ///     5: ngay/thang/nam
        ///     6: ngaythangnam
        ///     7: hh:mm - dd/MM/yyyy
        /// </param>
        /// <returns></returns>
        public static string DateTimeVi(DateTime dtDatetime, int intType)
        {
            string strDateTime = "";
            try
            {
                string strDayOfWeek = "";

                string strDay = "";
                if (dtDatetime.Day < 10)
                    strDay = "0" + dtDatetime.Day.ToString();
                else
                    strDay = dtDatetime.Day.ToString();

                string strMonth = "";
                if (dtDatetime.Month < 10)
                    strMonth = "0" + dtDatetime.Month.ToString();
                else
                    strMonth = dtDatetime.Month.ToString();

                string strYear = dtDatetime.Year.ToString();

                int intHoure = dtDatetime.Hour;
                int intMinute = dtDatetime.Minute;
                int intSecond = dtDatetime.Second;

                int intI = (int)dtDatetime.DayOfWeek;

                switch (intI)
                {
                    case 0: strDayOfWeek = "Chủ nhật"; break;
                    case 1: strDayOfWeek = "Thứ hai"; break;
                    case 2: strDayOfWeek = "Thứ ba"; break;
                    case 3: strDayOfWeek = "Thứ tư"; break;
                    case 4: strDayOfWeek = "Thứ năm"; break;
                    case 5: strDayOfWeek = "Thứ sáu"; break;
                    case 6: strDayOfWeek = "Thứ bảy"; break;
                    default: strDayOfWeek = "Null"; break;
                }

                switch (intType)
                {
                    case 1:   //Return type - Hom nay: Thu...ngay...thang...nam... ex: Hom nay: Thu hai ngay 21 thang 01 2011
                        strDateTime = "Hôm nay: " + strDayOfWeek + " ngày " + strDay + " tháng " + strMonth + " năm " + strYear;
                        break;

                    case 2:   //Return type - Thu...ngay...thang...nam... ex: Thu hai ngay 21 thang 01 2011
                        strDateTime = strDayOfWeek + " ngày " + strDay + " tháng " + strMonth + " năm " + strYear;
                        break;

                    case 3:   //Return type - Thu...ngay/thang/nam ex: Thu 2, 21/01/2011
                        strDateTime = strDayOfWeek + ", " + strDay + "/" + strMonth + "/" + strYear;
                        break;
                    case 4:   //Return type - nam/thang/ngay
                        strDateTime = strYear + "/" + strMonth + "/" + strDay;
                        break;
                    case 5:   //Return type - ngay/thang/nam
                        strDateTime = strDay + "/" + strMonth + "/" + strYear;
                        break;
                    case 6:   //Return type - ngaythangnam
                        strDateTime = strDay + strMonth + strYear;
                        break;
                    case 7:   //Return type - hh:mm - dd/MM/yyyy
                        strDateTime = intHoure + ":" + intMinute + " - " + strDay + "/" + strMonth + "/" + strYear;
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strDateTime;
        }

        /// <summary>
        /// Lấy một chuỗi ngẫu nhiên
        /// </summary>
        /// <param name="codeCount">Số lượng ký tự</param>
        /// <returns></returns>
        public static string NgauNhien_SoChu(int codeCount)
        {
            string allChar = "0,1,2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z";
            string[] allCharArray = allChar.Split(',');
            string randomCode = "";
            int temp = -1;

            Random rand = new Random();
            for (int i = 0; i < codeCount; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * ((int)DateTime.Now.Ticks));
                }
                int t = rand.Next(36);
                if (temp != -1 && temp == t)
                {
                    return NgauNhien_SoChu(codeCount);
                }
                temp = t;
                randomCode += allCharArray[t];
            }
            return randomCode;
        }

        /// <summary>
        /// ClearCache tự động
        /// </summary>
        public static bool ClearCache()
        {
            try
            {
                Cache cache = HttpRuntime.Cache;
                List<string> keys = new List<string>();
                foreach (DictionaryEntry entry in cache)
                {
                    keys.Add((string)entry.Key);
                }
                foreach (string key in keys)
                {
                    cache.Remove(key);
                }
                return true;
                //Response.Write("Clear cache thành công");
            }
            catch (Exception)
            {
                return false;
                //Response.Write("Clear cache thất bại");
            }
        }
    }

    #endregion

    #region O_REGEX_HELPER
    public class oRegexHelper
    {
        /// <summary>
        /// Lấy danh sách ảnh trong bài viết
        /// </summary>
        /// <param name="strContent">Nội dung html</param>
        /// <returns></returns>
        public static List<string> getlstImageInContent(string strContent)
        {
            var lstLinks = new List<string>();
            string pattern = " src=\"([^\"]*)\"";
            Regex rg = new Regex(pattern);
            MatchCollection mc1 = rg.Matches(strContent);

            for (int i = 0; i < mc1.Count; i++)
            {
                string strLink = mc1[i].Value;
                strLink = strLink.Substring(6);
                strLink = strLink.Substring(0, strLink.Length - 1);
                lstLinks.Add(strLink);
            }
            return lstLinks;
        }

        /// <summary>
        /// Lấy mã nguồn website
        /// </summary>
        /// <param name="link">Đường dẫn</param>
        /// <returns></returns>
        public static string LoadHTML(string link)
        {
            try
            {
                // Tạo yêu cầu.
                WebRequest obj = WebRequest.Create(link);

                // Lấy đáp ứng. công việc này sẽ lấy nội dung trang web về
                WebResponse webRespone = obj.GetResponse();

                // Đọc đáp ứng (dạng stream).
                StreamReader sr = new StreamReader(webRespone.GetResponseStream());
                string result = sr.ReadToEnd();

                //Lọc các ký tự thừa
                return Regex.Replace(result, "\t|\r|\n", "");
            }
            catch (Exception ex)
            {
                return "Lỗi trong quá trình xử lý";
            }
        }


        /// <summary>
        /// Đọc file RSS
        /// </summary>
        /// <param name="Url">Đường dẫn</param>
        /// <returns></returns>
        public static DataTable DocdulieuRSS(string Url)
        {
            DataSet Ds = new DataSet();
            XmlTextReader doc = new XmlTextReader(Url);
            try
            {
                Ds.ReadXml(doc);
            }
            catch
            {

            }

            return Ds.Tables[1];
        }

    }
    #endregion

    #region O_FILE_FOLDER_HELPER
    public class oFileHelper
    {
        static string strClassName = "oFileHelper";
        /// <summary>
        /// Tạo thư mục
        /// </summary>
        /// <param name="strPath">Đường dẫn</param>
        /// <returns></returns>
        public static bool CreateFolder(string strPath)
        {
            try
            {
                // Check if the directory we want the image uploaded to actually exists or not
                if (!Directory.Exists(HttpContext.Current.Server.MapPath(strPath)))
                {
                    // If it doesn't then we just create it before going any further
                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath(strPath));
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Làm sạch thư mục
        /// </summary>
        /// <param name="strPath">Đường dẫn</param>
        public static bool ClearFolder(string strPath)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(strPath);

                foreach (FileInfo fi in dir.GetFiles())
                {
                    fi.Delete();
                }

                foreach (DirectoryInfo di in dir.GetDirectories())
                {
                    ClearFolder(di.FullName);
                    di.Delete();
                }
                return true;
            }
            catch (Exception ex) { return false; }
        }

        /// <summary>
        /// Xóa thư mục
        /// </summary>
        /// <param name="strPath">Đường dẫn</param>
        /// <returns></returns>
        public static bool DeleteFolder(string strPath)
        {
            try
            {
                //DirectoryInfo dir = new DirectoryInfo(strPath);
                Directory.Delete(HttpContext.Current.Server.MapPath(strPath), true);
                return true;
            }
            catch (Exception ex) { return false; }
        }

        /// <summary>
        /// Xóa file
        /// </summary>
        /// <param name="strpath">Đường dẫn</param>
        /// <returns></returns>
        public static bool DeleteFile(string strpath)
        {
            try
            {
                string photoLocalName = HttpContext.Current.Server.MapPath(strpath);

                if (File.Exists(photoLocalName))
                {
                    File.Delete(photoLocalName);
                    return true;
                }
            }
            catch (Exception) { return false; }
            return false;
        }

        /// <summary>
        /// Upload và ressize ảnh
        /// </summary>
        /// <param name="fileUpload">upload control</param>
        /// <param name="strPath">Đường dẫn để upload ảnh lên</param>
        /// <param name="width">Độ rộng của ảnh muốn resize</param>
        /// <param name="strImgName">Trả về Tên ảnh</param>
        /// <returns></returns>
        public static bool UploadAnhResizeImage(FileUpload fileUpload, string strPath, int width, out string strImgName, string strText, bool blInsertText)
        {
            // First we check to see if the user has selected a file
            if (fileUpload.HasFile)
            {
                // Find the fileUpload control
                string filename = fileUpload.FileName;
                //Xac dinh kieu file
                string type = filename.Substring(filename.LastIndexOf(".") + 1);

                // Check if the directory we want the image uploaded to actually exists or not
                if (!Directory.Exists(HttpContext.Current.Server.MapPath(strPath)))
                {
                    // If it doesn't then we just create it before going any further
                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath(strPath));
                }
                // Lấy đường dẫn lưu ảnh
                string directory = HttpContext.Current.Server.MapPath(strPath + "/");

                // Create a bitmap of the content of the fileUpload control in memory
                Bitmap originalBMP = new Bitmap(fileUpload.FileContent);

                /*--------------Resize Image-----------------------------------------*/
                // Calculate the new image dimensions
                int origWidth = originalBMP.Width; //old width
                int origHeight = originalBMP.Height; //old height
                int newWidth;
                int newHeight;
                if (origWidth > width)
                {
                    newWidth = width;
                    newHeight = (newWidth * origHeight) / origWidth;
                }
                else
                {
                    newWidth = origWidth;
                    newHeight = origHeight;
                }

                // Create a new bitmap which will hold the previous resized bitmap
                Bitmap newBMP = new Bitmap(originalBMP, newWidth, newHeight);
                // Create a graphic based on the new bitmap
                Graphics oGraphics = Graphics.FromImage(newBMP);
                // Set the properties for the new graphic file
                oGraphics.SmoothingMode = SmoothingMode.AntiAlias; oGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                // Draw the new graphic based on the resized bitmap
                oGraphics.DrawImage(originalBMP, 0, 0, newWidth, newHeight);
                /*-------------------------------------------------------------------*/

                /*-------------------------------Insert Text---------------------------------*/
                if (blInsertText)
                {
                    int opacity = 200; // 50% opaque (0 = invisible, 255 = fully opaque) //Độ trong suốt của chữ
                    Font objFont = new Font("Tahoma", (20));//AntiqueWhite
                    SolidBrush objBrushWrite = new SolidBrush(Color.FromArgb(opacity, Color.Red));
                    // Create point for upper-left corner of drawing.
                    PointF drawPoint = new PointF(10, 10);
                    // *** DrawString ***'  
                    oGraphics.DrawString(strText, objFont, objBrushWrite, drawPoint);
                }
                /*-------------------------------------------------------------------------------*/


                //Kiểm tra có truyền tên ảnh mong muốn vào ko ? (Được khởi tạo là ko InsertText và có truyền tham số strText vào).
                string flName = "";
                if (!blInsertText && strText != "")
                {
                    flName = strText + "." + type; ;//strText ở đây được xem là tên ảnh mong muốn.
                }
                else
                {
                    DateTime dtNow = DateTime.Now;
                    flName = Convert.ToString(dtNow.Year + "" + dtNow.Month + "" + dtNow.Day + "" + dtNow.Hour + "" + dtNow.Minute + "" + dtNow.Second + "" + dtNow.Millisecond);

                    flName = "smail_" + flName + "." + type;
                }
                // Save the new graphic file to the server

                newBMP.Save(directory + flName);

                // Once finished with the bitmap objects, we deallocate them.
                originalBMP.Dispose();
                newBMP.Dispose();
                oGraphics.Dispose();
                strImgName = flName;
                return true;
            }
            else
            {
                strImgName = "";
                return false;
            }
        }

        /// <summary>
        /// Ghi log Error
        /// </summary>
        /// <param name="strFileName">Tên file</param>
        /// <param name="strFunctionName">Tên Hàm</param>
        /// <param name="strErr">Lỗi phát sinh</param>
        public static void WriteLogErr(string strFileName, string strFunctionName, string strErr)
        {
            string strPathLog = "/"; // oStringHelper.fncGetAppSettings("PATH_LOG_ERR");
            string strNewContent = DateTime.Now.ToString() + " - " + strFileName + "\\" + strFunctionName + "\\ =>>" + strErr;
            WriteFileAppend(strNewContent, strPathLog);
        }

        /// <summary>
        /// Đọc nội dung của file
        /// </summary>
        /// <param name="strPath">đ</param>
        /// <returns></returns>
        public static string ReadFile(string strPath)
        {
            string strReturn = "";
            try
            {
                string _strPath = Path.Combine(HttpRuntime.AppDomainAppPath, strPath);
                if (File.Exists(_strPath))
                {
                    // create reader & open file
                    using (StreamReader tr = new StreamReader(_strPath))
                    {
                        // read a line of text
                        strReturn = tr.ReadToEnd();
                        // close the stream
                        tr.Close();
                    }
                }
                else return "";
            }
            catch (Exception ex)
            {
                OsPortal.oFileHelper.WriteLogErr(strClassName, "ReadFile", ex.ToString());
                return "";
            }
            return strReturn;
        }

        /// <summary>
        /// Ghi file txt giữ nội dung cũ và ghi thêm nội dung mới
        /// </summary>
        /// <param name="strContent">Nội dung cần ghi</param>
        /// <param name="strPath">Đường dẫn lưu file</param>
        public static void WriteFileAppend(string strContent, string strPath)
        {
            try
            {
                // create a writer and open the file
                string _strPath = HttpContext.Current.Server.MapPath(strPath);

                if (!File.Exists(_strPath))
                {
                    FileStream fs = null;
                    using (fs = File.Create(_strPath)) { }
                }

                using (TextWriter tw = File.AppendText(_strPath))
                {
                    // write a line of text to the file
                    tw.WriteLine(strContent);
                    // close the stream
                    tw.Close();
                }
            }
            catch { }
        }

        /// <summary>
        /// Ghi file txt - Ghi đè file cũ
        /// </summary>
        /// <param name="strContent">Nội dung cần ghi</param>
        /// <param name="strPath">Đường dẫn lưu file</param>
        public static void WriteFile(string strContent, string strPath)
        {
            try
            {
                // create a writer and open the file
                string _strPath = HttpContext.Current.Server.MapPath(strPath);

                if (!File.Exists(_strPath))
                {
                    FileStream fs = null;
                    using (fs = File.Create(_strPath)) { }
                }

                System.IO.File.WriteAllText(_strPath, strContent);
                using (TextWriter tw = new StreamWriter(_strPath, false))
                {
                    // write a line of text to the file
                    tw.WriteLine(strContent);
                    // close the stream
                    tw.Close();
                }
            }
            catch { }
        }

        /// <summary>
        /// Lấy danh sách ảnh trong folder
        /// </summary>
        public static DataTable fncGetLstImageInFolder(string strPath)
        {
            //DirectoryInfo dir = new DirectoryInfo(HttpContext.Current.Server.MapPath(strPath));
            //FileInfo[] fiList = dir.GetFiles("*.jpg,*.gif,*.png,*.bmp");

            //DataTable table = new DataTable();
            //table.Columns.Add("Dosage", typeof(int));
            //table.Columns.Add("Drug", typeof(string));
            //table.Columns.Add("Patient", typeof(string));
            //table.Columns.Add("Date", typeof(DateTime));

            return fncGetLstFileFromFolder("*.jpg,*.gif,*.png,*.bmp", strPath);
        }

        /// <summary>
        /// Lấy danh sách file trong folder
        /// </summary>
        public static DataTable fncGetLstFileTextInFolder(string strPath)
        {
            return fncGetLstFileFromFolder("*.txt", strPath);
        }

        /// <summary>
        /// Lấy danh sách ảnh trong folder
        /// </summary>
        public static DataTable fncGetLstFileFromFolder(string lstType, string strPath)
        {
            DirectoryInfo dir = new DirectoryInfo(HttpContext.Current.Server.MapPath(strPath));
            FileInfo[] fiList = dir.GetFiles(lstType);

            DataTable table = new DataTable();
            table.Columns.Add("FileName", typeof(string));
            table.Columns.Add("Size", typeof(long));
            table.Columns.Add("Path", typeof(string));
            table.Columns.Add("CreateDate", typeof(DateTime));
            foreach (System.IO.FileInfo fi in fiList)
            {
                table.Rows.Add(fi.Name, fi.Length, fi.Directory.ToString().Replace(HttpContext.Current.Server.MapPath("~"), ""), fi.CreationTime);
            }
            return table;
        }
    }
    #endregion
}
