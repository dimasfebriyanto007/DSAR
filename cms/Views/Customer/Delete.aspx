<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<cms.Models.Nasabah>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Delete Customer
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h1>Delete Customer</h1>

<h3>Are you sure you want to delete this?</h3>
<fieldset>
    
    <% if (ViewData["Output"]!=null) { %><div class="error"><%= ViewData["Output"] %></div><% } %>

    <div class="display-label"><label>GCIF</label></div>
    <div class="display-field">
        <%: Html.DisplayFor(model => model.GCIF) %>
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
