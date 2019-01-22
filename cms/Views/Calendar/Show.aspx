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
