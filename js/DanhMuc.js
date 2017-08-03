$(document).ready(function () {
    xGetDM();
});

//Lấy danh sách sản phẩm
function xGetDM() {
    $.ajax({
        type: "POST",
        url: "/API.aspx/fncGetListDM",
        data: '{iDM_DanhMucCha_ID: ' + 0
            + ',iDM_ID: ' + 0
        + '}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var data = response.d;
            xBuildListDM(data, '#danhmuc_left');
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

    function xBuildListDM(data, danhmuc_left) {
        $(danhmuc_left).empty();
        var objData = jQuery.parseJSON(data);
        var strVar = "";

        for (var j = 0; j < objData.length; j++) {
            var iDM_DanhMucCha_ID = objData[j].DM_DanhMucCha_ID;
            var iDM_ID = objData[j].DM_ID;
            var strDM_TenDanhMuc = objData[j].DM_TenDanhMuc;

            strVar += "<li>";
            strVar += "                            <div class=\"dropdown\">";
            strVar += "                                <button class=\"btn btn-default dropdown-toggle\" type=\"button\" id=\"menu1\" data-toggle=\"dropdown\">";
            strVar += " " + strDM_TenDanhMuc;
            strVar += "                                    <span class=\"caret\"><\/span>";
            strVar += "                                </button>";
            strVar += "                                <ul class=\"dropdown-menu\" role=\"menu\" aria-labelledby=\"menu1\" id=\"dropcon" + iDM_ID + "\">";
            strVar += "                                </ul>";
            strVar += "                            </div>";
            strVar += "                        </li>";
            xBuildListDMCon(iDM_ID);

            //strVar += "<button class=\"btn btn-primary dropdown-toggle\" type=\"button\" data-toggle=\"dropdown\">";
            //strVar += "                        "+strDM_TenDanhMuc;
            //strVar += "                        <span class=\"caret\"><\/span>";
            //strVar += "                    <\/button>";
            //strVar += "                    <ul class=\"nav nav-pills nav-stacked admin-menu dropdown-menu\" id=\"dropcon\">"
        }
        $(danhmuc_left).append(strVar);

        function xBuildListDMCon(iDM_ID) {
            $.ajax({
                type: "POST",
                 url: "/API.aspx/fncGetListDM",
                 data: '{ iDM_DanhMucCha_ID:' + iDM_ID
                + ',iDM_ID:' + 0
                + '}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var data = response.d;
                    console.log(data);
                    var objData = jQuery.parseJSON(data);
                    var dropname = "#dropcon";
                    $(dropname + iDM_ID).empty();
                    var strVar = "";
                    for (var i = 0; i < objData.length; i++) {
                        var iDM_IDcon = objData[i].DM_ID;
                        var strDM_TenDanhMuc = objData[i].DM_TenDanhMuc;
                        strVar += "                        <li><a href=\"javascript:void(0)\" onclick=\"xGetSPCategory(1,8," + iDM_IDcon + ")\">" + strDM_TenDanhMuc + "<\/a><\/li>";
                    }
                    $(dropname + iDM_ID).append(strVar);
                    //alert(dropname + iDM_ID);
                },
                failure: function (response) {
                    var data = response.d;

                },
                error: function (response) {
                    var data = response.d;
                    console.log(data);
                    alert(data);
                }
            });
        }





        //var navItems = $('.admin-menu li > a');
        //var navListItems = $('.admin-menu li');
        //var allWells = $('.admin-content');
        //var allWellsExceptFirst = $('.admin-content:not(:first)');

        //allWellsExceptFirst.hide();
        //navItems.click(function (e) {
        //    e.preventDefault();
        //    navListItems.removeClass('active');
        //    $(this).closest('li').addClass('active');

        //    allWells.hide();
        //    var target = $(this).attr('data-target-id');
        //    $('#' + target).show();
        //});
    }
}




