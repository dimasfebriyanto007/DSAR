<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<cms.Models.RBH>" %>
<%@ Import Namespace="cms.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Add New RBH
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <script>
        $(function () {
            $('#Npk').keyup(function () {
                $('#Username').val($('#Npk').val());
            });
        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<ul id="tab">
	<li class="left"><%: @Html.ActionLink("Sales", "Index", "Sales") %></li>
	<li class="left"><%: @Html.ActionLink("Sales Leader", "Index", "SalesManager") %></li>
    <li class="left"><%: @Html.ActionLink("Branch Manager", "Index", "BranchManager")%></li>
    <li class="left"><%: @Html.ActionLink("ABM", "Index", "Abm")%></li>
    <li class="left active"><%: @Html.ActionLink("RBH", "Index", "Rbh")%></li>
    <li class="left"><%: @Html.ActionLink("Admin", "Index", "Admin")%></li>

</ul>

<h1>Add New RBH</h1>

<script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
<script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>

<% using (Html.BeginForm("Create", "Rbh", FormMethod.Post, new { enctype = "multipart/form-data" }))
   { %>
    <%: Html.ValidationSummary(true) %>
    <fieldset>
        <% if (ViewData["Output"]!=null) { %><div class="error"><%= ViewData["Output"] %></div><% } %>

        <table width="100%" border="0" cellspacing="1" cellpadding="6">
          <tr>
            <td width="40%" valign="top">
                <div class="editor-label">
                    <label>NPK</label>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(model => model.Npk, new { @style = "width:200px" })%>
                    <%: Html.ValidationMessageFor(model => model.Npk) %>
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
                    <label>Region</label>
                </div>
                <div class="editor-field">
                    <%= Html.DropDownListFor(model => model.RegionCode, new SelectList((IEnumerable<Region>)ViewData["Regions"], "RegionCode", "Name"))%>
                </div>
                                
            </td>
            <td valign="top">
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
        

        <br />
        <p>
            <input type="submit" value=" Save " />
        </p>
    </fieldset>
<% } %>

<div>
    <%: Html.ActionLink("Back to List", "Index") %>
</div>
</asp:Content>
