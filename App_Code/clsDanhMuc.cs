using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using OsPortal;
using System.Data;

/// <summary>
/// Summary description for clsDanhMuc
/// </summary>
public class clsDanhMuc
{
    public clsDanhMuc()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    static string strClassName = "clsDanhMuc";

    ///<summary>
    ///Lấy danh sách danh mục
    /// </summary>
    /// <returns></returns>
    public static DataTable fncGetListDM(int iDM_DanhMucCha_ID, int iDM_ID, string strDM_TenDanhMuc)
    {
        try
        {
            //Gọi Procudure đã viết
            DataTable tblResult = oSqlDataHelper.sExecuteDataTable("DanhMuc_FND", new SqlParameter[]
            //truyền tham số
            {
                new SqlParameter("@P_DM_DanhMucCha_ID", iDM_DanhMucCha_ID)
               ,new SqlParameter("@P_DM_ID", iDM_ID)
               ,new SqlParameter("@P_DM_TenDanhMuc", strDM_TenDanhMuc)
            });

            //Kiểm tra các trường hợp null
            if (tblResult == null) return null;
            if (tblResult.Rows.Count <= 0) return null;

            return tblResult;
        }
        catch (Exception ex)
        {
            //Ghi log nếu lỗi
            OsPortal.oFileHelper.WriteLogErr(strClassName, "fncGetListDM", ex.ToString());
            return new DataTable();
        }
    }
}