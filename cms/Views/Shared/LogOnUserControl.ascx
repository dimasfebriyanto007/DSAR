<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%
    if (Request.IsAuthenticated) {
%>
        Welcome <strong><%: ViewData["CurrentUserName"] %></strong> (<%: ViewData["CurrentUserRoleName"] %>)
        [ <%: Html.ActionLink("Change Password", "ChangePassword", "Account") %> | <%: Html.ActionLink("Log Off", "LogOff", "Account") %> ]
<%
    }
    else {
%> 
        [ <%: Html.ActionLink("Log On", "LogOn", "Account") %> ]
<%
    }
%>
