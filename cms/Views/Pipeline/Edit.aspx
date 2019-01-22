<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<cms.Models.NasabahProduct>" %>
<%@ Import Namespace="cms.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Edit Pipeline
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
<script type="text/javascript">
    $(function () {
        $("#Status").change(function () {
            if ($(this).val() == "BOOKING") {
                $('#ifBooking').show();
            } else {
                $('#ifBooking').hide();
            }
        });
    }); 
</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h1>Edit Pipeline</h1>

<script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
<script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>

<% using (Html.BeginForm("Edit", "Pipeline", FormMethod.Post, new { enctype = "multipart/form-data" }))
   { %>
    <%: Html.ValidationSummary(true) %>
    <fieldset>
        
        <% if (ViewData["Output"]!=null) { %><div class="error"><%= ViewData["Output"] %></div><% } %>
        <%: Html.HiddenFor(model => model.Id) %>

        <table width="100%" border="0" cellspacing="1" cellpadding="6">
          <tr>
            <td width="20%" valign="top">
                <div class="editor-label">
                    <label>GCIF</label>
                </div>
                <div class="editor-field">
                    <%: Html.DisplayFor(model => model.Nasabah.GCIF)%>
                </div>

                <div class="editor-label">
                    <label>Name</label>
                </div>
                <div class="editor-field">
                    <%: Html.DisplayFor(model => model.Nasabah.Name)%>
                </div>
        
                <div class="editor-label">
                    <label>Produk</label>
                </div>
                <div class="editor-field">
                    <%: Html.DisplayFor(model => model.Product.Name)%>
                </div>

                <div class="editor-label">
                    <label>Status</label>
                </div>
                <div class="editor-field">
                    <select name="Status" id="Status">
                     <%                                     
                       foreach (PipelineOptionModel opt in (IEnumerable<PipelineOptionModel>)ViewBag.Status)
                       {
                           string selected = (opt.OptionId.Trim() == Model.Status.Trim()) ? " selected=\"selected\"" : "";
                           Response.Write("<option value=\"" + opt.OptionId + "\"" + selected + ">" + opt.OptionString + "</option>");
                       }
                     %>
                    </select>
                    <%: Html.ValidationMessageFor(model => model.Status) %>
                </div>                

                <div class="editor-label">
                    <label>Remark</label>
                </div>
                <div class="editor-field">
                    <%: Html.ValidationMessageFor(model => model.Note)%>
                    <%= Html.TextAreaFor(model => model.Note, new { @style = "width:300px;height:80px" })%> 
                </div>

            </td>
            <td valign="top">
                
                <div id="ifBooking" style="display:none">
                    <div class="editor-label">
                        <label>No. CIF</label>
                    </div>
                    <div class="editor-field">
                        <%: Html.TextBox("CIF", "", new { @style = "width:150px" })%>
                    </div>   

                    <div class="editor-label">
                        <label>No. ACCT</label>
                    </div>
                    <div class="editor-field">
                        <%: Html.TextBox("ACCT", "", new { @style = "width:150px" })%>
                    </div>

                    <div class="editor-label">
                        <label>Amount</label>
                    </div>
                    <div class="editor-field">
                        Rp. <%: Html.TextBox("Amount", "", new { @style = "width:100px" })%>
                    </div>
                </div>

            </td>
          </tr>
        </table>
        
        <p>
            <input type="submit" value="Save" />
        </p>
    </fieldset>
<% } %>

<div>
    <%: Html.ActionLink("Back to List", "Index") %>
</div>

</asp:Content>