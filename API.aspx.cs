using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class API : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    [WebMethod]
    public static string fncGetListSP(int iCURRPAGE, int iPAGESIZE, int iSP_ID, string strSP_TenSanPham, float fSP_DonGia, string strDM_TenDanhMuc, string strNCC_TenNhaCungCap)
    {
        try
        {
            //Convert DataTable sang JSON để javascript có thể hiểu
            return OsPortal.oStringHelper.DataTableToJSON(clsSanPham.fncGetListSP(iCURRPAGE, iPAGESIZE, iSP_ID, strSP_TenSanPham, fSP_DonGia, strDM_TenDanhMuc, strNCC_TenNhaCungCap));
        }
        catch (Exception ex)
        {
            return "Lỗi" + ex.ToString();
        }
    }

    [WebMethod]
    public static string fncGetListDM(int iDM_DanhMucCha_ID, int iDM_ID, string strDM_TenDanhMuc)
    {
        try
        {
            return OsPortal.oStringHelper.DataTableToJSON(clsDanhMuc.fncGetListDM(iDM_DanhMucCha_ID, iDM_ID, strDM_TenDanhMuc));
        }
        catch(Exception ex)
        {
            return "Lỗi" + ex.ToString();
        }
    }
}