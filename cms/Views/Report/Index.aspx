<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="cms.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Report
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="server">

<script type="text/javascript">
    $(document).ready(function () {
        function loading_show(objid) {
            $('#' + objid).html("<div align=\"center\"><br /><img src='<%: Url.Content("~/Content/images/ajax-loader.gif") %>'/></div>").fadeIn('fast');
        }

        function load_list() {
            var month = ($('#month').val()) ? $('#month').val() : '';
            var year = ($('#year').val()) ? $('#year').val() : '';
            
            loading_show('list-container');
            
            $.ajax
		    ({
		        type: "POST",
		        url: "<%: Url.Content("~/Report/list") %>",
		        data: "month=" + month + "&year=" + year,
		        success: function (msg) {
		            $("#list-container").html(msg);
		        }
		    });
        }
        
        load_list();

        $('#load').click(function(){
            load_list();
        });

        $('#export').click(function(){
            var month = ($('#month').val()) ? $('#month').val() : '';
            var year = ($('#year').val()) ? $('#year').val() : '';
            
            document.location = "<%: Url.Content("~/Report/list") %>?month=" + month + "&year=" + year + "&export=1";
        });

    });
</script>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

<h1>Report</h1>

<table width="600" border="0" cellspacing="0" cellpadding="3" class="report">
    <tr>
    <td class="strong" width="100">Role</td>
    <td class="orange"><%: ViewData["Role"] %></td>
    <td colspan="2">
        <%: Html.DropDownList("month", new SelectList((IEnumerable<SelectOptionModel>)ViewData["MonthOption"], "OptionId", "OptionString", DateTime.Now.Month)) %>
        <%: Html.DropDownList("year", new SelectList((IEnumerable<int>)ViewData["YearOption"], DateTime.Now.Year)) %>
        &nbsp;<input class="button" value=" Load Report " type="button" id="load" />
        &nbsp;<input class="button" value=" Export to Excel " type="button" id="export" />
    </td>
    </tr>
    <tr>
    <td class="strong">No NPK</td>
    <td class="orange"><%: ViewData["Npk"] %></td>
    <td class="strong" width="100">Name</td>
    <td class="orange"><%: ViewData["Name"] %></td>
    </tr>
    <tr>
    <td class="strong">Branch Code</td>
    <td class="orange"><%: ViewData["BranchCode"] %></td>
    <td class="strong">Branch Name</td>
    <td class="orange"><%: ViewData["BranchName"] %></td>
    </tr>
</table>


<div id="list-container" style="overflow-x:scroll;overflow-y:auto;width:100%"></div>

</asp:Content>
