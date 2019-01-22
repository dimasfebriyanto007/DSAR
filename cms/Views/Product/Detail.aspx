<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<cms.Models.Product>" %>
<%@ Import Namespace="cms.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Detail Produk <%: Model.Name %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h1>Detail Produk <%: Model.Name %></h1>

<div style="text-align:center;margin:20px 10px">
    <img src="<%: Url.Content("~/images/product/" + Model.ImageName) %>.jpg" />
</div>

<div style="with:94%">
    <%: Html.Raw(Model.Description) %>
</div>

</asp:Content>