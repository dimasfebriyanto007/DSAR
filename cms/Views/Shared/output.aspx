<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%: Html.Raw(ViewBag.Output) %>

<script>
    $(function () {
        $("#checkall").click(function () {
            $(".cbox").attr("checked", this.checked);
        });

        $("#import").click(function () {
            $("#action").val("import");
            $("#CustImport").submit();
        });

        $("#delete").click(function () {
            $("#action").val("delete");
            $("#CustImport").submit();
        });

        $(".ajax").colorbox();
    });
</script>