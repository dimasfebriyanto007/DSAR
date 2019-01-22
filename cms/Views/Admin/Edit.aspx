<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<cms.Models.Admin>" %>
<%@ Import Namespace="cms.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Edit Admin
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<ul id="tab">
	<li class="left"><%: @Html.ActionLink("Sales", "Index", "Sales") %></li>
	<li class="left"><%: @Html.ActionLink("Sales Leader", "Index", "SalesManager") %></li>
    <li class="left"><%: @Html.ActionLink("Branch Manager", "Index", "BranchManager")%></li>
    <li class="left"><%: @Html.ActionLink("ABM", "Index", "Abm")%></li>
    <li class="left"><%: @Html.ActionLink("RBH", "Index", "Rbh")%></li>
    <li class="left active"><%: @Html.ActionLink("Admin", "Index", "Admin")%></li>

</ul>

<h1>Edit Admin</h1>

<script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
<script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>

<% using (Html.BeginForm("Edit", "Admin", FormMethod.Post, new { enctype = "multipart/form-data" }))
   { %>
    <%: Html.ValidationSummary(true) %>
    <fieldset>
        
        <% if (ViewData["Output"]!=null) { %><div class="error"><%= ViewData["Output"] %></div><% } %>
        <%: Html.HiddenFor(model => model.UserID) %>

        <table width="100%" border="0" cellspacing="1" cellpadding="6">
          <tr>
            <td width="40%" valign="top">
                <div class="editor-label">
                    <label>NPK</label>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(model => model.NIK, new { @style = "width:200px" })%>
                    <%: Html.ValidationMessageFor(model => model.NIK) %>
                </div>
            
                <div class="editor-label">
                    <label>Nama</label>
                </div>
                <div class="editor-field">
                    <%: Html.TextBox("Nama",ViewData["Nama"], new { @style = "width:200px" })%>
                </div>
                </td>
                <td  width="40%">
                 <div class="editor-label">
                    <label>Username</label>
                </div>
                <div class="editor-field">
                    <%: Html.TextBox("Username",ViewData["Username"], new { @style = "width:200px" })%>
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
                    <label>Role</label>
                </div>
                <div class="editor-field">
                    <select name="Role">
                <% if (ViewData["Role"].ToString() == "ADMINUSER")
                    { %>
                        <option value="ADMINBUSSINES">Bussines Admin </option>
                        <option value="ADMINUSER" selected="selected">User Admin </option>
                <% }
                    else
                    { %>
                        <option value="ADMINBUSSINES"selected="selected" >Bussines Admin </option>
                        <option value="ADMINUSER">User Admin </option>             
                    <% } %>
            </select>
                </div>


                <div class="editor-label">
                    <label>Status</label>
                </div>
                <div class="editor-field">
                    <select name="Status">
                <% if (ViewData["Status"].ToString() == "0")
                    { %>
                        <option value="1">Active</option>
                        <option value="0" selected="selected">Not Active</option>
                <% }
                    else
                    { %>
                        <option value="1" selected="selected">Active</option>
                        <option value="0">Not Active</option>                
                    <% } %>
            </select>
                </div>
            </td>
          </tr>
        </table>
        
        <p>
            <input type="submit" value="Save" />
        </p>
    </fieldset>
<% } %>

<div>
    <%: Html.ActionLink("Back to List", "Index") %>
</div>

</asp:Content>