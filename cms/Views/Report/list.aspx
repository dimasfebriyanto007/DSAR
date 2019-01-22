<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<html>
<head>
<style>
table.report
{
border-collapse:collapse;
}
table.report td, table.report th 
{
border:1px solid #000;
background:#fff;
font-size:8pt;
}
table.report td.orange
{
background:#ffcc99;
color:#ff0021;
}
table.report td.strong 
{
    font-weight:bold;
}
table.report th 
{
border:1px solid #000;
font-weight:bold;
color:#000;
text-align:center;
background-color:#ffcc00;
}
table.report tr.total 
{
border-top:2px solid #000;
border-bottom:2px solid #000;
}
</style>
</head>
<body>
<table cellspacing="0" cellpadding="0" class="report" width="1500">  
  <tr>
    <th rowspan="2">Week</th>
    <th rowspan="2" style="width:75px">Date</th>
    <th rowspan="2" style="width:75px">Status</th>
    <th colspan="2"># of sales activities</th>
    <th colspan="2"># of    PIPELINE</th>
    <th colspan="2">SME Loan approval</th>
    <th colspan="9">Bookings</th>
    <th colspan="6"># of referrals</th>
    <th rowspan="2">No. of new CIFs</th>
  </tr>
  <tr>
    <th>Calls</th>
    <th>Visit</th>
    <th>Warm</th>
    <th>Hot</th>
    <th># of loan</th>
    <th>Volume (in mio IDR)</th>
    <th># of CA</th>
    <th>Volume (in mio IDR)</th>
    <th># of non-salary SA</th>
    <th>Volume (in mio IDR)</th>
    <th># of salary SA</th>
    <th>Volume (in mio IDR)</th>
    <th># of TD</th>
    <th>Volume (in mio IDR)</th>
    <th>Total (exclude salary SA)</th>
    <th>EB loan</th>
    <th>Autoloan</th>
    <th>Credit card</th>
    <th>Mortgage</th>
    <th>Bancass</th>
    <th>Total</th>
  </tr>

<%: Html.Raw(ViewBag.Output) %>

</body>
</html>