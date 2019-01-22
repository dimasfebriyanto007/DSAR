﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="cms.Models" %>

<asp:Content ID="Content4" ContentPlaceHolderID="TitleContent" runat="server">
    RBH
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
            var Region = ($('#Region').val()) ? $('#Region').val() : '';

            loading_show('list-container');
            
            $.ajax
		    ({
		        type: "POST",
		        url: "<%: Url.Content("~/Rbh/list") %>",
		        data: "page=" + page + "&perpage=" + perpage + "&key=" + key + "&orderBy=" + orderBy + "&orderMode=" + orderMode + "&Region=" + Region,
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
	<li class="left"><%: @Html.ActionLink("Sales", "Index", "Sales") %></li>
	<li class="left"><%: @Html.ActionLink("Sales Leader", "Index", "SalesManager") %></li>
    <li class="left"><%: @Html.ActionLink("Branch Manager", "Index", "BranchManager")%></li>
    <li class="left"><%: @Html.ActionLink("ABM", "Index", "Abm")%></li>
    <li class="left active"><%: @Html.ActionLink("RBH", "Index", "Rbh")%></li>
    <li class="left"><%: @Html.ActionLink("Admin", "Index", "Admin")%></li>
</ul>

<h1>RBH</h1>

<div class="form"><label>Search&nbsp;&nbsp; </label>
<input name="searchWord" type="text" value="" id="key" />

<label>&nbsp;&nbsp;Region &nbsp;</label>
<%= Html.DropDownList("Region", new SelectList((IEnumerable<Region>) ViewData["Regions"], "RegionCode", "Name"), "-- All --", new { @id = "Region" })%>

<label>&nbsp;&nbsp;<input class="button" value="Search" type="button" id="search" /></label>
</div>

<form action="" method="post" name"order" style="background:none;border:none;padding:0;">
<p class="create-link2" style="margin:5px 0 0 0;padding:0px">
    <a class="create" href="<%: Url.Content("~/Rbh/Create") %>">Add New RBH</a>
</p>

<div class="urut">
Order By
<select id="orderBy">
    <option value="">Created Date</option>
    <option value="npk">NPK</option>
    <option value="name">Name</option>
    <option value="region">Region</option>
</select>
&nbsp;
<select id="orderMode">
    <option value="desc">Descending</option>
    <option value="asc">Ascending</option>    
</select>
</div>
<div class="clear"></div>
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