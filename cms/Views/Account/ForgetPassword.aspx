<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<cms.Models.Sale>" %>
<%@ Import Namespace="cms.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Forget Password
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h1>Forget Password</h1>

<% if (ViewData["Success"] != null)
   { %>
   <div class="success"><%= ViewData["Success"]%></div>
<% }
   else
   { %>

<script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
<script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>

<% using (Html.BeginForm("ForgetPassword", "Account", FormMethod.Post))
   { %>
    <%: Html.ValidationSummary(true)%>
    <fieldset>
        <% if (ViewData["Output"] != null)
           { %><br /><div class="error"><%= ViewData["Output"]%></div><% } %>

        <div class="editor-label">
            <label>Username / NPK</label>
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.Npk, new { @style = "width:200px" })%>
            <%: Html.ValidationMessageFor(model => model.Npk)%>
        </div>        

        <br />
        <p>
            <input type="submit" value=" Kirim " />
        </p>
    </fieldset>
<% }

}
   %>

</asp:Content>
