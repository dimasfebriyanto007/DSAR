<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<cms.Models.SalesNasabah>" %>
<%@ Import Namespace="cms.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Refer the Customer to other Sales
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h1>Refer the Customer to other Sales</h1>

<script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
<script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>

<% using (Html.BeginForm("Reference", "SalesActivity", FormMethod.Post, new { enctype = "multipart/form-data" }))
   { %>
    <%: Html.ValidationSummary(true) %>
    <fieldset>
        <% if (ViewData["Output"]!=null) { %><div class="error"><%= ViewData["Output"] %></div><% } %>
        <input type="hidden" name="NasabahId" value="<%: ViewData["NasabahId"] %>" />

        <div class="editor-label">
            <label>Customer Name</label>
        </div>
        <div class="editor-field">
            <%: ViewData["NasabahName"] %>
        </div>

        <div class="editor-label">
            <label>Refer To</label>
        </div>
        <div class="editor-field">
            <%= Html.DropDownListFor(model => model.SalesId, new SelectList((IEnumerable<Object>) ViewData["Sales"], "SalesId", "Name"))%>
            <%: Html.ValidationMessageFor(model => model.SalesId) %>
        </div>

        <div class="editor-label">
            <label>Product Offering</label>
        </div>
        <div class="editor-field">
        <% if (ViewData["ProductName"] == null)
           { %>
            <%= Html.DropDownListFor(model => model.ProductId, new SelectList((IEnumerable<Product>)ViewData["Products"], "ProductId", "Name"))%>
            <%: Html.ValidationMessageFor(model => model.ProductId)%>
        <% }
           else
           {
               Response.Write("<div>" + ViewData["ProductName"] + "</div>");
               Response.Write(Html.HiddenFor(model => model.ProductId));
               Response.Write(Html.Hidden("From","Pipeline"));
           } %>
        </div>
                        

        <br />
        <p>
            <input type="submit" value=" Send " />
        </p>
    </fieldset>
<% } %>

<div>
    <%: Html.ActionLink("Back to List", "Index") %>
</div>
</asp:Content>
