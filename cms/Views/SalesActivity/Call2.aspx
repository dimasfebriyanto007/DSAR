<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="cms.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Add New Call
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
<script type="text/javascript" src="<%: Url.Content("~/Scripts/jquery-1.11.1.min.js") %>"></script>
<script type="text/javascript" src="<%: Url.Content("~/Scripts/jquery-ui.min.js") %>"></script>    
<script type="text/javascript" src="<%: Url.Content("~/Scripts/jquery-ui-timepicker-addon.js") %>"></script>
<script type="text/javascript" src="<%: Url.Content("~/Scripts/jquery-ui-sliderAccess.js") %>"></script>
<script type="text/javascript">
    $(function () {

        function getStrBooking(rel) {
            var strBooking = '<div class="editor-label"><label>No. CIF</label></div>' +
                             '<div class="editor-field"><input id="CIF_' + rel + '" name="CIF_' + rel + '" style="width:150px" type="text" value="" /></div>' +
                             '<div class="editor-label"><label>No. ACCT</label></div>' +
                             '<div class="editor-field"><input id="ACCT_' + rel + '" name="ACCT_' + rel + '" style="width:150px" type="text" value="" /></div>' +
                             '<div class="editor-label"><label>Amount</label></div>' +
                             '<div class="editor-field">Rp. <input id="Amount_' + rel + '" name="Amount_' + rel + '" style="width:100px" type="text" value="" /></div>';
            return strBooking;
        }

        $(".tanggal").datetimepicker({ dateFormat: 'yy-mm-dd' });
        $(".status").change(function () {
            var rel = $(this).attr('rel');
            var strClass = '#ifBooking_' + rel;
            if ($(this).val() == "BOOKING" || $(this).val() == "WARM" || $(this).val() == "HOT") {
                var strBooking = getStrBooking(rel);
                $(strClass).html(strBooking);
            } else {
                $(strClass).html('');
            }
        });

        $(".reason").change(function () {
            var strAllOption = '<option value="COLD">COLD</option>' +
                                '<option value="WARM">WARM</option>' +
                                '<option value="HOT">HOT</option>' +
                                '<option value="BOOKING">BOOKING</option>' +
                                '<option value="CANCEL">CANCEL</option>';

            var strPipelineOption = '<option value="WARM">WARM</option>' +
                                    '<option value="HOT">HOT</option>';

            var strBookOption = '<option value="BOOKING">BOOKING</option>';
            var rel = $(this).attr('rel');
            var strClass1 = '#status_' + rel;
            var strClass2 = '#ifBooking_' + rel;

            $(strClass2).html('');
            if ($(this).val() == "6") {
                $(strClass1).html(strPipelineOption);
                var strBooking = getStrBooking(rel);
                $(strClass2).html(strBooking);
            } else if ($(this).val() == "7") {
                $(strClass1).html(strBookOption);
                var strBooking = getStrBooking(rel);
                $(strClass2).html(strBooking);
            } else {
                $(strClass1).html(strAllOption);
            }
        });
    }); 
</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h1>Add New Call</h1>

<script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
<script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>

<% using (Html.BeginForm())
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
            <label>Product Offering</label>
        </div>
        <div class="editor-field">
            <%: Html.Raw(ViewBag.htmlProducts) %>
        </div>

        <br />
        <p>
            <input type="submit" value=" Save " />
        </p>
    </fieldset>
<% } %>

<div>
    <%: Html.ActionLink("Back to List", "Index") %>
</div>
</asp:Content>
