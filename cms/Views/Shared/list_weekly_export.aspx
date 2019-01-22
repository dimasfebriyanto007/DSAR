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
<table width="600" border="0" cellspacing="0" cellpadding="3" class="report">
    <tr>
    <td class="strong">Branch Code</td>
    <td class="orange"><%: ViewData["BranchCode"] %></td>
    <td class="strong">Branch Name</td>
    <td class="orange"><%: ViewData["BranchName"] %></td>
    </tr>
    <tr>
    <td class="strong" width="100">Role</td>
    <td class="orange"><%: ViewData["Role"] %></td>
    <td class="strong" width="100">Month</td>
    <td class="orange"><%: ViewData["Month"] %></td>
    </tr>    
</table>
<br />
<h3>Activity</h3>
<table cellspacing="0" cellpadding="0" class="report" width="100%">  
  <tr>
    <th rowspan="2">Sales Name</th>
    <th colspan="6"># of Call</th>
    <th colspan="6"># of Visit</th>
    <th colspan="6"># of Loan</th>
    <th colspan="6">Vol of Loan (in mio IDR)</th>
  </tr>
  <tr>
    <th>W1</th>
    <th>W2</th>
    <th>W3</th>
    <th>W4</th>
    <th>W5</th>
    <th>Total</th>
    <th>W1</th>
    <th>W2</th>
    <th>W3</th>
    <th>W4</th>
    <th>W5</th>
    <th>Total</th>
    <th>W1</th>
    <th>W2</th>
    <th>W3</th>
    <th>W4</th>
    <th>W5</th>
    <th>Total</th>
    <th>W1</th>
    <th>W2</th>
    <th>W3</th>
    <th>W4</th>
    <th>W5</th>
    <th>Total</th>
  </tr>

<%: Html.Raw(ViewBag.Output1) %>

<br />
<h3>Pipeline</h3>
<table cellspacing="0" cellpadding="0" class="report" width="70%">  
  <tr>
    <th rowspan="2">Sales Name</th>
    <th colspan="6">Hot</th>
    <th colspan="6">Warm</th>
  </tr>
  <tr>
    <th>W1</th>
    <th>W2</th>
    <th>W3</th>
    <th>W4</th>
    <th>W5</th>
    <th>Total</th>
    <th>W1</th>
    <th>W2</th>
    <th>W3</th>
    <th>W4</th>
    <th>W5</th>
    <th>Total</th>
  </tr>

<%: Html.Raw(ViewBag.Output2) %>

<br />
<h3>Bookings</h3>
<table cellspacing="0" cellpadding="0" class="report" width="100%">  
  <tr>
    <th rowspan="2">Sales Name</th>
    <th colspan="6"># CA</th>
    <th colspan="6"># Non Salary SA</th>
    <th colspan="6"># TD</th>
    <th colspan="6"># Salary SA</th>
  </tr>
  <tr>
    <th>W1</th>
    <th>W2</th>
    <th>W3</th>
    <th>W4</th>
    <th>W5</th>
    <th>Total</th>
    <th>W1</th>
    <th>W2</th>
    <th>W3</th>
    <th>W4</th>
    <th>W5</th>
    <th>Total</th>
    <th>W1</th>
    <th>W2</th>
    <th>W3</th>
    <th>W4</th>
    <th>W5</th>
    <th>Total</th>
    <th>W1</th>
    <th>W2</th>
    <th>W3</th>
    <th>W4</th>
    <th>W5</th>
    <th>Total</th>
  </tr>

<%: Html.Raw(ViewBag.Output3) %>

<br />
<h3>Bookings Volume</h3>
<table cellspacing="0" cellpadding="0" class="report" width="100%">  
  <tr>
    <th rowspan="2">Sales Name</th>
    <th colspan="6">Volume CA (in mio IDR)</th>
    <th colspan="6">Volume Non Salary SA (in mio IDR)</th>
    <th colspan="6">Volume TD (in mio IDR)</th>
    <th colspan="6">Volume Salary SA (in mio IDR)</th>
    <th colspan="6">No of New CIF</th>
  </tr>
  <tr>
    <th>W1</th>
    <th>W2</th>
    <th>W3</th>
    <th>W4</th>
    <th>W5</th>
    <th>Total</th>
    <th>W1</th>
    <th>W2</th>
    <th>W3</th>
    <th>W4</th>
    <th>W5</th>
    <th>Total</th>
    <th>W1</th>
    <th>W2</th>
    <th>W3</th>
    <th>W4</th>
    <th>W5</th>
    <th>Total</th>
    <th>W1</th>
    <th>W2</th>
    <th>W3</th>
    <th>W4</th>
    <th>W5</th>
    <th>Total</th>
    <th>W1</th>
    <th>W2</th>
    <th>W3</th>
    <th>W4</th>
    <th>W5</th>
    <th>Total</th>
  </tr>

<%: Html.Raw(ViewBag.Output4) %>

<br />
<h3>Refferals</h3>
<table cellspacing="0" cellpadding="0" class="report" width="100%">  
  <tr>
    <th rowspan="2">Sales Name</th>
    <th colspan="6">Autoloans</th>
    <th colspan="6">Credit Card</th>
    <th colspan="6">Mortgage</th>
    <th colspan="6">Bancass</th>
    <th colspan="6">EB Loan</th>
  </tr>
  <tr>
    <th>W1</th>
    <th>W2</th>
    <th>W3</th>
    <th>W4</th>
    <th>W5</th>
    <th>Total</th>
    <th>W1</th>
    <th>W2</th>
    <th>W3</th>
    <th>W4</th>
    <th>W5</th>
    <th>Total</th>
    <th>W1</th>
    <th>W2</th>
    <th>W3</th>
    <th>W4</th>
    <th>W5</th>
    <th>Total</th>
    <th>W1</th>
    <th>W2</th>
    <th>W3</th>
    <th>W4</th>
    <th>W5</th>
    <th>Total</th>
    <th>W1</th>
    <th>W2</th>
    <th>W3</th>
    <th>W4</th>
    <th>W5</th>
    <th>Total</th>
  </tr>

<%: Html.Raw(ViewBag.Output5) %>

</body>
</html>