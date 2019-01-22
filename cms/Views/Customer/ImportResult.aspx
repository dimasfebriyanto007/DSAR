<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="cms.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Hasil Import
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="server">

<script type="text/javascript">
    $(document).ready(function () {
        function loading_show(objid) {
            $('#' + objid).html("<div align=\"center\"><br /><img src='<%: Url.Content("~/Content/images/ajax-loader.gif") %>'/></div>").fadeIn('fast');
        }

        function load_list(page) {
            var key = ($('#key').val()) ? $('#key').val() : '';
            var perpage = ($('#perpage').val()) ? $('#perpage').val() : '';
            
            loading_show('list-container');
            
            $.ajax
		    ({
		        type: "POST",
		        url: "<%: Url.Content("~/Customer/importlist") %>",
		        data: "page=" + page + "&perpage=" + perpage + "&key=" + key,
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

        $('#perpage').change(function(){
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

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

<h1>Hasil Import</h1>

<ul id="tab">
	<li class="left"><%: @Html.ActionLink("Import Data Customer", "Import", "Customer") %></li>
	<li class="left active"><%: @Html.ActionLink("Hasil Import", "ImportResult", "Customer")%></li>    
</ul>

<div class="form"><label>Search&nbsp;&nbsp; </label>
<input name="searchWord" type="text" value="" id="key" />
<label>&nbsp;&nbsp;<input class="button" value="Search" type="button" id="search" /></label>
</div>

<form action="" method="post" name="CustImport" id="CustImport" style="background:none;border:none;padding:0;">

<% if (ViewData["Output"] != null)
   { %>
<div class="error"><%= ViewData["Output"]%></div><br />
<% } %>

<% if (ViewData["Success"] != null)
   { %>
<div class="success"><%= ViewData["Success"]%></div><br />
<% } %>

<div id="list-container"></div>

<div style="float:right;">
    Tampilkan 
    <select name="perpage" id="perpage">        
        <option value="100">100</option>
        <option value="150">150</option>
        <option value="200">200</option>
		<option value="500">500</option>
    </select> per-halaman
</div>
</form>

</asp:Content>
