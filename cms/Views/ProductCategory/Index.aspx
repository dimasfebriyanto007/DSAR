<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="cms.Models" %>

<asp:Content ID="Content4" ContentPlaceHolderID="TitleContent" runat="server">
    Product Category
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="HeaderContent" runat="server">

<script type="text/javascript">
    $(document).ready(function () {
        function loading_show(objid) {
            $('#' + objid).html("<div align=\"center\"><br /><img src='<%: Url.Content("~/Content/images/ajax-loader.gif") %>'/></div>").fadeIn('fast');
        }

        function load_list(page) {
            var key = ($('#key').val()) ? $('#key').val() : '';
            var perpage = ($('#perpage').val()) ? $('#perpage').val() : '';
            var orderBy = ($('#orderBy').val()) ? $('#orderBy').val() : '';
            var orderMode = ($('#orderMode').val()) ? $('#orderMode').val() : '';
            
            loading_show('list-container');
            
            $.ajax
		    ({
		        type: "POST",
		        url: "<%: Url.Content("~/ProductCategory/list") %>",
		        data: "page=" + page + "&perpage=" + perpage + "&key=" + key + "&orderBy=" + orderBy + "&orderMode=" + orderMode,
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
        
    });
</script>

</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">

<ul id="tab">
    <%
        if (CommonModel.UserRole() == "ADMINBUSSINES" || CommonModel.UserRole() == "ADMIN")  // Calender - Index - Calender ==>  Calender - Review - Calender
        {
    %>
	<li class="left active"><%: @Html.ActionLink("Product", "Index", "Product") %></li>
	<li class="left"><%: @Html.ActionLink("Region", "Index", "Region")%></li>
    <li class="left"><%: @Html.ActionLink("Area", "Index", "Area")%></li>
    <li class="left"><%: @Html.ActionLink("Branch", "Index", "Branch")%></li>
    <li class="left"><%: @Html.ActionLink("Call Reason", "Index", "CallReason")%></li>
    <li class="left"><%: @Html.ActionLink("Visit Reason", "Index", "VisitReason")%></li>
    <li class="left"><%: @Html.ActionLink("Sales Team", "Index", "SalesTeam")%></li>
    <%
        } 
    %>
    <%
        if (CommonModel.UserRole() == "ADMINUSSER" || CommonModel.UserRole() == "ADMIN")  // Calender - Index - Calender ==>  Calender - Review - Calender
        {
    %>
    <li class="left"><%: @Html.ActionLink("Parameter", "Index", "Parameter")%></li>
     <%
        } 
    %>
    </ul>

<ul class="tabNavigation">
    <li><%: @Html.ActionLink("Product", "Index", "Product")%></li>
    <li><%: @Html.ActionLink("Category", "Index", "ProductCategory", new { @class = "selected" })%></li>
    <li><%: @Html.ActionLink("Variant", "Index", "ProductVariant")%></li>
</ul>

<h1>Product Category</h1>

<div class="form"><label>Search&nbsp;&nbsp; </label>
<input name="searchWord" type="text" value="" id="key" />
<label>&nbsp;&nbsp;<input class="button" value="Search" type="button" id="search" /></label>
</div>

<form action="" method="post" name"order" style="background:none;border:none;padding:0;">
<p class="create-link2" style="margin:5px 0 0 0;padding:0px">
    <a class="create" href="<%: Url.Content("~/ProductCategory/Create") %>">Add New Product Category</a>
</p>

<div class="urut">
Order By
<select id="orderBy">    
    <option value="name">Name</option>
</select>
&nbsp;
<select id="orderMode">
    <option value="asc">Ascending</option>
    <option value="desc">Descending</option>    
</select>
</div>
<div class="clear"></div>

<% if (ViewData["Output"] != null)
   { %>
<div class="success"><%= ViewData["Output"]%></div><br />
<% } %>

<div id="list-container"></div>

<div style="float:left;margin-left:100px">
    Tampilkan 
    <select name="perpage" id="Select3">        
        <option value="20">20</option>
        <option value="50">50</option>
        <option value="100">100</option>
    </select> per-halaman
</div>
</form>

</asp:Content>