<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<cms.Models.Nasabah>" %>
<%@ Import Namespace="cms.Models" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <title>Detail Customer</title>
    <link href="<%: Url.Content("~/Content/Site.css") %>" rel="stylesheet" type="text/css" />
    <script src="<%: Url.Content("~/Scripts/jquery-1.5.1.min.js") %>" type="text/javascript"></script>   
    <style>
        body.popup
        {
            background:#fff;
        }
    </style>
</head>
<body class="popup">
<h1><%: Html.DisplayFor(model => model.Name) %></h1>

<table width="600" border="0" cellspacing="1" cellpadding="6">
    <tr>
    <td width="40%" valign="top">
        <div class="editor-label">
            <label>GCIF</label>
        </div>
        <div class="editor-field">
            <%: Html.DisplayFor(model => model.GCIF) %>
        </div>
                
        <div class="editor-label">
            <label>ID Number (KTP/SIM)</label>
        </div>
        <div class="editor-field">
            <%: Html.DisplayFor(model => model.KtpId) %>
        </div>

        <div class="editor-label">
            <label>Gender</label>
        </div>
        <div class="editor-field">
        <% if (Model.Gender.ToString() == "f")
            { %>
                Perempuan
        <% }
            else
            { %>
                Laki-laki
            <% } %>
        </div>

        <div class="editor-label">
            <label>Birth Date</label>
        </div>
        <div class="editor-field">
            <%: Model.BirthDate.GetValueOrDefault().ToString("dd MMM yyyy") %>
        </div>                
                
    </td>
    <td valign="top">
        <div class="editor-label">
            <label>Address</label>
        </div>
        <div class="editor-field">
            <%: Html.DisplayFor(model => model.Address) %>
        </div>

        <div class="editor-label">
            <label>Home Phone</label>
        </div>
        <div class="editor-field">
            <%: Html.DisplayFor(model => model.HomePhone) %>
        </div>

        <div class="editor-label">
            <label>Mobile Phone</label>
        </div>
        <div class="editor-field">
            <%: Html.DisplayFor(model => model.MobilePhone) %>
        </div>

        <div class="editor-label">
            <label>Status</label>
        </div>
        <div class="editor-field">
            <%: Html.DisplayFor(model => model.Status) %>
        </div>
    </td>
    </tr>
</table>
<h2>Product</h2>
<%
    dsarEntities _db = new dsarEntities();
    var checkProduct = _db.NasabahProducts.Where(c => c.NasabahId == Model.NasabahId).OrderBy(c => c.Id);
    var checkStatusRekoProduct = _db.NasabahProducts.Where(c => c.NasabahId == Model.NasabahId && c.Status=="REKOMENDASI").OrderBy(c => c.Id);
    var checkStatusExisProduct = _db.NasabahProducts.Where(c => c.NasabahId == Model.NasabahId && c.Status == "EXISTING").OrderBy(c => c.Id);
     %>
<table class="grid-style" style="width:800px">
<thead><tr><th>No</th><th>Product</th><th>Product Name</th><th>Status</th><th>Sales</th><th>CIF</th><th>ACCT</th><th>Amount</th></tr></thead>
<tbody>
<%
    if (checkProduct.Count() > 0)
    {
        int i = 1;
        System.Globalization.NumberFormatInfo nfi = new System.Globalization.CultureInfo("en-US", false).NumberFormat;
        nfi.NumberDecimalSeparator = ",";
        nfi.NumberGroupSeparator = ".";
        nfi.NumberDecimalDigits = 0;
        
        foreach (NasabahProduct np in checkProduct)
        {
            string klas = (i % 2 == 0) ? "gridrow_alternate" : "gridrow";
            string strIcon = String.Empty;
            string imageId = string.Empty;
            string imageDesc = string.Empty;
            if (np.Note != null)
            {
                imageDesc = np.Status + np.Sale + " - " + np.Note; 
            }
            else 
            {
                imageDesc = np.Status;
            }
            
            
            if (checkStatusRekoProduct.Count() > 0)
            {
                switch (np.Status)
                {
                    case "BOOKING": imageId = "9" ;
                        break;
                    case "CANCEL": imageId = "10";
                        break;
                    case "COLD": imageId = "7";
                        break;
                    case "HOT": imageId = "8";
                        break;
                    case "WARM": imageId = "7";
                        break;
                    case "REKOMENDASI": imageId = "6";
                        break;
                    case "REFERRAL": imageId = "12";
                        break;  
                }                
            }
            else if (checkStatusExisProduct.Count() > 0)
            {
                imageId = "1";
            }
            else
            {
                switch (np.Status)
                {
                    case "BOOKING": imageId = "4";
                        break;
                    case "CANCEL": imageId = "5";
                        break;
                    case "COLD": imageId = "2";
                        break;
                    case "HOT": imageId = "3";
                        break;
                    case "WARM": imageId = "2";
                        break;
                    case "REFERRAL": imageId = "11";
                        break;                            
                }  
            }
              
                        
            switch (imageId)
            {
                case "1": strIcon = "<img src=\"" + Url.Content("~/Content/images/icon_existing.png") + "\" title=\"" + imageDesc + "\" />";
                    break;
                case "2": strIcon = "<img src=\"" + Url.Content("~/Content/images/icon_inisiatif_warm.png") + "\" title=\"" + imageDesc + "\" />";
                    break;
                case "3": strIcon = "<img src=\"" + Url.Content("~/Content/images/icon_inisiatif_hot.png") + "\" title=\"" + imageDesc + "\" />";
                    break;
                case "4": strIcon = "<img src=\"" + Url.Content("~/Content/images/icon_inisiatif_booking.png") + "\" title=\"" + imageDesc + "\" />";
                    break;
                case "5": strIcon = "<img src=\"" + Url.Content("~/Content/images/icon_inisiatif_cancel.png") + "\" title=\"" + imageDesc + "\" />";
                    break;
                case "6": strIcon = "<img src=\"" + Url.Content("~/Content/images/icon_rekomendasi_new.png") + "\" title=\"" + imageDesc + "\" />";
                    break;
                case "7": strIcon = "<img src=\"" + Url.Content("~/Content/images/icon_rekomendasi_warm.png") + "\" title=\"" + imageDesc + "\" />";
                    break;
                case "8": strIcon = "<img src=\"" + Url.Content("~/Content/images/icon_rekomendasi_hot.png") + "\" title=\"" + imageDesc + "\" />";
                    break;
                case "9": strIcon = "<img src=\"" + Url.Content("~/Content/images/icon_rekomendasi_booking.png") + "\" title=\"" + imageDesc + "\" />";
                    break;
                case "10": strIcon = "<img src=\"" + Url.Content("~/Content/images/icon_rekomendasi_cancel.png") + "\" title=\"" + imageDesc + "\" />";
                    break;
                case "11": strIcon = "<img src=\"" + Url.Content("~/Content/images/icon_inisiatif_reffer.png") + "\" title=\"" + imageDesc + "\" />";
                    break;
                case "12": strIcon = "<img src=\"" + Url.Content("~/Content/images/icon_rekomendasi_reffer.png") + "\" title=\"" + imageDesc + "\" />";
                    break;                    
            }                        

     %>
    <tr class="<%: klas %>">
        <td><%: i %></td>
        <td><%: np.Product.Name %></td>
        <td><%: (np.ProductVariant != null) ? np.ProductVariant.Name : "" %></td>
        <td><%: Html.Raw(strIcon) %></td>
        <td><%: (np.Sale!=null) ? np.Sale.Name : "" %></td>
        <td><%: np.CIF %></td>
        <td><%: np.ACCT %></td>
        <td><%: (np.Amount > 0) ? "Rp. " + np.Amount.GetValueOrDefault().ToString("N", nfi) : ""%></td>
    </tr>
<%
            i++;
        }
    }
     %>
</tbody>
</table>
<p>&nbsp;</p>
<table width="500" border="0" cellspacing="0" cellpadding="4">
  <tr>
    <th colspan="4" align="left" scope="col">Product Status Legend</th>
  </tr>
  <tr>
    <td width="10" align="center" valign="middle"><img src="<%: Url.Content("~/Content/images/icon_existing.png") %>" width="16" height="16" alt="Existing" /></td>
    <td>Existing Product</td>
    <td width="10" align="center" valign="middle"><img src="<%: Url.Content("~/Content/images/icon_rekomendasi_new.png") %>" width="16" height="16" alt="New" /></td>
    <td>Rekomendasi (NEW)</td>
  </tr>
  <tr>
    <td align="center" valign="middle"><img src="<%: Url.Content("~/Content/images/icon_inisiatif_warm.png") %>" width="16" height="16" alt="Warm" /></td>
    <td>Inisiatif dari sales (WARM)</td>
    <td align="center" valign="middle"><img src="<%: Url.Content("~/Content/images/icon_rekomendasi_warm.png") %>" width="16" height="16" alt="Warm" /></td>
    <td>Rekomendasi (WARM)</td>
  </tr>
  <tr>
    <td align="center" valign="middle"><img src="<%: Url.Content("~/Content/images/icon_inisiatif_hot.png") %>" width="16" height="16" alt="Hot" /></td>
    <td>Inisiatif dari sales (HOT)</td>
    <td align="center" valign="middle"><img src="<%: Url.Content("~/Content/images/icon_rekomendasi_hot.png") %>" width="16" height="16" alt="Hot" /></td>
    <td>Rekomendasi (HOT)</td>
  </tr>
  <tr>
    <td align="center" valign="middle"><img src="<%: Url.Content("~/Content/images/icon_inisiatif_booking.png") %>" width="16" height="16" alt="Booking" /></td>
    <td>Inisiatif dari sales (BOOKING)</td>
    <td align="center" valign="middle"><img src="<%: Url.Content("~/Content/images/icon_rekomendasi_booking.png") %>" width="16" height="16" alt="Booking" /></td>
    <td>Rekomendasi (BOOKING)</td>
  </tr>
  <tr>
    <td align="center" valign="middle"><img src="<%: Url.Content("~/Content/images/icon_inisiatif_cancel.png") %>" width="16" height="16" alt="Cancel" /></td>
    <td>Inisiatif dari sales (CANCEL)</td>
    <td align="center" valign="middle"><img src="<%: Url.Content("~/Content/images/icon_rekomendasi_cancel.png") %>" width="16" height="16" alt="Cancel" /></td>
    <td>Rekomendasi (CANCEL)</td>
  </tr>
</table>
</body>
</html>