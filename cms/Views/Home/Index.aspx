<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    DSAR Online
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">



   


    <center>
    <font color="red"><%:@ViewBag.RememberPassword%></font>
    <p>&nbsp;</p>    
    <div style="font-size:2em;line-height:40px;padding:120px 0 170px 0;">
    Selamat Datang di DSAR Online
    <br />(Daily Sales Activity Report)
    </div>    
    
    </center>
</asp:Content>
