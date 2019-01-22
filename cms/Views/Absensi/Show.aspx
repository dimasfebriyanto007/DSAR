<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="cms.Models" %>
<script>

    $(document).ready(function () {

        var date = new Date(<%: ViewData["year"] %>,<%: ViewData["month"] %>,1);
        var d = date.getDate();
        var m = date.getMonth();
        var y = date.getFullYear();

        $('#calendar').fullCalendar({
            year : y,
            month : m-1,
            date : d,
            titleFormat : '\'<%: ViewData["Title"] %>\'',
            header: {
                left: '',
                center: 'title',
                right: ''
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
