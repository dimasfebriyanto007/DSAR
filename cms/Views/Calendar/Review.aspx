<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="cms.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Activity Calendar
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="server">
<script  type="text/javascript" src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>"></script>
<link href="<%: Url.Content("~/Scripts/fullcalendar.css") %>" rel="stylesheet" type="text/css" />
<script src="<%: Url.Content("~/Scripts/fullcalendar.min.js") %>" type="text/javascript"></script>
<script src="<%: Url.Content("~/Scripts/jquery.qtip.min.js") %>" type="text/javascript"></script>

<script type="text/javascript">
    $(document).ready(function () {
        function loading_show(objid) {
            $('#' + objid).html("<div align=\"center\"><br /><img src='<%: Url.Content("~/Content/images/ajax-loader.gif") %>'/></div>").fadeIn('fast');
        }

        function load_list() {
            var month = ($('#month').val()) ? $('#month').val() : '';
            var year = ($('#year').val()) ? $('#year').val() : '';
            var Sales = ($('#Sales').val()) ? $('#Sales').val() : '';
            
            if (month=='' || year == ''){
                $.blockUI({
                    overlayCSS: { backgroundColor: '#000',opacity: .95 },
                    message: "<p>Silahkan pilih bulan absensi yang diinginkan</p>",
                    timeout: 3000,
                    theme: true
                });
                return false;
            }
            
            if (Sales==''){
                $.blockUI({
                    overlayCSS: { backgroundColor: '#000',opacity: .95 },
                    message: "<p>Silahkan pilih sales terlebih dahulu untuk melihat data aktifitas-nya</p>",
                    timeout: 3000,
                    theme: true
                });
                return false;
            }

            loading_show('list-container');
            
            $.ajax
		    ({
		        type: "POST",
		        url: "<%: Url.Content("~/Calendar/Show") %>",
		        data: "month=" + month + "&year=" + year + "&salesid=" + Sales,
		        success: function (msg) {
		            $("#list-container").html(msg);
		        }
		    });
        }

        $('#load').click(function(){
            load_list();
        });

        $('#Branch').change(function(){
            var val = $(this).val();
            $.ajax
		    ({
		        type: "POST",
		        url: "<%: Url.Content("~/Sales/LoadSales") %>",
		        data: "BranchCode=" + val,
		        success: function (msg) {
		            $("#Sales").html(msg);
		        }
		    });
        });
    });
</script>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

<h1>Activity Calendar</h1>

<div class="form"><label>Month&nbsp;&nbsp; </label>
<%: Html.DropDownList("month", new SelectList((IEnumerable<SelectOptionModel>)ViewData["MonthOption"], "OptionId", "OptionString", DateTime.Now.Month)) %>
<%: Html.DropDownList("year", new SelectList((IEnumerable<int>)ViewData["YearOption"], DateTime.Now.Year)) %>

<% if (CommonModel.UserRole() == "ABM" || CommonModel.UserRole() == "RBH")
   { %>
   <label>&nbsp;&nbsp;Branch &nbsp;</label>
    <%= Html.DropDownList("Branch", new SelectList((IEnumerable<Branch>)ViewData["Branchs"], "BranchCode", "Name"), "-- Choose Branch --", new { @id = "Branch" })%>
<% } %>
<label>&nbsp;&nbsp;Sales &nbsp;</label>
<%= Html.DropDownList("Sales", new SelectList((IEnumerable<Sale>)ViewData["Sales"], "SalesId", "Name"), "-- Choose Sales --", new { @id = "Sales" })%>

<label>&nbsp;&nbsp;<input class="button" value=" Lihat Calendar " type="button" id="load" /></label>
</div>

<br />
<div id="list-container" style="min-height:300px"></div>

</asp:Content>
