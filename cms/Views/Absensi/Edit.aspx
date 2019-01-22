<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<cms.Models.Absensi>" %>
<%@ Import Namespace="cms.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Edit Absensi
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h1>Edit Absensi</h1>

<script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
<script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>

<% using (Html.BeginForm("Edit", "Absensi", FormMethod.Post, new { enctype = "multipart/form-data" }))
   { %>
    <%: Html.ValidationSummary(true) %>
    <fieldset>
        
        <% if (ViewData["Output"]!=null) { %><div class="error"><%= ViewData["Output"] %></div><% } %>
        <%: Html.Hidden("strDate",ViewData["Date"]) %>

        <div class="editor-label">
            <label>Tanggal Absensi</label>
        </div>
        <div class="editor-field">
            <%: ViewData["strDate"] %>
        </div>

        <div class="editor-label">
            <label>Reason</label>
        </div>
        <div class="editor-field">
            <select name="Reason" id="Reason">
                <%                               
                foreach (string opt in (IEnumerable<string>)ViewData["Reasons"])
                {
                    string selected = "";
                    if (ViewData["Reason"] != null)
                    {
                        selected = (opt == ViewData["Reason"].ToString()) ? " selected=\"selected\"" : "";
                    }
                    Response.Write("<option value=\"" + opt + "\"" + selected + ">" + opt + "</option>");
                }
                %>
            </select>
            <%: Html.ValidationMessageFor(model => model.Reason) %>
        </div>
        
        <p>
            <input type="submit" value="Save" />
        </p>
    </fieldset>
<% } %>

<div>
    <%: Html.ActionLink("Back to List", "Index") %>
</div>

</asp:Content>