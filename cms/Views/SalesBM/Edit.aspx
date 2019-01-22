<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<cms.Models.Sale>" %>
<%@ Import Namespace="cms.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Edit Sales
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h1>Edit Sales</h1>

<script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
<script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>

<% using (Html.BeginForm("Edit", "SalesBM", FormMethod.Post, new { enctype = "multipart/form-data" }))
   { %>
    <%: Html.ValidationSummary(true) %>
    <fieldset>
        
        <% if (ViewData["Output"]!=null) { %><div class="error"><%= ViewData["Output"] %></div><% } %>
        <%: Html.HiddenFor(model => model.SalesId) %>

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
                    <label>Sales Role</label>
                </div>
                <div class="editor-field">
                    <%= Html.DropDownListFor(model => model.TeamId, new SelectList((IEnumerable<SalesTeam>) ViewData["Teams"], "TeamId", "Name"))%>
                </div>

                <div class="editor-label">
                    <label>Sales Leader</label>
                </div>
                <div class="editor-field">
                    <%= Html.DropDownListFor(model => model.SmId, new SelectList((IEnumerable<SalesManager>)ViewData["Leaders"], "ManagerId", "Name"), "")%>
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
						<option value="2">Resign</option>
                <% }
                    else
                    { %>
                        <option value="1" selected="selected">Active</option>
                        <option value="0">Not Active</option>
						<option value="2">Resing</option>
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