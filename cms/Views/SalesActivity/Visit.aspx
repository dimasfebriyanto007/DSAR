<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<cms.Models.Visit>" %>
<%@ Import Namespace="cms.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Add New Visit
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
            var strClass1 = '#reason_' + rel;
            var strBlankOption = '<option value=""></option>';
            var strBookingOption = '<option value="">--CHOOSE--</option>' + 
                                 '<option value="10">OPEN ACCOUNT/NTB</option>' +
                                '<option value="11">TOP UP FUND</option>' +
                                '<option value="16">OTHERS</option>';
            var strHotOption = '<option value="">--CHOOSE--</option>' + 
                                '<option value="12">SET BOOKING</option>' +
                                   '<option value="16">OTHERS</option>';
            var strWarmOption = '<option value="">--CHOOSE--</option>' + 
                                '<option value="13">SET VISIT 2nd ATTEMPT</option>' +
                                   '<option value="16">OTHERS</option>';
            var strColdOption = '<option value="">--CHOOSE--</option>' + 
                                '<option value="14">SET VISIT 3rd ATTEMPT</option>' +
                                '<option value="16">OTHERS</option>';
            var strCancelOption = '<option value="">--CHOOSE--</option>' + 
                                    '<option value="15">NOT INTERESTED (DROPPED)</option>' +
                                   '<option value="16">OTHERS</option>';
            if ($(this).val() == "HOT") {
                $(strClass1).html(strHotOption);
                var strBooking = getStrBooking(rel);
                $(strClass).html(strBooking);
            } else if ($(this).val() == "BOOKING") {
                $(strClass1).html(strBookingOption);
                var strBooking = getStrBooking(rel);
                $(strClass).html(strBooking);
            } else if ($(this).val() == "WARM") {
                $(strClass1).html(strWarmOption);
                $(strClass).html('');
            } else if ($(this).val() == "COLD") {
                $(strClass1).html(strColdOption);
                $(strClass).html('');
            } else if ($(this).val() == "CANCEL") {
                $(strClass1).html(strCancelOption);
                $(strClass).html('');
            } else {
                $(strClass1).html(strBlankOption);
                $(strClass).html('');
            }
        });


    }); 
</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h1>Add New Visit</h1>

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
