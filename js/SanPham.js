$(document).ready(function () {
    xGetSP(1, 8);
    xGetSP(2, 8);
    xGetSP(2, 4);
  
});

//Lấy danh sách sản phẩm
function xGetSP(i_currpage, i_pagesize) {
    $.ajax({
        type: "POST",
        url: "/API.aspx/fncGetListSP",
        data: '{iCURRPAGE: ' + i_currpage
            + ',iPAGESIZE: ' + i_pagesize
            + ',iSP_ID: ' + 0
            + ',strSP_TenSanPham: "" '
            + ',fSP_DonGia: ' + 0
            + ',strDM_TenDanhMuc: "" '
            + ',strNCC_TenNhaCungCap: "" '
        + '}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var data = response.d;
            xBuildListSP(data, '#arrivals','#lingeres_pushup', '#hot');
            console.log(data);
            //alert(data);
        },
        failure: function (response) {
            var data = response.d;
            //xBuildListSP(data);
            console.log(data);
            alert(data);

        },
        error: function (response) {
            var data = response.d;
            //xBuildListSP(data);
            console.log(data);
            alert(data);
        }
    });

    function xBuildListSP(data, arrivals, lingeres_pushup, hot) {
        var objData = jQuery.parseJSON(data);
        //$("#tblSanPham ").empty();
        //for (var i = 0; i < objData.length; i++) {
        //    var iSP_ID = objData[i].SP_ID;
        //    var strSP_TenSanPham = objData[i].SP_TenSanPham;
        //    var fSP_DonGia = objData[i].SP_DonGia;
        //    var strDM_TenDanhMuc = objData[i].DM_TenDanhMuc;
        //    var strNCC_TenNhaCungCap = objData[i].NCC_TenNhaCungCap;

        //    var strRowSP = "<tr>"
        //    + "<td>" + iSP_ID + "</td>"
        //    + "<td>" + strSP_TenSanPham + "</td>"
        //    + "<td>" + fSP_DonGia + "</td>"
        //    + "<td>" + strDM_TenDanhMuc + "</td>"
        //    + "<td>" + strNCC_TenNhaCungCap + "</td>"
        //    + "</tr>";

        //    $('#tblSanPham ').append(strRowSP);
        //}

        $(arrivals).empty();
        var strVar = "<div class=\"col-md-12\" id=\"abc\"><div class=\"carousel carousel-showmanymoveone slide\" id=\"carouselABC\"><div class=\"carousel-inner\">";
        var temp = 0;
        for (var j = 0; j < objData.length; j++) {
            var iSP_ID = objData[j].SP_ID;
            var strSP_TenSanPham = objData[j].SP_TenSanPham;
            var fSP_DonGia = objData[j].SP_DonGia;
            var strDM_TenDanhMuc = objData[j].DM_TenDanhMuc;
            var strNCC_TenNhaCungCap = objData[j].NCC_TenNhaCungCap;
            var strSP_LinkAnh = objData[j].SP_LinkAnh;
            if (temp == 0) {
                strVar += "  <div class=\"item active\">";
                strVar += "                      <div class=\"col-md-3 \">";
                strVar += "                             <img src=\"" + strSP_LinkAnh + "\"style=\"width: 100%\" \/>";
                strVar += "                                <div class=\"cod-md-12\" style=\"margin:5px\">";
                strVar += "                                    <p class=\"name_product\">" + strSP_TenSanPham + "<\/p>";
                strVar += "                                    <p class=\"producttype\">" + strDM_TenDanhMuc + "<\/p>";
                strVar += "                                    <div class=\"cod-md-12\">";
                strVar += "                                        <div class=\"prices\">";
                strVar += "                                            $" + fSP_DonGia;
                strVar += "                                            <div style=\"float:right\">";
                strVar += "                                                <img src=\"image\/star.png\" alt=\"1\" title=\"Dày cộp\" \/>";
                strVar += "                                                <img src=\"image\/star.png\" alt=\"2\" title=\"Dày\" \/>";
                strVar += "                                                <img src=\"image\/star.png\" alt=\"3\" title=\"Hơi mỏng\" \/>";
                strVar += "                                                <img src=\"image\/star.png\" alt=\"4\" title=\"Mỏng\" \/>";
                strVar += "                                                <img src=\"image\/star.png\" alt=\"5\" title=\"Siêu mỏng\" \/>";
                strVar += "                                            <\/div>";
                strVar += "                                        <\/div>";
                strVar += "                                    <\/div>";
                strVar += "                                <\/div>";
                strVar += "                            <\/div>";
                strVar += "                        <\/div>";
                temp++;
            } else {
                strVar += "<div class=\"item\">";
                strVar += "                            <div class=\"col-md-3 \">";
                strVar += "                                <img src=\"" + strSP_LinkAnh + "\" style=\"width: 100%\" \/>";
                strVar += "                                <div class=\"cod-md-12\" style=\"margin:5px\">";
                strVar += "                                    <p class=\"name_product\">" + strSP_TenSanPham + "<\/p>";
                strVar += "                                    <p class=\"producttype\">" + strDM_TenDanhMuc + "<\/p>";
                strVar += "                                    <div class=\"cod-md-12\">";
                strVar += "                                        <div class=\"prices\">";
                strVar += "                                            $" + fSP_DonGia;
                strVar += "                                            <div style=\"float:right\">";
                strVar += "                                                <img src=\"image\/star.png\" alt=\"1\" title=\"Dày cộp\" \/>";
                strVar += "                                                <img src=\"image\/star.png\" alt=\"2\" title=\"Dày\" \/>";
                strVar += "                                                <img src=\"image\/star.png\" alt=\"3\" title=\"Hơi mỏng\" \/>";
                strVar += "                                                <img src=\"image\/star.png\" alt=\"4\" title=\"Mỏng\" \/>";
                strVar += "                                                <img src=\"image\/star.png\" alt=\"5\" title=\"Siêu mỏng\" \/>";
                strVar += "                                            <\/div>";
                strVar += "                                        <\/div>";
                strVar += "                                    <\/div>";
                strVar += "                                <\/div>";
                strVar += "                            <\/div>";
                strVar += "                        <\/div>";

            }
        }

        strVar += " </div> <a class=\"left carousel-control\" href=\"#carouselABC\" data-slide=\"prev\"><i class=\"glyphicon glyphicon-chevron-left\"></i></a><a class=\"right carousel-control\" href=\"#carouselABC\" data-slide=\"next\"><i class=\"glyphicon glyphicon-chevron-right\"></i></a> </div></div>";

        $(arrivals).append(strVar);
        $('#carouselABC').carousel({ interval: 3600 });
        $('.carousel-showmanymoveone .item').each(function () {
            var itemToClone = $(this);

            for (var i = 1; i < 4; i++) {
                itemToClone = itemToClone.next();

                // wrap around if at end of item collection
                if (!itemToClone.length) {
                    itemToClone = $(this).siblings(':first');
                }

                // grab item, clone, add marker class, add to collection
                itemToClone.children(':first-child').clone()
                  .addClass("cloneditem-" + (i))
                  .appendTo($(this));
            }
        });
        console.log(strVar);

        $(lingeres_pushup).empty();
        var strVar = "";

        for (var j = 0; j < objData.length; j++) {
            var iSP_ID = objData[j].SP_ID;
            var strSP_TenSanPham = objData[j].SP_TenSanPham;
            var fSP_DonGia = objData[j].SP_DonGia;
            var strDM_TenDanhMuc = objData[j].DM_TenDanhMuc;
            var strNCC_TenNhaCungCap = objData[j].NCC_TenNhaCungCap;
            var strSP_LinkAnh = objData[j].SP_LinkAnh;

            strVar += "                    <div class=\"col-md-4\" style=\"padding:5px\">";
            strVar += "                                <img src=\"" + strSP_LinkAnh + "\" style=\"width: 100%\" \/>";
            strVar += "                        <div class=\"cod-md-12\" style=\"margin:5px\">";
            strVar += "                            <p class=\"name_product\">" + strSP_TenSanPham + "<\/p>";
            strVar += "                            <p class=\"producttype\">" + strDM_TenDanhMuc + "<\/p>";
            strVar += "                            <div class=\"cod-md-12\">";
            strVar += "                                <div class=\"prices\">";
            strVar += "                                    $ " + fSP_DonGia;
            strVar += "                                    <div style=\"float:right\">";
            strVar += "                                        <img src=\"image\/star.png\" alt=\"1\" title=\"Dày cộp\" \/>";
            strVar += "                                        <img src=\"image\/star.png\" alt=\"2\" title=\"Dày\" \/>";
            strVar += "                                        <img src=\"image\/star.png\" alt=\"3\" title=\"Hơi mỏng\" \/>";
            strVar += "                                        <img src=\"image\/star.png\" alt=\"4\" title=\"Mỏng\" \/>";
            strVar += "                                        <img src=\"image\/star.png\" alt=\"5\" title=\"Siêu mỏng\" \/>";
            strVar += "                                    <\/div>";
            strVar += "                                <\/div>";
            strVar += "                            <\/div>";
            strVar += "                        <\/div>";
            strVar += "                    <\/div>";
        }
        $(lingeres_pushup).append(strVar);

        $(hot).empty();
        var strVar = "";

        for (var j = 0; j < objData.length; j++) {
            var iSP_ID = objData[j].SP_ID;
            var strSP_TenSanPham = objData[j].SP_TenSanPham;
            var fSP_DonGia = objData[j].SP_DonGia;
            var strDM_TenDanhMuc = objData[j].DM_TenDanhMuc;
            var strNCC_TenNhaCungCap = objData[j].NCC_TenNhaCungCap;
            var strSP_LinkAnh = objData[j].SP_LinkAnh;

            strVar += " <div class=\"col-md-3\">";
            strVar += "            <div class=\"view view-sixth\">";
            strVar += "                                <img src=\"" + strSP_LinkAnh + "\" style=\"width: 100%\" \/>";
            strVar += "                <div class=\"mask\">";
            strVar += "                    <h2>" + strNCC_TenNhaCungCap + "<\/h2>";
            strVar += "                    <p>A wonderful serenity has taken possession of my entire soul, like these sweet mornings of spring which I enjoy with my whole heart.<\/p>";
            strVar += "                    <a href=\"#\" class=\"info\">Read More<\/a>";
            strVar += "                <\/div>";
            strVar += "            <\/div>";
            strVar += "            <div class=\"cod-md-12\" style=\"margin:5px\">";
            strVar += "                <p class=\"name_product\">" + strSP_TenSanPham + "<\/p>";
            strVar += "                <p class=\"producttype\">" + strDM_TenDanhMuc + "<\/p>";
            strVar += "                <div class=\"cod-md-12\">";
            strVar += "                    <div class=\"prices\">";
            strVar += "                        $ " + fSP_DonGia;
            strVar += "                        <div style=\"float:right\">";
            strVar += "                            <img src=\"image\/star.png\" alt=\"1\" title=\"Dày cộp\" \/>";
            strVar += "                            <img src=\"image\/star.png\" alt=\"2\" title=\"Dày\" \/>";
            strVar += "                            <img src=\"image\/star.png\" alt=\"3\" title=\"Hơi mỏng\" \/>";
            strVar += "                            <img src=\"image\/star.png\" alt=\"4\" title=\"Mỏng\" \/>";
            strVar += "                            <img src=\"image\/star.png\" alt=\"5\" title=\"Siêu mỏng\" \/>";
            strVar += "                        <\/div>";
            strVar += "                    <\/div>";
            strVar += "                <\/div>              ";
            strVar += "            <\/div>";
            strVar += "        <\/div>";
        }
        $(hot).append(strVar);
    }
}

