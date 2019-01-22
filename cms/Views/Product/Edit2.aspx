<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<cms.Models.Product>" %>
<%@ Import Namespace="cms.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Edit Product
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<ul id="tab">
	<li class="left active"><%: @Html.ActionLink("Product", "Index", "Product") %></li>
	<li class="left"><%: @Html.ActionLink("Region", "Index", "Region")%></li>
    <li class="left"><%: @Html.ActionLink("Area", "Index", "Area")%></li>
    <li class="left"><%: @Html.ActionLink("Branch", "Index", "Branch")%></li>
    <li class="left"><%: @Html.ActionLink("Call Reason", "Index", "CallReason")%></li>
    <li class="left"><%: @Html.ActionLink("Visit Reason", "Index", "VisitReason")%></li>
    <li class="left"><%: @Html.ActionLink("Sales Team", "Index", "SalesTeam")%></li>
</ul>

<ul class="tabNavigation">
    <li><%: @Html.ActionLink("Product", "Index", "Product", new { @class = "selected" })%></li>
    <li><%: @Html.ActionLink("Category", "Index", "ProductCategory")%></li>
    <li><%: @Html.ActionLink("Variant", "Index", "ProductVariant")%></li>
</ul>

<h1>Edit Product</h1>

<script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
<script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>

<% using (Html.BeginForm("Edit", "Product", FormMethod.Post, new { enctype = "multipart/form-data" }))
   { %>
    <%: Html.ValidationSummary(true) %>
    <fieldset>
        
        <% if (ViewData["Output"]!=null) { %><div class="error"><%= ViewData["Output"] %></div><% } %>
        <%: Html.HiddenFor(model => model.ProductId) %>

        <div class="editor-label">
            <label>Product Name</label>
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.Name, new { @style = "width:300px" })%>
            <%: Html.ValidationMessageFor(model => model.Name) %>
        </div>

        <div class="editor-label">
            <label>Product Code</label>
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.Code, new { @style = "width:150px" })%>
            <%: Html.ValidationMessageFor(model => model.Code)%>
        </div>
        
        <div class="editor-label">
            <label>Category</label>
        </div>
        <div class="editor-field">
            <%= Html.DropDownListFor(model => model.CategoryId, new SelectList((IEnumerable<ProductCategory>)ViewData["Categories"], "CategoryId", "Name"))%>
        </div>

        <div class="editor-label">
            <label>Detail Product</label>
        </div>
        <div class="editor-field">
            <%: Html.TextAreaFor(model => model.Description, new { @style = "width:900px;height:400px" })%>
            <%: Html.ValidationMessageFor(model => model.Description) %>
        </div>
        
        <p>
            <input type="submit" value="Save" />
        </p>
    </fieldset>
<% } %>

<div>
    <%: Html.ActionLink("Back to List", "Index") %>
</div>

<script type="text/javascript">
    window.onload = function () {
        var oFCKeditor = new FCKeditor('Description', '900', '400');
        oFCKeditor.BasePath = "<%: Url.Content("~/fckeditor/") %>";
        oFCKeditor.ReplaceTextarea();            
    }
</script>

</asp:Content>