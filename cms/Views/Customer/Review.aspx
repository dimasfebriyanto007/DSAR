<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="cms.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Customer List
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="server">
<script  type="text/javascript" src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>"></script>
<script type="text/javascript">
    $(document).ready(function () {
        function loading_show(objid) {
            $('#' + objid).html("<div align=\"center\"><br /><img src='<%: Url.Content("~/Content/images/ajax-loader.gif") %>'/></div>").fadeIn('fast');
        }

        function load_list(page) {
            var Sales = ($('#Sales').val()) ? $('#Sales').val() : '';
            var key = ($('#key').val()) ? $('#key').val() : '';
            var perpage = ($('#perpage').val()) ? $('#perpage').val() : '';
            var orderBy = ($('#orderBy').val()) ? $('#orderBy').val() : '';
            var orderMode = ($('#orderMode').val()) ? $('#orderMode').val() : '';
            var Status = ($('#Status').val()) ? $('#Status').val() : '';
            
            if (Sales==''){
                $.blockUI({
                    overlayCSS: { backgroundColor: '#000',opacity: .95 },
                    message: "<p>Silahkan pilih sales terlebih dahulu untuk nasabah-nya</p>",
                    timeout: 3000,
                    theme: true
                });
                return false;
            }

            loading_show('list-container');
            
            $.ajax
		    ({
		        type: "POST",
		        url: "<%: Url.Content("~/Customer/list") %>",
		        data: "page=" + page + "&perpage=" + perpage + "&key=" + key + "&orderBy=" + orderBy + "&orderMode=" + orderMode + "&Status=" + Status + "&SelectedSalesId=" + Sales,
		        success: function (msg) {
               
		            $("#list-container").html(msg);
		        }
		    });
        }

        $('#load').click(function(){
            load_list(1);
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

<h1>Customer List</h1>

<div class="form">
<% if (CommonModel.UserRole() == "ABM" || CommonModel.UserRole() == "RBH")
   { %>
   <label>&nbsp;&nbsp;Branch &nbsp;</label>
    <%= Html.DropDownList("Branch", new SelectList((IEnumerable<Branch>)ViewData["Branchs"], "BranchCode", "Name"), "-- Choose Branch --", new { @id = "Branch" })%>
<% } %>
<label>&nbsp;&nbsp;Sales &nbsp;</label>
<%= Html.DropDownList("Sales", new SelectList((IEnumerable<Sale>)ViewData["Sales"], "SalesId", "Name"), "-- Choose Sales --", new { @id = "Sales" })%>

<label>&nbsp;&nbsp;<input class="button" value=" Lihat Nasabah " type="button" id="load" /></label>
</div>

<p class="create-link2" style="margin:5px 0 0 0;padding:0px">
    <a class="create" href="<%: Url.Content("~/Customer/Create") %>">Add New Customer</a> 
</p>

<% if (TempData["Success"] != null)
   { %>
<div class="success"><%= TempData["Success"]%></div><br />
<% } %>

<div id="list-container" style="min-height:300px"></div>

<div class="clear"></div>

<table width="500" border="0" cellspacing="0" cellpadding="4">
  <tr>
    <th colspan="4" align="left" scope="col">Product Status Legend</th>
  </tr>
  <tr>
    <td width="10" align="center" valign="middle"><img src="<%: Url.Content("~/Content/images/icon_existing.png") %>" width="16" height="16" alt="Existing" /></td>
    <td>Existing Product</td>
    <td width="10" align="center" valign="middle"><img src="<%: Url.Content("~/Content/images/icon_rekomendasi_new.png") %>" width="16" height="16" alt="New" /></td>
    <td>Rekomendasi (NEW)</td>
  </tr>
  <tr>
    <td align="center" valign="middle"><img src="<%: Url.Content("~/Content/images/icon_inisiatif_warm.png") %>" width="16" height="16" alt="Warm" /></td>
    <td>Inisiatif dari sales (WARM)</td>
    <td align="center" valign="middle"><img src="<%: Url.Content("~/Content/images/icon_rekomendasi_warm.png") %>" width="16" height="16" alt="Warm" /></td>
    <td>Rekomendasi (WARM)</td>
  </tr>
  <tr>
    <td align="center" valign="middle"><img src="<%: Url.Content("~/Content/images/icon_inisiatif_hot.png") %>" width="16" height="16" alt="Hot" /></td>
    <td>Inisiatif dari sales (HOT)</td>
    <td align="center" valign="middle"><img src="<%: Url.Content("~/Content/images/icon_rekomendasi_hot.png") %>" width="16" height="16" alt="Hot" /></td>
    <td>Rekomendasi (HOT)</td>
  </tr>
  <tr>
    <td align="center" valign="middle"><img src="<%: Url.Content("~/Content/images/icon_inisiatif_booking.png") %>" width="16" height="16" alt="Booking" /></td>
    <td>Inisiatif dari sales (BOOKING)</td>
    <td align="center" valign="middle"><img src="<%: Url.Content("~/Content/images/icon_rekomendasi_booking.png") %>" width="16" height="16" alt="Booking" /></td>
    <td>Rekomendasi (BOOKING)</td>
  </tr>
  <tr>
    <td align="center" valign="middle"><img src="<%: Url.Content("~/Content/images/icon_inisiatif_cancel.png") %>" width="16" height="16" alt="Cancel" /></td>
    <td>Inisiatif dari sales (CANCEL)</td>
    <td align="center" valign="middle"><img src="<%: Url.Content("~/Content/images/icon_rekomendasi_cancel.png") %>" width="16" height="16" alt="Cancel" /></td>
    <td>Rekomendasi (CANCEL)</td>
  </tr>
</table>

</asp:Content>
