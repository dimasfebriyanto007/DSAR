<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="cms.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Monthly Sales Report
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="server">
<script  type="text/javascript" src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>"></script>
<script type="text/javascript">
    $(document).ready(function () {
        function loading_show(objid) {
            $('#' + objid).html("<div align=\"center\"><br /><img src='<%: Url.Content("~/Content/images/ajax-loader.gif") %>'/></div>").fadeIn('fast');
        }

        function load_list() {
            var dateS = ($('#dateS').val()) ? $('#dateS').val() : '';
            var dateE = ($('#dateE').val()) ? $('#dateE').val() : '';
            var Team = ($('#Team').val()) ? $('#Team').val() : '';
            
            if (dateS=='' || dateE==''){
                $.blockUI({
                    overlayCSS: { backgroundColor: '#000',opacity: .95 },
                    message: "<p>Silahkan isi tanggal report yang diinginkan</p>",
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
		        url: "<%: Url.Content("~/ReportSM/MonthlyList") %>",
		        data: "DateStart=" + dateS + "&DateEnd=" + dateE + "&team=" + Team,
		        success: function (msg) {
		            $("#list-container").html(msg);
		        }
		    });
        }

        $('#load').click(function(){
            load_list();
        });

        $('#export').click(function(){
            var dateS = ($('#dateS').val()) ? $('#dateS').val() : '';
            var dateE = ($('#dateE').val()) ? $('#dateE').val() : '';
            var Branch = ($('#Branch').val()) ? $('#Branch').val() : '';
            var Team = ($('#Team').val()) ? $('#Team').val() : '';

            if (dateS=='' || dateE==''){
                $.blockUI({
                    overlayCSS: { backgroundColor: '#000',opacity: .95 },
                    message: "<p>Silahkan isi tanggal report yang diinginkan</p>",
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
            
            document.location = "<%: Url.Content("~/ReportSM/MonthlyList") %>?DateStart=" + dateS + "&DateEnd=" + dateE + "&team=" + Team + "&export=1";
        });

        $("#dateS").datepicker({
            changeMonth: true,
            changeYear: true,
            dateFormat: 'dd-mm-yy',
            onClose: function (selectedDate) {
                $("#dateE").datepicker("option", "minDate", selectedDate);
            }
        });
        $("#dateE").datepicker({
            changeMonth: true,
            changeYear: true,
            dateFormat: 'dd-mm-yy',
            onClose: function (selectedDate) {
                $("#dateS").datepicker("option", "maxDate", selectedDate);
            }
        });
    });
</script>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

<ul id="tab">
	<li class="left"><%: @Html.ActionLink("Daily Sales Report", "Index", "ReportSM") %></li>
	<li class="left"><%: @Html.ActionLink("Weekly Sales Report", "Weekly", "ReportSM")%></li>
    <li class="left active"><%: @Html.ActionLink("Monthly Sales Report", "Monthly", "ReportSM")%></li>
</ul>

<h1>Monthly Sales Report</h1>

<div class="form"><label>Date&nbsp;&nbsp; </label>
From <input name="DateStart" type="text" value="<%: ViewData["DateStart"] %>" id="dateS" style="width:100px" readonly="readonly" />
To 
<input name="DateEnd" type="text" value="<%: ViewData["DateEnd"] %>" id="dateE" style="width:100px" readonly="readonly" />

<label>&nbsp;&nbsp;Role &nbsp;</label>
<%= Html.DropDownList("Team", new SelectList((IEnumerable<SalesTeam>) ViewData["Teams"], "TeamId", "Name"), "-- Choose Role --", new { @id = "Team" })%>

<label>&nbsp;&nbsp;<input class="button" value=" Load Report " type="button" id="load" /> &nbsp; <input class="button" value=" Export to Excel " type="button" id="export" /></label>
</div>

<br />
<div id="list-container" style="overflow-x:scroll;overflow-y:auto;width:100%;min-height:300px"></div>

</asp:Content>
