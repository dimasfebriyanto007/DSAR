<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="cms.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Sales Log
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="server">
<script type="text/javascript">
    $(document).ready(function () {
        function loading_show(objid) {
            $('#' + objid).html("<div align=\"center\"><br /><img src='<%: Url.Content("~/Content/images/ajax-loader.gif") %>'/></div>").fadeIn('fast');
        }

        function load_list(page) {
//            var key = ($('#key').val()) ? $('#key').val() : '';
var DateTo = ($('#dateTo').val()) ? $('#dateTo').val() : '';
var DateEnd = ($('#dateEnd').val()) ? $('#dateEnd').val() : '';

var Action = ($('#Action').val()) ? $('#Action').val() : '';

var Npk = ($('#Npk').val()) ? $('#Npk').val() : '';
var Name = ($('#Name').val()) ? $('#Name').val() : '';

var NpkUser = ($('#NpkUser').val()) ? $('#NpkUser').val() : '';
var UserName = ($('#UserName').val()) ? $('#UserName').val() : '';

            var perpage = ($('#perpage').val()) ? $('#perpage').val() : '';
            var orderBy = ($('#orderBy').val()) ? $('#orderBy').val() : '';
            var orderMode = ($('#orderMode').val()) ? $('#orderMode').val() : '';
            
            loading_show('list-container');
            
            $.ajax
		    ({
		        type: "POST",
		        url: "<%: Url.Content("~/SalesLog/list") %>",
		        data: "page=" + page + "&perpage=" + perpage + "&DateTo=" + DateTo +"&DateEnd="+DateEnd+"&Action="+Action+"&Npk="+Npk+"&Name="+Name +"&orderBy=" + orderBy + "&orderMode=" + orderMode+ "&NpkUser=" + NpkUser+ "&UserName=" + UserName,
		        success: function (msg) {
		            $("#list-container").html(msg);
		        }
		    });
        }

        $('.page-menu').live('click', function () {
            var page = $(this).attr('p');
            load_list(page);
        });

        load_list(1);

        $('#search').click(function(){
            load_list(1);
        });

        $('#perpage, #orderBy, #orderMode').change(function(){
            load_list(1);
        });

        $('#key').keypress(function(event) {
            var keycode = (event.keyCode ? event.keyCode : event.which);
            if(keycode == '13') {
                load_list(1);
            }
        });

        $('#dateTo').datepicker({
            changeMonth: true,
            changeYear: true,
            dateFormat: 'dd-mm-yy'
        });

        $('#dateEnd').datepicker({
            changeMonth: true,
            changeYear: true,
            dateFormat: 'dd-mm-yy'
        });
        
    });
</script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

 <div><ul id="tab">
	<li class="left "><%: @Html.ActionLink("Product", "Index", "ProductLog")%></li>
	<li class="left"><%: @Html.ActionLink("Region", "Index", "RegionLog")%></li>
    <li class="left"><%: @Html.ActionLink("Area", "Index", "AreaLog")%></li>
    <li class="left"><%: @Html.ActionLink("Branch", "Index", "BranchLog")%></li>
    <li class="left"><%: @Html.ActionLink("Call Reason", "Index", "CallReasonLog")%></li>
    <li class="left"><%: @Html.ActionLink("Visit Reason", "Index", "VisitReasonLog")%></li>
    <li class="left"><%: @Html.ActionLink("Sales Team", "Index", "SalesTeamLog")%></li>
    <li class="left"><%: @Html.ActionLink("Customer", "Index", "CustomerLog")%></li>
    <li class="left"><%: @Html.ActionLink("Informasi", "Index", "NewsLog")%></li>
    <li class="left"><%: @Html.ActionLink("Password", "Index", "PasswordLog")%></li>

    <li class="left active"><%: @Html.ActionLink("User", "Index", "SalesLog")%></li>
    
</ul></div>


<ul class="tabNavigation">
    <li><%: @Html.ActionLink("Sales ", "Index", "SalesLog", new { @class = "selected" })%></li>
    <li><%: @Html.ActionLink("Sales Leader ", "Index", "SalesManagerLog")%></li>
    <li><%: @Html.ActionLink("Branch Manager ", "Index", "BranchManagerLog")%></li>
    <li><%: @Html.ActionLink("ABM ", "Index", "AbmLog")%></li>
    <li><%: @Html.ActionLink("RBH ", "Index", "RbhLog")%></li>
    <li><%: @Html.ActionLink("Admin ", "Index", "AdminLog")%></li>
</ul>



<h1>Sales </h1>
<%: ViewData["OutputError"]%>
<br />
<div class="form">

<table>
<tr>
    <td style="width:100px"><label>Period Date </label></td>
    <td style="width:300px">
        <input name="dateTo" type="text" value="<%: ViewData["dateTo"] %>" id="dateTo" style="width:100px" /> &nbsp; To &nbsp;
        <input name="dateEnd" type="text" value="<%: ViewData["dateEnd"] %>" id="dateEnd" style="width:100px" />
    </td>
    <td style="width:100px"><label>Npk User Action</label></td>
    <td style="width:150px">
        <input name="NpkUser" type="text" value="<%: ViewData["NpkUser"] %>" id="NpkUser" style="width:100px" />
    </td>
</tr>
<tr>
    <td><label>NPK</label></td>
    <td>
        <input name="Npk" type="text" value="<%: ViewData["Npk"] %>" id="Npk" style="width:100px" />
    </td>
    <td><label> Name User Action</label></td>
    <td>
        <input name="UserName" type="text" value="<%: ViewData["UserName"] %>" id="UserName" style="width:100px" />
    </td>
</tr>
<tr>
    <td><label>Name</label></td>
    <td>
        <input name="Name" type="text" value="<%: ViewData["Name"] %>" id="Name" style="width:100px" />
    </td>
    <td><label>Action</label> </td>
    <td>
        <select name="Action" id="Action" style="width:100px">
                            <option value="" selected="selected"> -- All-- </option>
                            <option value="Add">Add</option>
                            <option value="Edit">Edit</option>
                            <option value="Delete">Delete</option></select>
    </td>
</tr>
</table>

<label>&nbsp;&nbsp;<input class="button" value="Search" type="button" id="search" /></label>
</div>

<form action="" method="post" name"order" style="background:none;border:none;padding:0;">





<%--<div class="urut">
Order By
<select id="orderBy">    
    <option value="name">Name</option>
</select>
&nbsp;
<select id="orderMode">
    <option value="asc">Ascending</option>
    <option value="desc">Descending</option>    
</select>
</div>--%>
<div class="clear"></div>

<% if (ViewData["Output"] != null)
   { %>
<div class="success"><%= ViewData["Output"]%></div><br />
<% } %>

<div id="list-container"></div>

<div style="float:right;">
    Tampilkan 
    <select name="perpage" id="Select3">        
        <option value="20">20</option>
        <option value="50">50</option>
        <option value="100">100</option>
    </select> per-halaman
</div>
</form>
</asp:Content>