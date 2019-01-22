<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="cms.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Import Data Customer
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
<script  type="text/javascript" src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>"></script>
<script  type="text/javascript" src="<%: Url.Content("~/Scripts/jquery.form.js") %>"></script>
<script  type="text/javascript">
    $(function () {
        $('#DoImport').ajaxForm({
            beforeSubmit: function () {
                $.blockUI({
                    theme: true,
                    overlayCSS: { backgroundColor: '#000',opacity: .95 },
                    title: 'Import Customer',
                    message: '<p>Importing customer data, please wait...</p>'               
                });
            },
            success: function (data) {
                if ($.trim(data) == "success") {
                    $.blockUI({
                        overlayCSS: { backgroundColor: '#000',opacity: .95 },
                        message: "Data customer berhasil diimport",
                        timeout: 3000,
                        theme: true
                    });

                    setTimeout('document.location = "<%: Url.Content("~/Customer/ImportResult") %>"',3000);
                } else {
                    $.blockUI({
                        overlayCSS: { backgroundColor: '#000',opacity: .95 },
                        message: data,
                        timeout: 5000,
                        theme: true
                    });
                    $('.blockOverlay').attr('title', 'Click to unblock').click($.unblockUI);
                }
            }
        });
    });
</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<ul id="tab">
	<li class="left active"><%: @Html.ActionLink("Import Data Customer", "Import", "Customer") %></li>
	<li class="left"><%: @Html.ActionLink("Hasil Import", "ImportResult", "Customer")%></li>    
</ul>

<h1>Import Data Customer</h1>

<script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
<script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>

<% using (Html.BeginForm("DoImport", "Customer", FormMethod.Post, new { enctype = "multipart/form-data", id = "DoImport" }))
   { %>
    <%: Html.ValidationSummary(true) %>
    <fieldset>
        <% if (ViewData["Output"]!=null) { %><div class="error"><%= ViewData["Output"] %></div><% } %>

        <div class="editor-label">
            <label>File Excel</label>
        </div>
        <div class="editor-field">
            <input type="file" name="excel" /><br />
            <br />
            Untuk mengimport data customer, silahkan gunakan contoh template berikut ini : <a href="<%: Url.Content("~/Content/customer_import.xls") %>"><strong>customer_import.xls</strong></a>
            <br /> Jika telah mendownload template tersebut, Anda <font color="red">tidak diperbolehkan</font> untuk :
            <ol>
                <li>Mengubah nama worksheet. Sheet yang berisi data customer harus bernama "Sheet1".</li>
                <li>Mengubah nama kolom.</li>
                <li>Mengubah format kolom.</li>
            </ol>
        </div>

        <br />
        <p>
            <input type="submit" value=" Start Import " />
        </p>
    </fieldset>
<% } %>

<div>
    <%: Html.ActionLink("Back to List", "Index") %>
</div>
</asp:Content>
