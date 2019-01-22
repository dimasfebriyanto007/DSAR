<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<cms.Models.SalesManager>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Delete Sales Leader
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<ul id="tab">
	<li class="left"><%: @Html.ActionLink("Sales", "Index", "Sales") %></li>
	<li class="left active"><%: @Html.ActionLink("Sales Leader", "Index", "SalesManager")%></li>
    <li class="left"><%: @Html.ActionLink("Branch Manager", "Index", "BranchManager")%></li>
    <li class="left"><%: @Html.ActionLink("ABM", "Index", "Abm")%></li>
    <li class="left"><%: @Html.ActionLink("RBH", "Index", "Rbh")%></li>
    <li class="left"><%: @Html.ActionLink("Admin", "Index", "Admin")%></li>

</ul>

<h1>Delete Sales Leader</h1>

<h3>Are you sure you want to delete this?</h3>
<fieldset>
    
    <% if (ViewData["Output"]!=null) { %><div class="error"><%= ViewData["Output"] %></div><% } %>

    <div class="display-label"><label>NPK</label></div>
    <div class="display-field">
        <%: Html.DisplayFor(model => model.Npk) %>
    </div>

    <div class="display-label"><label>Name</label></div>
    <div class="display-field">
        <%: Html.DisplayFor(model => model.Name) %>
    </div>

</fieldset>
<% using (Html.BeginForm()) { %>
    <p>
        <input type="submit" value="Delete" /> |
        <%: Html.ActionLink("Back to List", "Index") %>
    </p>
<% } %>

</asp:Content>
