using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using cms.Models;
using System.Linq;
using System.Net.Mail;
using System.Security.AccessControl;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;

public class CommonModel
{
    public static bool BelumAbsen()
    {
        dsarEntities _db = new dsarEntities();
        User user = CommonModel.GetCurrentUser();

        if (CommonModel.UserRole() != "SALES") return false;

        var checkAbsen = _db.Absensis.Where(c => c.SalesId == user.RelatedId && c.DateAbsen.Year == DateTime.Now.Year && c.DateAbsen.Month == DateTime.Now.Month && c.DateAbsen.Day == DateTime.Now.Day);
        if (checkAbsen.Count() > 0)
        {
            return false;
        }

        return true;
    }

    public static bool ProfileNotUpdated()
    {
        dsarEntities _db = new dsarEntities();
        User user = CommonModel.GetCurrentUser();

        if (CommonModel.UserRole() != "SALES") return false;

        var checkProfil = _db.Sales.Where(c => c.SalesId == user.RelatedId);
        if (checkProfil.Count() > 0)
        {
            Sale salesInfo = checkProfil.First();
            if (String.IsNullOrWhiteSpace(salesInfo.Name)) return true;
            if (String.IsNullOrWhiteSpace(salesInfo.Email)) return true;
            if (String.IsNullOrWhiteSpace(salesInfo.Phone)) return true;

            var checkSM = _db.SalesManagers.Where(c => c.BranchCode == salesInfo.BranchCode);
            if (checkSM.Count() > 0)
            {
                if (salesInfo.SmId == null || salesInfo.SmId <= 0) return true;
            }

            return false;
        }

        return true;
    }

    public static User GetCurrentUser()
    {
        dsarEntities _db = new dsarEntities();

        MembershipUser MUser = Membership.GetUser();
        User CurrentUser = new User();
        if (MUser != null)
        {
            string CUserName = MUser.UserName;
            CurrentUser = _db.Users.Where(c => c.UserName == CUserName).First();
        }

        return CurrentUser;
    }

    public static string UserRole()
    {
        dsarEntities _db = new dsarEntities();

        MembershipUser MUser = Membership.GetUser();
        string CUserName = MUser.UserName;
        User CurrentUser = _db.Users.Where(c => c.UserName == CUserName).First();

        return CurrentUser.Role;
    }

    public static UserDetailModel GetUserDetail(string CUserName)
    {
        dsarEntities _db = new dsarEntities();
        UserDetailModel userDetail = new UserDetailModel();

        if (!String.IsNullOrWhiteSpace(CUserName))
        {
            User CurrentUser = _db.Users.Where(c => c.UserName == CUserName).First();
            switch (CurrentUser.Role)
            {
                case "SALES":
                    Sale sales = _db.Sales.Where(c => c.SalesId == CurrentUser.RelatedId).FirstOrDefault();
                    if (sales != null)
                    {
                        userDetail.NPK = (sales.Npk != null) ? sales.Npk : String.Empty;
                        userDetail.Role = CurrentUser.Role;
                        userDetail.Name = sales.Name;
                        userDetail.Email = sales.Email;
                        userDetail.Phone = sales.Phone;
                    }
                    break;
                case "SM":
                    SalesManager sm = _db.SalesManagers.Where(c => c.ManagerId == CurrentUser.RelatedId).FirstOrDefault();
                    if (sm != null)
                    {
                        userDetail.NPK = sm.Npk;
                        userDetail.Role = CurrentUser.Role;
                        userDetail.Name = sm.Name;
                        userDetail.Email = sm.Email;
                        userDetail.Phone = sm.Phone;
                    }
                    break;
                case "BM":
                    BranchManager bm = _db.BranchManagers.Where(c => c.BmId == CurrentUser.RelatedId).FirstOrDefault();
                    if (bm != null)
                    {
                        userDetail.NPK = bm.Npk;
                        userDetail.Role = CurrentUser.Role;
                        userDetail.Name = bm.Name;
                        userDetail.Email = bm.Email;
                        userDetail.Phone = bm.Phone;
                    }
                    break;
                case "ABM":
                    ABM abm = _db.ABMs.Where(c => c.AbmId == CurrentUser.RelatedId).FirstOrDefault();
                    if (abm != null)
                    {
                        userDetail.NPK = abm.Npk;
                        userDetail.Role = CurrentUser.Role;
                        userDetail.Name = abm.Name;
                        userDetail.Email = abm.Email;
                        userDetail.Phone = abm.Phone;
                    }
                    break;
                case "RBH":
                    RBH rbh = _db.RBHs.Where(c => c.RbhId == CurrentUser.RelatedId).FirstOrDefault();
                    if (rbh != null)
                    {
                        userDetail.NPK = rbh.Npk;
                        userDetail.Role = CurrentUser.Role;
                        userDetail.Name = rbh.Name;
                        userDetail.Email = rbh.Email;
                        userDetail.Phone = rbh.Phone;
                    }
                    break;
                case "ADMIN":
                    Admin Admin = _db.Admins.Where(c => c.UserID == CurrentUser.UserId).FirstOrDefault();
                    if (Admin != null)
                    {
                        //userDetail.NPK = Admin.NIK;
                        userDetail.NPK = "System Admin";

                        userDetail.Role = CurrentUser.Role;
                        userDetail.Name = CUserName;
                        //userDetail.Name = Admin.Name;
                        userDetail.Email = "";
                        userDetail.Phone = "";
                    }
                    break;
                case "ADMINUSER":
                    Admin ADMINUSER = _db.Admins.Where(c => c.UserID == CurrentUser.UserId).FirstOrDefault();
                    if (ADMINUSER != null)
                    {
                        userDetail.NPK = ADMINUSER.NIK;
                        userDetail.Role = CurrentUser.Role;
                        userDetail.Name = ADMINUSER.Name;
                        userDetail.Email = "";
                        userDetail.Phone = "";
                    }
                    break;
                case "ADMINBUSSINES":
                    Admin ADMINBUSSINES = _db.Admins.Where(c => c.UserID == CurrentUser.UserId).FirstOrDefault();
                    if (ADMINBUSSINES != null)
                    {
                        userDetail.NPK = ADMINBUSSINES.NIK;
                        userDetail.Role = CurrentUser.Role;
                        userDetail.Name = ADMINBUSSINES.Name;
                        userDetail.Email = "";
                        userDetail.Phone = "";
                    }
                    break;
            }
        }

        return userDetail;
    }


    public static void UpdateProductStatus(long NasabahId)
    {
        dsarEntities _db = new dsarEntities();

        var data = (from c in _db.Nasabahs select c).Where(c => c.NasabahId == NasabahId);
        if (data.Count() > 0)
        {

            Nasabah item = data.First();

            var checkProduk = _db.Products.OrderBy(c => c.ProductId);

            string ProductStatus = String.Empty;

            foreach (Product pr in checkProduk)
            {
                var checkNP = _db.NasabahProducts.Where(c => c.NasabahId == item.NasabahId && c.ProductId == pr.ProductId).OrderByDescending(c => c.LastUpdate);
                if (checkNP.Count() > 0)
                {
                    NasabahProduct np = checkNP.First();

                    string strReferal = "";
                    var checkRef = _db.SalesNasabahs.Where(c => c.NasabahId == item.NasabahId && c.IsReferral == 1 && c.ProductId == pr.ProductId);
                    if (checkRef.Count() > 0)
                    {
                        var sn = checkRef.First();
                        Sale salesFrom = _db.Sales.Where(c => c.SalesId == sn.RefferalFrom).FirstOrDefault();
                        Sale salesTo = _db.Sales.Where(c => c.SalesId == sn.SalesId).FirstOrDefault();
                        strReferal = " - REFERRAL FROM " + salesFrom.Name + " TO " + salesTo.Name;
                    }

                    string salesName = (np.SalesId != null) ? np.Sale.Name + " (" + np.Status + ")" + strReferal : np.Status;
                    string imageId = "";

                    if (np.SalesId > 0)
                    {
                        if (item.Status == "NEW")
                        {
                            switch (np.Status)
                            {
                                case "REKOMENDASI": imageId = "6"; break;
                                case "EXISTING": imageId = "1"; break;
                                case "WARM": imageId = "2"; break;
                                case "HOT": imageId = "3"; break;
                                case "BOOKING": imageId = "4"; break;
                                case "CANCEL": imageId = "5"; break;
                            }
                        }
                        else
                        {
                            switch (np.Status)
                            {
                                case "REKOMENDASI": imageId = "6"; break;
                                case "EXISTING": imageId = "1"; break;
                                case "WARM": imageId = "7"; break;
                                case "HOT": imageId = "8"; break;
                                case "BOOKING": imageId = "9"; break;
                                case "CANCEL": imageId = "10"; break;
                            }
                        }
                    }
                    else
                    {
                        imageId = "1";
                    }

                    ProductStatus += pr.ProductId + ":" + np.SalesId + ":" + imageId + ":" + salesName + ",";
                }



            }

            ProductStatus = ProductStatus.TrimEnd(',');
            item.ProductStatus = ProductStatus;

            _db.ApplyCurrentValues(item.EntityKey.EntitySetName, item);
            _db.SaveChanges();

        }

    }


    public static void ResizeAndSave(string savePath, string fileName, Stream imageBuffer, int maxSideSize, bool makeItSquare, string format = "jpg")
    {
        int newWidth;
        int newHeight;
        Image image = Image.FromStream(imageBuffer);
        int oldWidth = image.Width;
        int oldHeight = image.Height;
        Bitmap newImage;
        if (makeItSquare)
        {
            int smallerSide = oldWidth >= oldHeight ? oldHeight : oldWidth;
            double coeficient = maxSideSize / (double)smallerSide;
            newWidth = Convert.ToInt32(coeficient * oldWidth);
            newHeight = Convert.ToInt32(coeficient * oldHeight);
            Bitmap tempImage = new Bitmap(image, newWidth, newHeight);
            int cropX = (newWidth - maxSideSize) / 2;
            int cropY = (newHeight - maxSideSize) / 2;
            newImage = new Bitmap(maxSideSize, maxSideSize);
            Graphics tempGraphic = Graphics.FromImage(newImage);
            tempGraphic.SmoothingMode = SmoothingMode.AntiAlias;
            tempGraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            tempGraphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            tempGraphic.DrawImage(tempImage, new Rectangle(0, 0, maxSideSize, maxSideSize), cropX, cropY, maxSideSize, maxSideSize, GraphicsUnit.Pixel);
        }
        else
        {
            int maxSide = oldWidth >= oldHeight ? oldWidth : oldHeight;

            if (maxSide > maxSideSize)
            {
                double coeficient = maxSideSize / (double)maxSide;
                newWidth = Convert.ToInt32(coeficient * oldWidth);
                newHeight = Convert.ToInt32(coeficient * oldHeight);
            }
            else
            {
                newWidth = oldWidth;
                newHeight = oldHeight;
            }
            newImage = new Bitmap(image, newWidth, newHeight);
        }

        if (format == "png")
        {
            newImage.Save(savePath + fileName + ".png", ImageFormat.Png);
        }
        else
        {
            newImage.Save(savePath + fileName + ".jpg", ImageFormat.Jpeg);
        }

        image.Dispose();
        newImage.Dispose();
    }


    public static void ResizeAndSave2(string savePath, string fileName, Stream imageBuffer, int maxWidth, int maxHeight, string format = "jpg")
    {
        int newWidth;
        int newHeight;
        Image image = Image.FromStream(imageBuffer);
        int oldWidth = image.Width;
        int oldHeight = image.Height;
        Bitmap newImage;
        //int maxSide = oldWidth >= oldHeight ? oldWidth : oldHeight;

        if (oldWidth > maxWidth)
        {
            double coeficient = maxWidth / (double)oldWidth;
            newWidth = Convert.ToInt32(coeficient * oldWidth);
            newHeight = Convert.ToInt32(coeficient * oldHeight);
        }
        else
        {
            newWidth = oldWidth;
            newHeight = oldHeight;
        }

        if (newHeight > maxHeight)
        {
            double coeficient = maxHeight / (double)newHeight;
            newWidth = Convert.ToInt32(coeficient * newWidth);
            newHeight = Convert.ToInt32(coeficient * newHeight);
        }

        newImage = new Bitmap(image, newWidth, newHeight);

        if (format == "png")
        {
            newImage.Save(savePath + fileName + ".png", ImageFormat.Png);
        }
        else
        {
            newImage.Save(savePath + fileName + ".jpg", ImageFormat.Jpeg);
        }

        image.Dispose();
        newImage.Dispose();
    }

    public static void ResizeAndCrop(string savePath, string fileName, Stream imageBuffer, int desWidth, int desHeight, string format = "jpg")
    {
        int x, y, w, h;
        Image image = Image.FromStream(imageBuffer);

        if (image.Height > image.Width)
        {
            w = (image.Width * desHeight) / image.Height;
            h = desHeight;
            x = (desWidth - w) / 2;
            y = 0;
        }
        else
        {
            w = desWidth;
            h = (image.Height * desWidth) / image.Width;
            x = 0;
            y = (desHeight - h) / 2;
        }

        Bitmap bmp = new Bitmap(desWidth, desHeight);

        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(image, x, y, w, h);
        }

        if (format == "png")
        {
            bmp.Save(savePath + fileName + ".png", ImageFormat.Png);
        }
        else
        {
            bmp.Save(savePath + fileName + ".jpg", ImageFormat.Jpeg);
        }

        image.Dispose();
        bmp.Dispose();
    }

    public static string FriendlyString(string phrase)
    {
        byte[] bytes = Encoding.GetEncoding("Cyrillic").GetBytes(phrase);
        string str = Encoding.ASCII.GetString(bytes);
        str = str.ToLower();
        str = Regex.Replace(str, @"[^a-z0-9\s]", ""); // Remove all non valid chars          
        str = Regex.Replace(str, @"\s+", " ").Trim(); // convert multiple spaces into one space  
        str = Regex.Replace(str, @"\s", "-"); // //Replace spaces by dashes
        return str;
    }

    public static string Truncate(string s, int length, bool atWord, bool addEllipsis)
    {
        // Return if the string is less than or equal to the truncation length
        if (s == null || s.Length <= length)
            return s;

        // Do a simple tuncation at the desired length
        string s2 = s.Substring(0, length);

        // Truncate the string at the word
        if (atWord)
        {
            // List of characters that denote the start or a new word (add to or remove more as necessary)
            List<char> alternativeCutOffs = new List<char>() { ' ', ',', '.', '?', '/', ':', ';', '\'', '\"', '\'', '-' };

            // Get the index of the last space in the truncated string
            int lastSpace = s2.LastIndexOf(' ');

            // If the last space index isn't -1 and also the next character in the original
            // string isn't contained in the alternativeCutOffs List (which means the previous
            // truncation actually truncated at the end of a word),then shorten string to the last space
            if (lastSpace != -1 && (s.Length >= length + 1 && !alternativeCutOffs.Contains(s.ToCharArray()[length])))
                s2 = s2.Remove(lastSpace);
        }

        // Add Ellipsis if desired
        if (addEllipsis)
            s2 += "...";

        return s2;
    }


    public static void CreateDirectory(DirectoryInfo dirInfo)
    {
        if (!dirInfo.Parent.Exists) CreateDirectory(dirInfo.Parent);
        if (!dirInfo.Exists) dirInfo.Create();
        /*FileSystemAccessRule everyOne = new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, AccessControlType.Allow);
        DirectorySecurity dirSecurity = new DirectorySecurity(dirInfo.Name, AccessControlSections.Group);
        dirSecurity.AddAccessRule(everyOne);
        Directory.SetAccessControl(dirInfo.Name, dirSecurity);*/
    }


    public static string GenerateRandomCode(int length = 6)
    {
        // For generating random numbers.
        Random random = new Random();

        string s = "";
        for (int i = 0; i < length; i++)
            s = String.Concat(s, random.Next(10).ToString());
        return s;
    }

    public static bool isEmail(string inputEmail = "")
    {
        string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
              @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
              @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
        Regex re = new Regex(strRegex);
        if (re.IsMatch(inputEmail))
            return (true);
        else
            return (false);
    }

    public static bool isNumber(string inputString = "")
    {
        string strRegex = "^[0-9]+$";
        Regex re = new Regex(strRegex);
        if (re.IsMatch(inputString))
            return (true);
        else
            return (false);
    }

    public static bool SendMail(string txtTo, string txtSubject, string txtBody, string txtFrom = "")
    {
        txtFrom = (String.IsNullOrEmpty(txtFrom)) ? ConfigurationManager.AppSettings["SmtpUser"] : txtFrom;
        MailAddress from = new MailAddress(txtFrom);
        MailAddress to = new MailAddress(txtTo);
        MailMessage message = new MailMessage(from, to);
        message.IsBodyHtml = true;
        message.Subject = txtSubject;
        message.Body = txtBody;
        SmtpClient client = new SmtpClient();

        client.Host = ConfigurationManager.AppSettings["SmtpHost"];
        client.Credentials = new System.Net.NetworkCredential
             (ConfigurationManager.AppSettings["SmtpUser"], ConfigurationManager.AppSettings["SmtpPass"]);
        //client.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["SmtpSsl"]); // remarked by Aditia - 20160808 

        try
        {
            client.Send(message);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static void WriteToFile(string strPath, string content)
    {

        System.IO.StreamWriter file = new System.IO.StreamWriter(strPath);
        file.WriteLine(content);

        file.Close();
    }

    public static string StripHTML(string htmlString)
    {
        if (htmlString == null) return htmlString;
        string pattern = @"<(.|\n)*?>";
        return Regex.Replace(htmlString, pattern, string.Empty);
    }


    public static DateTime GetFirstDayOfMonth(DateTime dtDate)
    {
        DateTime dtFrom = dtDate;
        dtFrom = dtFrom.AddDays(-(dtFrom.Day - 1));
        return dtFrom;
    }


    public static DateTime GetFirstDayOfMonth(int iMonth)
    {
        DateTime dtFrom = new DateTime(DateTime.Now.Year, iMonth, 1);
        dtFrom = dtFrom.AddDays(-(dtFrom.Day - 1));
        return dtFrom;

    }


    public static DateTime GetLastDayOfMonth(DateTime dtDate)
    {
        DateTime dtTo = dtDate;
        dtTo = dtTo.AddMonths(1);
        dtTo = dtTo.AddDays(-(dtTo.Day));
        return dtTo;
    }


    public static DateTime GetLastDayOfMonth(int iMonth)
    {
        DateTime dtTo = new DateTime(DateTime.Now.Year, iMonth, 1);
        dtTo = dtTo.AddMonths(1);
        dtTo = dtTo.AddDays(-(dtTo.Day));
        return dtTo;
    }

    public static string Nl2br(string text, bool isXhtml = true)
    {
        Regex LineEnding = new Regex(@"(\r\n|\r|\n)+");
        var encodedText = HttpUtility.HtmlEncode(text);
        var replacement = isXhtml ? "<br /><br />" : "<br><br>";
        return LineEnding.Replace(encodedText, replacement);
    }

    public static string Br2ln(string text)
    {
        text = text.Replace("<br/>", "\r\n");
        text = text.Replace("<br />", "\r\n");
        text = text.Replace("<br>", "\r\n");
        return text;
    }


    public static string CalculateAge(DateTime birthDate)
    {
        DateTime currentDate = DateTime.Now;
        int years = currentDate.Year - birthDate.Year;
        int months = 0;
        int days = 0;

        // Check if the last year, was a full year.
        if (currentDate < birthDate.AddYears(years) && years != 0)
        {
            years--;
        }

        // Calculate the number of months.
        birthDate = birthDate.AddYears(years);

        if (birthDate.Year == currentDate.Year)
        {
            months = currentDate.Month - birthDate.Month;
        }
        else
        {
            months = (12 - birthDate.Month) + currentDate.Month;
        }

        // Check if last month was a complete month.
        if (currentDate < birthDate.AddMonths(months) && months != 0)
        {
            months--;
        }

        // Calculate the number of days.
        birthDate = birthDate.AddMonths(months);

        days = (currentDate - birthDate).Days;

        return years + " tahun " + months + " bulan";
    }

    public static string GenerateRandomString(int Length)
    {
        string _allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
        Random randNum = new Random();
        char[] chars = new char[Length];
        int allowedCharCount = _allowedChars.Length;

        for (int i = 0; i < Length; i++)
        {
            chars[i] = _allowedChars[(int)((_allowedChars.Length) * randNum.NextDouble())];
        }

        return new string(chars);
    }


    //---- add rdf
    public static string inActiveUser3wrongpass(string userName)
    {
        string message = string.Empty;
        dsarEntities _db = new dsarEntities();
        var todaysDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
        var LoginCount = (from c in _db.UserLoginLogs select c).Where(c => c.UserName == userName && c.LoginDate.Value.Day == DateTime.Now.Day && c.LoginDate.Value.Month == DateTime.Now.Month && c.LoginDate.Value.Year == DateTime.Now.Year && c.Status == false).Count();
        if (LoginCount >= 3)
        {
            dsarEntities _db2 = new dsarEntities();
            var Update = (from c in _db2.Users select c).Where(c => c.UserName == userName).FirstOrDefault();
            if (Update != null)
            {
                Update.IsLockedOut = true;
                _db2.SaveChanges();
            }
            return message = "Anda sudah salah 3 kali login dan Npk " + userName + " di nonaktifkan harap hubungi administrator";
        }
        return message;
    }

    public static void ActiveUser3wrongpass(string userName)
    {
        dsarEntities _db = new dsarEntities();
        var LoginCount = (from c in _db.UserLoginLogs select c).Where(c => c.UserName == userName && c.LoginDate.Value.Day == DateTime.Now.Day && c.LoginDate.Value.Month == DateTime.Now.Month && c.LoginDate.Value.Year == DateTime.Now.Year && c.Status == false);
        if (LoginCount != null)
        {
            foreach (var item in LoginCount)
            {
                dsarEntities _db2 = new dsarEntities();
                var data = (from a in _db2.UserLoginLogs select a).Where(a => a.Id == item.Id).FirstOrDefault();
                data.Status = true;
                _db2.SaveChanges();
            }
        }

    }
    public static bool getChangePasswordfor6time(string username, string Password)
    {
        dsarEntities _db = new dsarEntities();
        var data = (from c in _db.ChangePasswordLogs select c).Where(c => c.UserName == username).OrderByDescending(c => c.Date).Take(6);

        if (data.Where(b => b.Password == Password).Count() > 0)
        {
            return false;
        }

        return true;
    }
    public static bool checkLoginStatus(string username)
    {
        string mac = GetMACAddress();


        dsarEntities _db = new dsarEntities();

        var data = (from c in _db.UserLoginLogs select c).Where(c => c.UserName == username && c.Status == true).OrderByDescending(c => c.LoginDate).FirstOrDefault();
        if (data != null)
        {
            if (data.Address != mac && data.StatusLogin == true)
            {
                return false;
            }
        }

        return true;
    }
    public static string GetMACAddress()
    {
        //IPHostEntry Host = default(IPHostEntry);
        //string Hostname = null;
        //string IPAddress = string.Empty;
        //Hostname = System.Environment.MachineName;
        //Host = Dns.GetHostEntry(Hostname);
        //foreach (IPAddress IP in Host.AddressList)
        //{

        //    if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
        //    {
        //        IPAddress = IP.ToString();
        //    }
        //}

        //string macAddresses = string.Empty;

        //foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
        //{
        //    if (nic.OperationalStatus == OperationalStatus.Up)
        //    {
        //        macAddresses += nic.GetPhysicalAddress().ToString();
        //        break;
        //    }
        //}
        //string info = Hostname + "/" + IPAddress + "/" + macAddresses;
        //return info;

        //return  System.Security.Principal.WindowsIdentity.GetCurrent().Name;


        //return string.Format("|Hostname:{0}|IPAddress:{1}|GetMAC:{2}", Hostname,IPAddress);



        //string[] computer_name = System.Net.Dns.GetHostEntry(HttpContext.Current.Request.ServerVariables["REMOTE_HOST"]).HostName.Split(new Char[] { '.' });
        //string computer_name = System.Net.Dns.GetHostEntry(HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]).AddressList.Last().ToString();
        string computer_name = System.Net.Dns.GetHostEntry(HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]).HostName;



        return computer_name;

    }

    public static string GetParam(int ID)
    {
        string Param = string.Empty;
        dsarEntities _db = new dsarEntities();
        var data = (from c in _db.MaterParameters select c).Where(c => c.Id == ID).FirstOrDefault();
        Param = data.ParameterValue;
        return Param;
    }
    public static bool checkEXPPass(string username)
    {
        string Param = CommonModel.GetParam(1);
        if (!string.IsNullOrEmpty(Param))
        {
            int Day = Convert.ToInt32(Param) * -1;

            DateTime DateNow5 = DateTime.Now.AddDays(Day);
            dsarEntities _db = new dsarEntities();
            var data = (from c in _db.ChangePasswordLogs select c).Where(c => c.UserName == username).OrderByDescending(c => c.Date).FirstOrDefault();
            if (data != null)
            {
                if ((data.Date.Value) <= DateNow5)
                {
                    return true;
                }
            }
        }




        return false;
    }

    public static bool rememberPassword(string username)
    {

        string Param = CommonModel.GetParam(1);
        string Param2 = CommonModel.GetParam(4);
        if (!string.IsNullOrEmpty(Param))
        {
            int Day = (Convert.ToInt32(Param) - Convert.ToInt32(Param2)) * -1;

            DateTime DateNow5 = DateTime.Now.AddDays(Day);

            dsarEntities _db = new dsarEntities();
            var data = (from c in _db.ChangePasswordLogs select c).Where(c => c.UserName == username).OrderByDescending(c => c.Date).FirstOrDefault();
            if (data.Date.Value <= DateNow5)
            {
                return true;
            }
        }



        return false;
    }
    public static bool CheckFirstLogin(string username)
    {

        dsarEntities _db = new dsarEntities();

        var data = (from c in _db.ChangePasswordLogs select c).Where(c => c.UserName == username).FirstOrDefault();

        if (data == null)
        {
            return true;
        }
        return false;
    }
}


