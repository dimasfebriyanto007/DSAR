﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<cms.Models.SalesTeam>" %>
<%@ Import Namespace="cms.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Add New Sales Team
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
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
    <li class="left "><%: @Html.ActionLink("Branch", "Index", "Branch")%></li>
    <li class="left "><%: @Html.ActionLink("Call Reason", "Index", "CallReason")%></li>
    <li class="left "><%: @Html.ActionLink("Visit Reason", "Index", "VisitReason")%></li>
    <li class="left active"><%: @Html.ActionLink("Sales Team", "Index", "SalesTeam")%></li>
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

<h1>Add New Sales Team</h1>

<script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
<script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>

<% using (Html.BeginForm("Create", "SalesTeam", FormMethod.Post, new { enctype = "multipart/form-data" }))
   { %>
    <%: Html.ValidationSummary(true) %>
    <fieldset>
        <% if (ViewData["Output"]!=null) { %><div class="error"><%= ViewData["Output"] %></div><% } %>

        <div class="editor-label">
            <label>Name</label>
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.Name, new { @style = "width:300px" })%>
            <%: Html.ValidationMessageFor(model => model.Name) %>
        </div>

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
