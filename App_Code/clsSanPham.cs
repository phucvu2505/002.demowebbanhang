using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using OsPortal;

/// <summary>
/// Summary description for clsSanPham
/// </summary>
public class clsSanPham
{
    public clsSanPham()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    static string strClassName = "clsSanPham";

    /// <summary>
    /// Lấy danh sách sản phẩm
    /// </summary>
    /// <returns></returns>
    public static DataTable fncGetListSP(int iCURRPAGE, int iPAGESIZE, int iSP_ID, string strSP_TenSanPham, float fSP_DonGia, string strDM_TenDanhMuc, string strNCC_TenNhaCungCap)
    {
        try
        {
            //Gọi Procedure đã viết
            DataTable tblResult = oSqlDataHelper.sExecuteDataTable("SanPham_FND", new SqlParameter[] {
                                                   //Truyền tham số
            new SqlParameter("@P_CURRPAGE", iCURRPAGE)
                   ,new SqlParameter("@P_PAGESIZE", iPAGESIZE)
                   ,new SqlParameter("@P_SP_ID", iSP_ID)
                   ,new SqlParameter("@P_SP_TenSanPham", strSP_TenSanPham)
                   ,new SqlParameter("@P_SP_DonGia", fSP_DonGia)
                   ,new SqlParameter("@P_DM_TenDanhMuc", strDM_TenDanhMuc)
                   ,new SqlParameter("@P_NCC_TenNhaCungCap", strNCC_TenNhaCungCap)
            });

            //Kiểm tra các trường hợp null
            if (tblResult == null) return null;
            if (tblResult.Rows.Count <= 0) return null;

            //Trả cái bảng về thôi
            return tblResult;
        }
        catch (Exception ex)
        {
            //Ghi log nếu lỗi
            OsPortal.oFileHelper.WriteLogErr(strClassName, "fncGetListSP", ex.ToString());
            return new DataTable();
        }
    }
}