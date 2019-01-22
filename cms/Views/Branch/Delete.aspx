<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<cms.Models.Branch>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Delete Branch
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<ul id="tab">
    <%
        if (CommonModel.UserRole() == "ADMINBUSSINES" || CommonModel.UserRole() == "ADMIN")  // Calender - Index - Calender ==>  Calender - Review - Calender
        {
    %>
	<li class="left "><%: @Html.ActionLink("Product", "Index", "Product") %></li>
	<li class="left "><%: @Html.ActionLink("Region", "Index", "Region")%></li>
    <li class="left "><%: @Html.ActionLink("Area", "Index", "Area")%></li>
    <li class="left active"><%: @Html.ActionLink("Branch", "Index", "Branch")%></li>
    <li class="left"><%: @Html.ActionLink("Call Reason", "Index", "CallReason")%></li>
    <li class="left"><%: @Html.ActionLink("Visit Reason", "Index", "VisitReason")%></li>
    <li class="left"><%: @Html.ActionLink("Sales Team", "Index", "SalesTeam")%></li>
    <%
        } 
    %>
    <%
        if (CommonModel.UserRole() == "ADMINUSSER" || CommonModel.UserRole() == "ADMIN")  // Calender - Index - Calender ==>  Calender - Review - Calender
        {
    %>
    <li class="left"><%: @Html.ActionLink("Parameter", "Index", "Parameter")%></li>
     <%
        } 
    %>
    </ul>

<h1>Delete Branch</h1>

<h3>Are you sure you want to delete this?</h3>
<fieldset>
    
    <% if (ViewData["Output"]!=null) { %><div class="error"><%= ViewData["Output"] %></div><% } %>

    <div class="display-label"><label>Area</label></div>
    <div class="display-field">
        <%: Html.DisplayFor(model => model.Area.Name) %>
    </div>

    <div class="display-label"><label>Branch Name</label></div>
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
