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
            + ',strDM_TenDanhMuc: "" '
        + '}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var data = response.d;
            xBuildListDM(data, '#danhmuc_left');
            console.log(data);
            alert(data);
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
        var objData = jQuery.parseJSON(data);
        $(danhmuc_left).empty();
        var strVar = "";

        for (var j = 0; j < objData.length; j++) {
            var iDM_DanhMucCha_ID = objData[j].DM_DanhMucCha_ID;
            var iDM_ID = objData[j].DM_ID;
            var strDM_TenDanhMuc = objData[j].DM_TenDanhMuc;

            strVar += " <button class=\"btn btn-primary dropdown-toggle\" type=\"button\" data-toggle=\"dropdown\">";
            strVar += ""+strDM_TenDanhMuc;
            strVar += "                        <span class=\"caret\"><\/span>";
            strVar += "                    <\/button>";
            strVar += "                    <ul class=\"nav nav-pills nav-stacked admin-menu dropdown-menu\">";
            strVar += "                        <li class=\"active\"><a href=\"#\" data-target-id=\"home\">Push up<\/a><\/li>";
            strVar += "                        <li><a href=\"#\" data-target-id=\"widgets\">Lightly Lined & Unlined<\/a><\/li>";
            strVar += "                        <li><a href=\"#\" data-target-id=\"pages\">Strapless & Multi-Way<\/a><\/li>";
            strVar += "                        <li><a href=\"#\" data-target-id=\"charts\">Balconnet<\/a><\/li>";
            strVar += "                        <li><a href=\"#\" data-target-id=\"table\">Full Coverage<\/a><\/li>";
            strVar += "                        <li><a href=\"#\" data-target-id=\"forms\">Demi<\/a><\/li>";
            strVar += "                        <li><a href=\"#\" data-target-id=\"calender\">Front-Close<\/a><\/li>";
            strVar += "                        <li><a href=\"#\" data-target-id=\"library\">Recerback<\/a><\/li>";
            strVar += "                        <li><a href=\"#\" data-target-id=\"applications\">Bralettes & Crop Bustiers<\/a><\/li>";
            strVar += "                        <li><a href=\"#\" data-target-id=\"settings\">Backless & Bra Accessories<\/a><\/li>";
            strVar += "                    <\/ul>";

        }
        $(danhmuc_left).append(strVar);

        var navItems = $('.admin-menu li > a');
        var navListItems = $('.admin-menu li');
        var allWells = $('.admin-content');
        var allWellsExceptFirst = $('.admin-content:not(:first)');

        allWellsExceptFirst.hide();
        navItems.click(function (e) {
            e.preventDefault();
            navListItems.removeClass('active');
            $(this).closest('li').addClass('active');

            allWells.hide();
            var target = $(this).attr('data-target-id');
            $('#' + target).show();
        });
    }
}

   

