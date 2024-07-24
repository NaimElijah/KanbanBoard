using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Authentication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using IntroSE.Kanban.Backend.BusinessLayer.AuthenticationModule;
using System.Globalization;
using IntroSE.Kanban.Backend.DataAccessLayer.DALControllers;
using IntroSE.Kanban.Backend.DataAccessLayer.DAOs;
using IntroSE.Kanban.Backend.ServiceLayer.Services;

namespace IntroSE.Kanban.Backend.BusinessLayer.UserModule
{
    internal class UserFacade
    {


        private Dictionary<string, UserBL> Users { get; set; }  // (User Email - UserBL)
        private Authentication Auth { get; set; }
        private UserController UController { get; set; }

        internal UserFacade(Authentication auth)
        {
            Auth = auth;
            Users = new Dictionary<string, UserBL>();
            UController = new UserController();
        }




        /// <summary>
        /// This method registers a new user to the system.
        /// </summary>
        /// <param name="email">The user email address, used as the username for logging the system.</param>
        /// <param name="password">The user password.</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        internal void Register(string email, string password)
        {
        
            // now checking the legitness of the input and handling it accordingly

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                Logger.GetLog().Error("A user that is trying to register entered a null or empty values in credentials places");
                throw new ArgumentNullException();
            }
            else if (Users.ContainsKey(email))
            {
                Logger.GetLog().Error("User " + email + " is trying to register even though he's already registered");
                throw new InvalidOperationException();
            }
            else if (!IsValidEmail(email))
            {
                Logger.GetLog().Error("Register: This method is being used with the invalid email: " + email);  // board name doesn't exist
                throw new InvalidFilterCriteriaException();
            }

            // now for the case that the check were positive and this is legit

            UserBL addition = new UserBL(email, password); // password legitness is checked here in the constructor with the set we implemented according to the password rules provided.
            // an InvalidCredentialException is thrown in the line above when the password given does not comply with the password rules.

            Users.Add(email, addition);
            Auth.AddUser(email);
            Logger.GetLog().Info("User " + email + " has successfully registered to the system");
                
        }





        /// <summary>
        /// A method that checks if an email is valid or not.
        /// </summary>
        /// <param name="email">The email address to validate.</param>
        /// <returns>true if valid, false otherwise.</returns>
        internal bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            try
            {
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));

                string DomainMapper(Match match)
                {
                    var idn = new IdnMapping();
                    string domainName = idn.GetAscii(match.Groups[2].Value);
                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }

            try
            {
                string pattern = @"^(?("")(""[^""]+?""@)|(([0-9a-zA-Z]((\.|\+)?[0-9a-zA-Z])*)@))((\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][0-9a-zA-Z-]*[0-9a-zA-Z]*\.)+[a-zA-Z]{2,24}))$";
                Regex regex = new Regex(pattern, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));

                return regex.IsMatch(email);
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }











        /// <summary>
        ///  This method logs in an existing user.
        /// </summary>
        /// <param name="email">The email address of the user to login</param>
        /// <param name="password">The password of the user to login</param>
        /// <returns>A response with the user's email, unless an error occurs (see <see cref="GradingService"/>)</returns>
        internal void Login(string email, string password)
        {

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                Logger.GetLog().Error("Login: Tried to log in but one or more of the credentials given is null or empty");
                throw new ArgumentNullException();
            }

            else if (!this.Users.ContainsKey(email))
            {
                Logger.GetLog().Error($"Login: {email} Tried to log in but email is unrecognized");
                throw new NotSupportedException();
            }

            bool _isLoggedIn = Auth.IsLoggedIn(email);
            if (_isLoggedIn)
            {
                Logger.GetLog().Error($"Login: {email} Tried to log in but user is already logged in");
                throw new InvalidOperationException();
            }

            if (!Users[email].IsCorrectPassword(password))
            {
                Logger.GetLog().Error($"Login: {email} Tried to log in but password is incorrect");
                throw new InvalidCredentialException();
            }
            else
            {
                Logger.GetLog().Info($"Login: {email} Succesfully logged in");
                Auth.ChangeLoggedStatus(email);
            }
            
            
        }









        /// <summary>
        /// This method logs out a logged in user. 
        /// </summary>
        /// <param name="email">The email of the user to log out</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        internal void Logout(string email)
        {
            
            if (string.IsNullOrEmpty(email))
            {
                Logger.GetLog().Error("User is trying to logout using null argument");
                throw new ArgumentNullException();
            }
            if (!this.Users.ContainsKey(email))  // is not registered
            {
                Logger.GetLog().Error("User is trying to logout using unregisterd email");
                throw new InvalidCredentialException();
            }
            bool _isLogedIn = Auth.IsLoggedIn(email);
            if (!_isLogedIn)
            {
                Logger.GetLog().Error("user " + email + " is already logged out and trying to log out again");
                throw new InvalidOperationException();
            }

            Auth.ChangeLoggedStatus(email);
            Logger.GetLog().Info(email + " logged out successfully");
            
        }









        //////////////////////////////////////////////////////////////////////////////////////////////////////////////    DAL RELATED FROM HERE










        ///<summary>This method loads all persisted data.
        ///<para>
        ///<b>IMPORTANT:</b> When starting the system via the GradingService - do not load the data automatically, only through this method. 
        ///In some cases we will call LoadData when the program starts and in other cases we will call DeleteData. Make sure you support both options.
        ///</para>
        /// </summary>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        internal void LoadData()  // for Users. Also initialize and put emails in the Authentication.
        {

            // use the user controller here to select all and get all of the UserDaos and add them to a list of new UserBLs and then add these to the
            //                                                                                                                          Dict here.
            List<object> temp = UController.SelectAll();
            List<UserDAO> userDaos = new List<UserDAO>();

            foreach (object item in temp)
            {
                userDaos.Add((UserDAO)item);
            }


            List<UserBL> userBls = new List<UserBL>();

            foreach (UserDAO userdao in userDaos)
            {
                userBls.Add(new UserBL(userdao));
            }

            foreach (UserBL userbl in userBls)
            {
                Users.Add(userbl.Email, userbl);
            }

            LoadInitializeAuthHelper();  // and after the data is loaded into user facade, we'll put all the emails here in the authentication

            // Loading done.
        }



        ///<summary>This method loads all data of emails in the system(after the user facade dict has been loaded) into the Authentication.</summary>
        /// <returns>nothing just loads the emails into Authentication</returns>
        internal void LoadInitializeAuthHelper()     //  this should be executed after the Dict in user facade has already been loaded.
        {
            foreach (string emailInSystem in Users.Keys)
            {
                Auth.AddUser(emailInSystem);
                Auth.ChangeLoggedStatus(emailInSystem);   // beacuse the add user function in auth already puts the logged in status as true when we add a user, but this is the startup and
            }   //                                                                                                              we want all of them to be logged out at the start.
        }












        ///<summary>This method deletes all persisted data.
        ///<para>
        ///<b>IMPORTANT:</b> 
        ///In some cases we will call LoadData when the program starts and in other cases we will call DeleteData. Make sure you support both options.
        ///</para>
        /// </summary>
        ///<returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        internal void DeleteData()  // for Users.
        {
            UController.DeleteAll();

            Users = new Dictionary<string, UserBL>();
            Auth.DeleteAllData();
        }






    }
}
