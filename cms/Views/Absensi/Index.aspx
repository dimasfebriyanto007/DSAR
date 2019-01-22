<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="cms.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Absensi
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
<link href="<%: Url.Content("~/Scripts/fullcalendar.css") %>" rel="stylesheet" type="text/css" />
<script src="<%: Url.Content("~/Scripts/fullcalendar.min.js") %>" type="text/javascript"></script>
<script src="<%: Url.Content("~/Scripts/jquery.qtip.min.js") %>" type="text/javascript"></script>
<script>

    $(document).ready(function () {

        var date = new Date();
        var d = date.getDate();
        var m = date.getMonth();
        var y = date.getFullYear();

        $('#calendar').fullCalendar({
            header: {
                left: 'prev,next today',
                center: 'title',
                right: 'month,basicWeek,basicDay'
            },
            editable: false,
            events: <%: Html.Raw(ViewBag.strJson) %>,
            eventRender: function(event, element) {
                            element.qtip({
                                content: event.tooltip,
                                position: {
                                     corner: {
                                        target: 'topCenter',
                                        tooltip: 'bottomLeft'
                                     }
                                  },
                                  style: {
                                     name: 'cream',
                                     padding: '7px 13px',
                                     width: {
                                        max: 250,
                                        min: 0
                                     },
                                     tip: true
                                  }
                            });
                        }
        });

    });

</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<div class="legend">
    <div style="float:left;width:20px;height:10px;margin-right:8px;background:#209316"></div>
    <div style="float:left;margin-right:20px;">Hadir</div>
    <div style="float:left;width:20px;height:10px;margin-right:8px;background:#F90000"></div>
    <div style="float:left;margin-right:20px;">Tidak Hadir</div>
    <div style="float:left;width:20px;height:10px;margin-right:8px;background:#0000FF"></div>
    <div style="float:left;margin-right:20px;">Ijin</div>
    <div class="clear"></div>
</div>

<div id='calendar'></div>

<div class="legend">
    <br />
    <div style="float:left;width:20px;height:10px;margin-right:8px;background:#209316"></div>
    <div style="float:left;margin-right:20px;">Hadir</div>
    <div style="float:left;width:20px;height:10px;margin-right:8px;background:#F90000"></div>
    <div style="float:left;margin-right:20px;">Tidak Hadir</div>
    <div style="float:left;width:20px;height:10px;margin-right:8px;background:#0000FF"></div>
    <div style="float:left;margin-right:20px;">Ijin</div>
    <div class="clear"></div>   
</div>
</asp:Content>
