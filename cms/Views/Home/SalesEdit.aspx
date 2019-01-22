<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<cms.Models.Sale>" %>
<%@ Import Namespace="cms.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    DSAR Online
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">

<script type="text/javascript">
    $(document).ready(function () {
        function loading_show(objid) {
            $('#' + objid).html("<div align=\"center\"><br /><img src='<%: Url.Content("~/Content/images/ajax-loader.gif") %>'/></div>").fadeIn('fast');
        }

        function load_list(page) {

            loading_show('list-container');
            
            $.ajax
		    ({
		        type: "POST",
		        url: "<%: Url.Content("~/News/Home") %>",
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

        <%
        if (!String.IsNullOrWhiteSpace(ViewBag.PopupProduct)){
            Response.Write("$.colorbox({html:'<a href=\""+Url.Content("~/Product/Detail/" + ViewBag.ProductId)+"\"><img src=\"" + ViewBag.PopupProduct + "\" width=\"100%\" height=\"100%\"></a>',scrolling:false})");
        }
        %>
        
    });
</script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h1>Selamat Datang di DSAR Online</h1>

<% if (TempData["alert"] != null)
   {
       if (TempData["alert"].ToString() == "1")
       {
       %>
       <div class="error">Silahkan isi absen terlebih dahulu sebelum melanjutkan</div>
<%      }
       else if (TempData["alert"].ToString() == "2")
       {
%>
        <div class="error">Silahkan isi profil Anda sebelum melanjutkan </div>
<%
       }           
    } %>

<script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
<script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>

<% using (Html.BeginForm("SalesEdit", "Home", FormMethod.Post, new { enctype = "multipart/form-data" }))
   { %>
    <%: Html.ValidationSummary(true) %>

<table width="100%" border="0" cellspacing="1" cellpadding="6">
    <tr>
    <td width="50%" valign="top">

    <fieldset>
                
        <%
       if (ViewBag.notAbsen.ToString() == "1" && CommonModel.UserRole() == "SALES")
       {
             %>
             <div class="span-14 box-absen">
	            <strong><blink><font color="red">Anda belum mengisi absen hari ini</font></blink></strong><br />
                <p style="color:green">Sebelum memulai aktifitas Anda harus mengisi absen terlebih dahulu. Klik tombol di bawah ini untuk mengisi Absen.</p>
	            <input type="button" name="absen" class="absenbtn" value="Klik untuk mengisi absensi" onclick="document.location='<%: Url.Content("~/Home/Absen") %>'" />	
            </div>
            <div class="clear"></div>
        <%
       }
             %>
        <%: Html.Raw(ViewBag.Output) %>
        
            <div class="editor-label">
                <label>NPK</label>
            </div>
            <div class="editor-field">
                <%: Html.DisplayFor(model => model.Npk)%>
            </div>

            <div class="editor-label">
                <label>Name</label>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Name, new { @style = "width:300px" })%>
                <%: Html.ValidationMessageFor(model => model.Name) %>
            </div>
        
            <div class="editor-label">
                <label>Email</label>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Email, new { @style = "width:300px" })%>
                <%: Html.ValidationMessageFor(model => model.Email) %>
            </div>

            <div class="editor-label">
                <label>Phone</label>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Phone, new { @style = "width:200px" })%>
                <%: Html.ValidationMessageFor(model => model.Phone)%>
            </div>

            <div class="editor-label">
                <label>Sales Role</label>
            </div>
            <div class="editor-field">
                <%: Html.DisplayFor(model => model.SalesTeam.Name) %>
            </div>

            <div class="editor-label">
                <label>Branch</label>
            </div>
            <div class="editor-field">
                <%: Html.DisplayFor(model => model.Branch.Name) %>
            </div>

            <div class="editor-label">
                <label>Sales Leader</label>
            </div>
            <div class="editor-field">
                <% if (ViewData["Leaders"] != null && ViewBag.smExists == "1")
                   { %>
                <%= Html.DropDownListFor(model => model.SmId, new SelectList((IEnumerable<SalesManager>)ViewData["Leaders"], "ManagerId", "Name"), "")%>
                <% }
                   else
                   { %>
                   Tidak ada Sales Manager di cabang <%: ViewData["BranchName"] %>
                <% } %>
            </div>
               
            <div class="editor-label">
                <label>Username</label>
            </div>
            <div class="editor-field">
                <div><%: ViewData["Username"] %></div>
            </div>

            <div class="editor-label">
                <label>Password</label>
            </div>
            <div class="editor-field">
                <%: Html.Password("Password", "", new { @style = "width:200px" })%>
                <span class="hint">Kosongkan jika tidak ingin mengubah password</span>
            </div>

            <div class="editor-label">
                <label>Konfirmasi Password</label>
            </div>
            <div class="editor-field">
                <%: Html.Password("ConfirmPassword", "", new { @style = "width:200px" })%>
            </div>

            <div class="editor-label">
                <label>Status</label>
            </div>
            <div class="editor-field">                    
            <% if (ViewData["Status"].ToString() == "0")
                { %>
                    <div>Not Active</div>
            <% }
                else
                { %>
                    <div>Active</div>
                <% } %>
        </select>
            </div>
        
        <p>
            <input type="submit" value="Save" />
        </p>
    </fieldset>

        </td>
    <td width="50%" valign="top">

    <h2>Informasi & Product Update</h2>

    <div id="list-container"></div>

    </td>
    </tr>
</table>

<% } %>

<div>
    <%: Html.ActionLink("Back to List", "Index") %>
</div>

</asp:Content>