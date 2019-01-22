<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="cms.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Pipeline List
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
            var orderBy = ($('#orderBy').val()) ? $('#orderBy').val() : '';
            var orderMode = ($('#orderMode').val()) ? $('#orderMode').val() : '';
            var Status = ($('#Status').val()) ? $('#Status').val() : '';

            loading_show('list-container');
            
            $.ajax
		    ({
		        type: "POST",
		        url: "<%: Url.Content("~/Pipeline/list") %>",
		        data: "page=" + page + "&perpage=" + perpage + "&key=" + key + "&orderBy=" + orderBy + "&orderMode=" + orderMode + "&Status=" + Status,
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

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

<h1>Pipeline List</h1>

<div class="form"><label>Search&nbsp;&nbsp; </label>
<input name="searchWord" type="text" value="" id="key" />

<label>&nbsp;&nbsp;Status &nbsp;</label>
<select name="Status" id="Status">
    <option value="">-- All --</option>
    <option value="HOT">HOT</option>
    <option value="WARM">WARM</option>
</select>
<label>&nbsp;&nbsp;<input class="button" value="Search" type="button" id="search" /></label>
</div>

<form action="" method="post" name"order" style="background:none;border:none;padding:0;">

<div class="urut">
Order By
<select id="orderBy">
    <option value="">Status</option>
    <option value="gcif">GCIF</option>
    <option value="name">Name</option>
    <option value="id">Created Date</option>
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

<div style="float:right;">
    Tampilkan 
    <select name="perpage" id="perpage">        
        <option value="20">20</option>
        <option value="50">50</option>
        <option value="100">100</option>
    </select> per-halaman
</div>
</form>

</asp:Content>
