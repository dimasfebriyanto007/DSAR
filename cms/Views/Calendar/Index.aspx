<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="cms.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Calendar
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
    <div style="float:left;width:20px;height:10px;margin-right:10px;background:#3366CC"></div>
    Call Activity
    <br />
    <div style="float:left;width:20px;height:10px;margin-right:10px;background:#B7005C"></div>
    Visit Activity
    <br />
</div>

<div id='calendar'></div>

<div class="legend">
    <br />
    <div style="float:left;width:20px;height:10px;margin-right:10px;background:#3366CC"></div>
    Call Activity
    <br />
    <div style="float:left;width:20px;height:10px;margin-right:10px;background:#B7005C"></div>
    Visit Activity    
</div>
</asp:Content>
