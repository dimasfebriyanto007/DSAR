<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Logon.Master" Inherits="System.Web.Mvc.ViewPage<cms.Models.LogOnModel>" %>

<asp:Content ID="loginTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Daily Sales Activity Report
</asp:Content>

<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">
	<h1 style="font-size:2.6em;color:#fff">&nbsp; &nbsp; &nbsp;Daily Sales Activity Report (DSAR) Online</h1>
	
    <script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
    <script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>

    <div class="project-tiger">
    <img src="<%: Url.Content("~/Content/images/maybank.png") %>" border="0" />
    </div>

    <% using (Html.BeginForm("LogOn", "Account", FormMethod.Post, new { @class = "login-form", @style = "color:#fff" }))
       { %>
        <%: Html.ValidationSummary(true, "Login was unsuccessful. Please correct the errors and try again.") %>
                        
            <div class="editor-label">
                <label for="UserName">NPK (Input 10 Digit NPK)</label>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(m => m.UserName) %>
                <%: Html.ValidationMessageFor(m => m.UserName) %>
            </div>
                
            <div class="editor-label">
                <%: Html.LabelFor(m => m.Password) %>
            </div>
            <div class="editor-field">
                <%: Html.PasswordFor(m => m.Password) %>
                <%: Html.ValidationMessageFor(m => m.Password) %>
            </div>
                
            <div class="editor-label">
                <%: Html.CheckBoxFor(m => m.RememberMe) %>
                <%: Html.LabelFor(m => m.RememberMe) %>
            </div>

            <%--<div class="editor-label">
                <a href="<%: Url.Content("~/Account/Register") %>" class="homepage">Registration</a> | <a href="<%: Url.Content("~/Account/ForgetPassword") %>" class="homepage">Forget Password?</a>
            </div>--%>
                
            <p>
                <input type="submit" value="Log On" />
            </p>
        </div>
    <% } %>
</asp:Content>
