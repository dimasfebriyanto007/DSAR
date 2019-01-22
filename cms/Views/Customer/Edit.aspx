<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<cms.Models.Nasabah>" %>
<%@ Import Namespace="cms.Models" %>


<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
Edit Customer
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
<script>
    $(function () {
        var today = new Date();
        var thisYear = today.getFullYear();
        var defaultYear = thisYear - 25;
        $('#BirthDate').datepicker({
            changeMonth: true,
            changeYear: true,
            dateFormat: 'yy-mm-dd',
            defaultDate: defaultYear + '-01-01'
        });
    });
</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <% if (Model.Status == "NEW" || CommonModel.UserRole() == "ADMIN")
       { %>
        <h1>Edit Customer</h1>
    <% } else { %>
        <h1>Review Customer <%: Model.Status %></h1>
        <h5>(hanya Customer dengan status selain Existing/Leads yang dapat diubah)</h5>
    <% } %>


<script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
<script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>

<% using (Html.BeginForm("Edit", "Customer", FormMethod.Post, new { enctype = "multipart/form-data" }))
   { %>
    <%: Html.ValidationSummary(true) %>
    <fieldset>
        
        <% if (ViewData["Output"]!=null) { %><div class="error"><%= ViewData["Output"] %></div><% } %>
        <%: Html.HiddenFor(model => model.NasabahId) %>

        <table width="100%" border="0" cellspacing="1" cellpadding="6">
          <tr>
            <td width="40%" valign="top">

                <div class="editor-label">
                    <label>Source of Customer</label>
                </div>
                <div class="editor-field">
                <% if (ViewData["Status"].ToString() == "NEW")
                    { %>
                        <select name="status">
                 <% } else 
                    {%>
                        <select name="status" disabled>
                <% } %>

                <% if (ViewData["Status"].ToString() == "EXISTING")
                    { %>
                        <option value="EXISTING" selected="selected">Leads</option>
                <% } else if (ViewData["Status"].ToString() == "REFER")
                    { %>
                        <option value="REFER" selected="selected">Referral</option>
                        <option value="WALK-IN">Walk-In</option>
                        <option value="EVENT">Event</option>
                        <option value="NEW">Others</option>   
                <% } else if (ViewData["Status"].ToString() == "WALK-IN")
                    { %>
                        <option value="REFER">Referral</option>
                        <option value="WALK-IN" selected="selected">Walk-In</option>
                        <option value="EVENT">Event</option>
                        <option value="NEW">Others</option>   
                <% } else if (ViewData["Status"].ToString() == "EVENT")
                    { %>
                        <option value="REFER">Referral</option>
                        <option value="WALK-IN">Walk-In</option>
                        <option value="EVENT" selected="selected">Event</option>
                        <option value="NEW">Others</option>                                                                                    
                    <% } else 
                    {%>
                        <option value="REFER">Referral</option>
                        <option value="WALK-IN">Walk-In</option>
                        <option value="EVENT">Event</option>
                        <option value="NEW" selected="selected">Others</option> 
                    <% } %>
                    </select>
                </div>

                <div class="editor-label">
                    <label>GCIF</label>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(model => model.GCIF, new { @style = "width:200px" })%>
                    <%: Html.ValidationMessageFor(model => model.GCIF) %>
                </div>

                <div class="editor-label">
                    <label>Name</label>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(model => model.Name, new { @style = "width:300px" })%>
                    <%: Html.ValidationMessageFor(model => model.Name) %>
                </div>
        
                <div class="editor-label">
                    <label>ID Number (KTP/SIM)</label>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(model => model.Status, new { @style = "width:200px" })%>
                    <%: Html.ValidationMessageFor(model => model.KtpId) %>
                </div>

                <div class="editor-label">
                    <label>Gender</label>
                </div>
                <div class="editor-field">
                    <select name="Gender">
                <% if (ViewData["Gender"].ToString() == "f")
                    { %>
                        <option value="m">Laki-laki</option>
                        <option value="f" selected="selected">Perempuan</option>
                <% }
                    else
                    { %>
                        <option value="m" selected="selected">Laki-laki</option>
                        <option value="f">Perempuan</option>                
                    <% } %>
                    </select>
                </div>

                <div class="editor-label">
                    <label>Birth Date</label>
                </div>
                <div class="editor-field">
                    <input id="BirthDate" name="BirthDate" readonly="readonly" style="width:90px" type="text" value="<%: Model.BirthDate.GetValueOrDefault().ToString("yyyy-MM-dd") %>" />
                    <%: Html.ValidationMessageFor(model => model.BirthDate)%>
                </div>                
                
            </td>
            <td valign="top">
                <div class="editor-label">
                    <label>Address</label>
                </div>
                <div class="editor-field">
                    <%: Html.ValidationMessageFor(model => model.Address)%>
                    <%= Html.TextAreaFor(model => model.Address, new { @style = "width:300px;height:80px" })%> 
                </div>

                <div class="editor-label">
                    <label>Home Phone</label>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(model => model.HomePhone, new { @style = "width:200px" })%>
                    <%: Html.ValidationMessageFor(model => model.HomePhone)%>
                </div>

                <div class="editor-label">
                    <label>Mobile Phone</label>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(model => model.MobilePhone, new { @style = "width:200px" })%>
                    <%: Html.ValidationMessageFor(model => model.MobilePhone)%>
                </div>
            </td>
          </tr>
        </table>
        
        <p>
            <% if (Model.Status != "EXISTING" || CommonModel.UserRole() == "ADMIN")
               { %>
            <input type="submit" value="Save"/>
            <% } %>
        </p>
    </fieldset>
<% } %>

<div>
    <%: Html.ActionLink("Back to List", "Index") %>
</div>

</asp:Content>