<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<cms.Models.News>" %>
<%@ Import Namespace="cms.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Edit Informasi
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h1>Edit Informasi</h1>

<script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
<script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>

<% using (Html.BeginForm("Edit", "News", FormMethod.Post, new { enctype = "multipart/form-data" }))
   { %>
    <%: Html.ValidationSummary(true) %>
    <fieldset>
        
        <% if (ViewData["Output"]!=null) { %><div class="error"><%= ViewData["Output"] %></div><% } %>
        <%: Html.HiddenFor(model => model.NewsId) %>

        <div class="editor-label">
            <label>Title</label>
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.Title, new { @style = "width:700px" })%>
            <%: Html.ValidationMessageFor(model => model.Title) %>
        </div>

        <div class="editor-label">
            <label>Intro Text</label>
        </div>
        <div class="editor-field">
            <%: Html.ValidationMessageFor(model => model.IntroText) %>
            <%: Html.TextAreaFor(model => model.IntroText)%>           
        </div>
        
        <div class="editor-label">
            <label>Content Text</label>
        </div>
        <div class="editor-field">
            <%: Html.ValidationMessageFor(model => model.ContentText)%>
            <%= Html.TextAreaFor(model => model.ContentText)%>
        </div>

        <div class="editor-label">
            <%: Html.LabelFor(model => model.Publish) %>
        </div>
        <div class="editor-field">
            <%= Html.DropDownListFor(model => model.Publish, new SelectList((IEnumerable<SelectOptionModel>) ViewData["YesNoOption"], "OptionId", "OptionString"), new { @id = "Publish" })%>
            <%: Html.ValidationMessageFor(model => model.Publish) %>
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
        var oFCKeditor = new FCKeditor('IntroText', '900', '300');
        oFCKeditor.BasePath = "<%: Url.Content("~/fckeditor/") %>";
        oFCKeditor.ReplaceTextarea();
            
        var oFCKeditor2 = new FCKeditor('ContentText', '900', '400');
        oFCKeditor2.BasePath = "<%: Url.Content("~/fckeditor/") %>";
        oFCKeditor2.ReplaceTextarea();
    }
</script>

</asp:Content>