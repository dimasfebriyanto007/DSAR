<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%
    if (Request.IsAuthenticated)
    {
        if (CommonModel.UserRole() == "ADMIN")
        {
%>
<ul id="menu">
    <li>
        <%= Html.ActionLink("Home", "Index", "Home")%></li>
    <li>
        <%= Html.ActionLink("Customer", "Index", "Customer")%></li>
    <li>
        <%= Html.ActionLink("Pipeline", "Index", "Pipeline")%></li>
    <li>
        <%= Html.ActionLink("User", "Index", "Sales")%></li>
    <li>
        <%= Html.ActionLink("Informasi", "Index", "News")%></li>
    <li>
        <%= Html.ActionLink("Setting", "Index", "Product")%></li>
    <li>
        <%= Html.ActionLink("Report", "Index", "ReportAdmin")%></li>
    <li>
        <%= Html.ActionLink("Log", "Index", "ProductLog")%></li>
</ul>
<%
        }

        else if (CommonModel.UserRole() == "ADMINUSER")  // Calender - Index - Calender ==>  Calender - Review - Calender
        {
%>
<ul id="menu">
    <li>
        <%= Html.ActionLink("Home", "Index", "Home")%></li>
    <li>
        <%= Html.ActionLink("User", "Index", "Sales")%></li>
    <li>
        <%= Html.ActionLink("Setting", "Index", "Parameter")%></li>
</ul>
<%
        }

        else if (CommonModel.UserRole() == "ADMINBUSSINES")  // Calender - Index - Calender ==>  Calender - Review - Calender
        {
%>
<ul id="menu">
    <li>
        <%= Html.ActionLink("Home", "Index", "Home")%></li>
    <li>
        <%= Html.ActionLink("Customer", "Index", "Customer")%></li>
    <li>
        <%= Html.ActionLink("Informasi", "Index", "News")%></li>
    <li>
        <%= Html.ActionLink("Setting", "Index", "Product")%></li>
</ul>
<%
        }

        else if (CommonModel.UserRole() == "SALES")  // Calender - Index - Calender ==>  Calender - Review - Calender
        {
%>
<ul id="menu">
    <li>
        <%= Html.ActionLink("Home", "Index", "Home")%></li>
    <li>
        <%= Html.ActionLink("Absensi", "Index", "Absensi")%></li>
    <li>
        <%= Html.ActionLink("Customer", "Index", "Customer")%></li>
    <li>
        <%= Html.ActionLink("Pipeline", "Index", "Pipeline")%></li>
    <li>
        <%= Html.ActionLink("Calendar", "Review", "Calendar")%></li>
    <li>
        <%= Html.ActionLink("Report", "Index", "Report")%></li>
</ul>
<%
        }
        else if (CommonModel.UserRole() == "SM")
        {
%>
<ul id="menu">
    <li>
        <%= Html.ActionLink("Home", "Index", "Home")%></li>
    <li>
        <%= Html.ActionLink("Sales", "Index", "SalesSM")%></li>
    <li>
        <%= Html.ActionLink("Absensi", "Review", "Absensi")%></li>
    <li>
        <%= Html.ActionLink("Customer", "Review", "Customer")%></li>
    <li>
        <%= Html.ActionLink("Pipeline", "Index", "Pipeline")%></li>
    <li>
        <%= Html.ActionLink("Calendar", "Review", "Calendar")%></li>
    <li>
        <%= Html.ActionLink("Report", "Index", "ReportSM")%></li>
</ul>
<%
        }
        else if (CommonModel.UserRole() == "BM")
        {
%>
<ul id="menu">
    <li>
        <%= Html.ActionLink("Home", "Index", "Home")%></li>
    <li>
        <%= Html.ActionLink("Sales", "Index", "SalesBM")%></li>
    <li>
        <%= Html.ActionLink("Absensi", "Review", "Absensi")%></li>
    <li>
        <%= Html.ActionLink("Customer", "Review", "Customer")%></li>
    <li>
        <%= Html.ActionLink("Pipeline", "Index", "Pipeline")%></li>
    <li>
        <%= Html.ActionLink("Calendar", "Review", "Calendar")%></li>
    <li>
        <%= Html.ActionLink("Report", "Index", "ReportBM")%></li>
</ul>
<%
        }
        else if (CommonModel.UserRole() == "ABM")
        {
%>
<ul id="menu">
    <li>
        <%= Html.ActionLink("Home", "Index", "Home")%></li>
    <li>
        <%= Html.ActionLink("Sales", "Index", "SalesABM")%></li>
    <li>
        <%= Html.ActionLink("Absensi", "Review", "Absensi")%></li>
    <li>
        <%= Html.ActionLink("Customer", "Review", "Customer")%></li>
    <li>
        <%= Html.ActionLink("Pipeline", "Index", "Pipeline")%></li>
    <li>
        <%= Html.ActionLink("Calendar", "Review", "Calendar")%></li>
    <li>
        <%= Html.ActionLink("Report", "Index", "ReportABM")%></li>
</ul>
<%
        }
        else if (CommonModel.UserRole() == "RBH")
        {
%>
<ul id="menu">
    <li>
        <%= Html.ActionLink("Home", "Index", "Home")%></li>
    <li>
        <%= Html.ActionLink("Sales", "Index", "SalesRBH")%></li>
    <li>
        <%= Html.ActionLink("Absensi", "Review", "Absensi")%></li>
    <li>
        <%= Html.ActionLink("Customer", "Review", "Customer")%></li>
    <li>
        <%= Html.ActionLink("Pipeline", "Index", "Pipeline")%></li>
    <li>
        <%= Html.ActionLink("Calendar", "Review", "Calendar")%></li>
    <li>
        <%= Html.ActionLink("Report", "Index", "ReportRBH")%></li>
</ul>
<%
        }

    }
    else
    {   
%>
<p>
    &nbsp;</p>
<% } %>