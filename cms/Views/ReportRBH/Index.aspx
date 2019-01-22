<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="cms.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Daily Sales Report
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="server">
<script  type="text/javascript" src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>"></script>
<script type="text/javascript">
    $(document).ready(function () {
        function loading_show(objid) {
            $('#' + objid).html("<div align=\"center\"><br /><img src='<%: Url.Content("~/Content/images/ajax-loader.gif") %>'/></div>").fadeIn('fast');
        }

        function load_list() {
            var date = ($('#date').val()) ? $('#date').val() : '';
            var Branch = ($('#Branch').val()) ? $('#Branch').val() : '';
            var Team = ($('#Team').val()) ? $('#Team').val() : '';
            
            if (date==''){
                $.blockUI({
                    overlayCSS: { backgroundColor: '#000',opacity: .95 },
                    message: "<p>Silahkan isi tanggal report yang diinginkan</p>",
                    timeout: 3000,
                    theme: true
                });
                return false;
            }

            if (Branch==''){
                $.blockUI({
                    overlayCSS: { backgroundColor: '#000',opacity: .95 },
                    message: "<p>Silahkan pilih cabang terlebih dahulu untuk melihat report</p>",
                    timeout: 3000,
                    theme: true
                });
                return false;
            }

            if (Team==''){
                $.blockUI({
                    overlayCSS: { backgroundColor: '#000',opacity: .95 },
                    message: "<p>Silahkan pilih role terlebih dahulu untuk melihat report</p>",
                    timeout: 3000,
                    theme: true
                });
                return false;
            }

            loading_show('list-container');
            
            $.ajax
		    ({
		        type: "POST",
		        url: "<%: Url.Content("~/ReportRBH/list") %>",
		        data: "date=" + date + "&branch=" + Branch + "&team=" + Team,
		        success: function (msg) {
		            $("#list-container").html(msg);
		        }
		    });
        }

        $('#load').click(function(){
            load_list();
        });

        $('#export').click(function(){
            var date = ($('#date').val()) ? $('#date').val() : '';
            var Branch = ($('#Branch').val()) ? $('#Branch').val() : '';
            var Team = ($('#Team').val()) ? $('#Team').val() : '';

            if (date==''){
                $.blockUI({
                    overlayCSS: { backgroundColor: '#000',opacity: .95 },
                    message: "<p>Silahkan isi tanggal report yang diinginkan</p>",
                    timeout: 3000,
                    theme: true
                });
                return false;
            }

            if (Branch==''){
                $.blockUI({
                    overlayCSS: { backgroundColor: '#000',opacity: .95 },
                    message: "<p>Silahkan pilih cabang terlebih dahulu untuk melihat report</p>",
                    timeout: 3000,
                    theme: true
                });
                return false;
            }

            if (Team==''){
                $.blockUI({
                    overlayCSS: { backgroundColor: '#000',opacity: .95 },
                    message: "<p>Silahkan pilih role terlebih dahulu untuk melihat report</p>",
                    timeout: 3000,
                    theme: true
                });
                return false;
            }
            
            document.location = "<%: Url.Content("~/ReportRBH/list") %>?date=" + date + "&branch=" + Branch + "&team=" + Team + "&export=1";
        });

        $('#date').datepicker({
            changeMonth: true,
            changeYear: true,
            dateFormat: 'dd-mm-yy'
        });
    });
</script>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

<ul id="tab">
	<li class="left active"><%: @Html.ActionLink("Daily Sales Report", "Index", "ReportRBH") %></li>
	<li class="left"><%: @Html.ActionLink("Weekly Sales Report", "Weekly", "ReportRBH")%></li>
    <li class="left"><%: @Html.ActionLink("Monthly Sales Report", "Monthly", "ReportRBH")%></li>
</ul>

<h1>Daily Sales Report</h1>

<div class="form"><label>Date&nbsp;&nbsp; </label>
<input name="date" type="text" value="<%: ViewData["date"] %>" id="date" style="width:100px" />

<label>&nbsp;&nbsp;Branch &nbsp;</label>
<%= Html.DropDownList("Branch", new SelectList((IEnumerable<Branch>) ViewData["Branchs"], "BranchCode", "Name"), "-- Choose Branch --", new { @id = "Branch" })%>

<label>&nbsp;&nbsp;Role &nbsp;</label>
<%= Html.DropDownList("Team", new SelectList((IEnumerable<SalesTeam>) ViewData["Teams"], "TeamId", "Name"), "-- Choose Role --", new { @id = "Team" })%>

<label>&nbsp;&nbsp;<input class="button" value=" Load Report " type="button" id="load" /> &nbsp; <input class="button" value=" Export to Excel " type="button" id="export" /></label>
</div>

<br />
<div id="list-container" style="overflow-x:scroll;overflow-y:auto;width:100%;min-height:300px"></div>

</asp:Content>
