using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Security.Cryptography;

namespace cms.Models
{
    public class UserRepository
    {
        public MembershipUser CreateUser(string username, string password, string email)
        {
            using (dsarEntities db = new dsarEntities())
            {
                User user = new User();

                user.UserName = username;                
                user.PasswordSalt = CreateSalt();
                user.Password = CreatePasswordHash(password, user.PasswordSalt);
                user.CreatedDate = DateTime.Now;
                user.IsLockedOut = false;
                user.LastLoginDate = DateTime.Now;

                db.AddToUsers(user);
                db.SaveChanges();


                return GetUser(username);
            }
        }


        public MembershipUser GetUser(string username)
        {
            using (dsarEntities db = new dsarEntities())
            {
                var result = from u in db.Users where (u.UserName == username) select u;

                if (result.Count() != 0)
                {
                    var dbuser = result.FirstOrDefault();

                    string _username = dbuser.UserName;
                    int _providerUserKey = dbuser.UserId;
                    string _email = "";
                    string _passwordQuestion = "";
                    string _comment = "";
                    bool _isApproved = true;
                    bool _isLockedOut = dbuser.IsLockedOut;
                    DateTime _creationDate = dbuser.CreatedDate;
                    DateTime _lastLoginDate = dbuser.LastLoginDate;
                    DateTime _lastActivityDate = DateTime.Now;
                    DateTime _lastPasswordChangedDate = DateTime.Now;
                    DateTime _lastLockedOutDate = DateTime.Now;

                    MembershipUser user = new MembershipUser("CustomMembershipProvider",
                                                              _username,
                                                              _providerUserKey,
                                                              _email,
                                                              _passwordQuestion,
                                                              _comment,
                                                              _isApproved,
                                                              _isLockedOut,
                                                              _creationDate,
                                                              _lastLoginDate,
                                                              _lastActivityDate,
                                                              _lastPasswordChangedDate,
                                                              _lastLockedOutDate);

                    return user;
                }
                else
                {
                    return null;
                }
            }
        }

        public static string CreateSalt()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[32];
            rng.GetBytes(buff);

            return Convert.ToBase64String(buff);
        }

        public static string CreatePasswordHash(string pwd, string salt)
        {
            string saltAndPwd = String.Concat(pwd, salt);
            string hashedPwd =
                    FormsAuthentication.HashPasswordForStoringInConfigFile(
                    saltAndPwd, "sha1");
            return hashedPwd;
        }

        public bool ValidateUser(string username, string password)
        {
            using (dsarEntities db = new dsarEntities())
            {
                var result = from u in db.Users where (u.UserName == username) && (u.IsLockedOut == false) select u;

                if (result.Count() != 0)
                {
                    var dbuser = result.First();

                    if (dbuser.Password == CreatePasswordHash(password, dbuser.PasswordSalt))
                        return true;
                    else
                        return false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            int minPwdLength = Membership.MinRequiredPasswordLength;

            if (newPassword.Length < minPwdLength) return false;

            using (dsarEntities db = new dsarEntities())
            {
                var user = (from u in db.Users where u.UserName == username select u).First();

                string oldPwdSalt = CreatePasswordHash(oldPassword, user.PasswordSalt);

                if (user.Password != oldPwdSalt) return false;

                user.PasswordText = newPassword;
                user.PasswordSalt = CreateSalt();
                user.Password = CreatePasswordHash(newPassword, user.PasswordSalt);

                db.ApplyCurrentValues(user.EntityKey.EntitySetName, user);
                db.SaveChanges();

            }

            return true;
        }

    }
}