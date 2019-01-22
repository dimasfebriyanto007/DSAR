<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="cms.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Weekly Sales Report
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="server">
<script  type="text/javascript" src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>"></script>
<script type="text/javascript">
    $(document).ready(function () {
        function loading_show(objid) {
            $('#' + objid).html("<div align=\"center\"><br /><img src='<%: Url.Content("~/Content/images/ajax-loader.gif") %>'/></div>").fadeIn('fast');
        }

        function load_list() {
            var month = ($('#month').val()) ? $('#month').val() : '';
            var year = ($('#year').val()) ? $('#year').val() : '';
            var Branch = ($('#Branch').val()) ? $('#Branch').val() : '';
            var Team = ($('#Team').val()) ? $('#Team').val() : '';
            
            if (month=='' || year == ''){
                $.blockUI({
                    overlayCSS: { backgroundColor: '#000',opacity: .95 },
                    message: "<p>Silahkan pilih bulan report yang diinginkan</p>",
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
		        url: "<%: Url.Content("~/ReportRBH/WeeklyList") %>",
		        data: "month=" + month + "&year=" + year + "&branch=" + Branch + "&team=" + Team,
		        success: function (msg) {
		            $("#list-container").html(msg);
		        }
		    });
        }

        $('#load').click(function(){
            load_list();
        });

        $('#export').click(function(){
            var month = ($('#month').val()) ? $('#month').val() : '';
            var year = ($('#year').val()) ? $('#year').val() : '';
            var Branch = ($('#Branch').val()) ? $('#Branch').val() : '';
            var Team = ($('#Team').val()) ? $('#Team').val() : '';

            if (month=='' || year == ''){
                $.blockUI({
                    overlayCSS: { backgroundColor: '#000',opacity: .95 },
                    message: "<p>Silahkan pilih bulan report yang diinginkan</p>",
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
            
            document.location = "<%: Url.Content("~/ReportRBH/WeeklyList") %>?month=" + month + "&year=" + year + "&branch=" + Branch + "&team=" + Team + "&export=1";
        });

    });
</script>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

<ul id="tab">
	<li class="left"><%: @Html.ActionLink("Daily Sales Report", "Index", "ReportRBH") %></li>
	<li class="left active"><%: @Html.ActionLink("Weekly Sales Report", "Weekly", "ReportRBH")%></li>
    <li class="left"><%: @Html.ActionLink("Monthly Sales Report", "Monthly", "ReportRBH")%></li>
</ul>

<h1>Weekly Sales Report</h1>

<div class="form"><label>Month&nbsp;&nbsp; </label>
<%: Html.DropDownList("month", new SelectList((IEnumerable<SelectOptionModel>)ViewData["MonthOption"], "OptionId", "OptionString", DateTime.Now.Month)) %>
<%: Html.DropDownList("year", new SelectList((IEnumerable<int>)ViewData["YearOption"], DateTime.Now.Year)) %>

<label>&nbsp;&nbsp;Branch &nbsp;</label>
<%= Html.DropDownList("Branch", new SelectList((IEnumerable<Branch>) ViewData["Branchs"], "BranchCode", "Name"), "-- Choose Branch --", new { @id = "Branch" })%>

<label>&nbsp;&nbsp;Role &nbsp;</label>
<%= Html.DropDownList("Team", new SelectList((IEnumerable<SalesTeam>) ViewData["Teams"], "TeamId", "Name"), "-- Choose Role --", new { @id = "Team" })%>

<label>&nbsp;&nbsp;<input class="button" value=" Load Report " type="button" id="load" /> &nbsp; <input class="button" value=" Export to Excel " type="button" id="export" /></label>
</div>

<br />
<div id="list-container" style="min-height:300px"></div>

</asp:Content>
