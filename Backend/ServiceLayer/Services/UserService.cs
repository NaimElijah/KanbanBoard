using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.BusinessLayer.AuthenticationModule;
using IntroSE.Kanban.Backend.BusinessLayer.UserModule;

namespace IntroSE.Kanban.Backend.ServiceLayer.Services
{
    public class UserService
    {

        private UserFacade Uf { get; set; }

        internal UserService(UserFacade uf)
        {
            Uf = uf;
        }




        /// <summary>
        /// This method registers a new user to the system.
        /// </summary>
        /// <param name="email">The user email address, used as the username for logging into the system.</param>
        /// <param name="password">The user password.</param>
        /// <exception> in this method an InvalidCredentialException can be thrown when the password doesn't comply with the password rules or when an existing email is given </exception>
        /// <exception> in this method an InvalidOperationException can be thrown when the user isn't logged in </exception>
        /// <pre-condition> the email must not be already registered and the password must comply with the password rules</pre-condition>
        /// <post-condition> the user will be registered and logged in </post-condition>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string Register(string email, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    Logger.GetLog().Error("Register: Tried but one or more of the credentials given is null or empty");
                    throw new ArgumentNullException();
                }
                Uf.Register(email.ToLower(), password);
                return new Response(null, null).GetSerializedVersion();
            }
            catch (ArgumentNullException e)  // for null problems
            {
                return new Response("One or more of the credentials given to register are null or empty", null).GetSerializedVersion();
            }
            catch (InvalidOperationException e)  // for flow problems
            {
                return new Response("User is trying to register even though he's already registered in our system", null).GetSerializedVersion();
            }
            catch (InvalidCredentialException e) // for invalid password, would be thrown in the UserBL's set, when we check there
            {
                return new Response("The password provided during registration does not comply with the password rules", null).GetSerializedVersion();
            }
            catch (InvalidFilterCriteriaException e) // for invalid email
            {
                return new Response("The email provided during registration isn't valid", null).GetSerializedVersion();
            }
            catch (DataMisalignedException e) // for failed sql execution in database
            {
                return new Response("SQL Execution in database failed", null).GetSerializedVersion();
            }

        }



        /// <summary>
        ///  This method logs in an existing user.
        /// </summary>
        /// <param name="email">The email address of the user to login</param>
        /// <param name="password">The password of the user to login</param>
        /// <exception> in this method an InvalidCredentialException can be thrown when the password isn't correct or when the email given is not registered </exception>
        /// <exception> in this method an InvalidOperationException can be thrown when the user isn't logged in </exception>
        /// <pre-condition> the user will be logged out </pre-condition>
        /// <post-condition> the user will be logged in </post-condition>
        /// <returns>A response with the user's email, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string Login(string email, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    Logger.GetLog().Error("Login: Tried but one or more of the credentials given is null or empty");
                    throw new ArgumentNullException();
                }
                Uf.Login(email.ToLower(), password);
                return new Response(null, email).GetSerializedVersion();
            }
            catch (InvalidOperationException e)
            {
                return new Response("User is already logged in", null).GetSerializedVersion();
            }
            catch (InvalidCredentialException g)
            {
                return new Response("Incorrect Credentials", null).GetSerializedVersion();
            }
            catch (ArgumentNullException g)
            {
                return new Response("One or more of the credentials given is null or empty", null).GetSerializedVersion();
            }
            catch (NotSupportedException g)
            {
                return new Response("email is unrecognized", null).GetSerializedVersion();
            }
            catch (DataMisalignedException e) // for failed sql execution in database
            {
                return new Response("SQL Execution in database failed", null).GetSerializedVersion();
            }

        }



        /// <summary>
        /// This method logs out a logged in user. 
        /// </summary>
        /// <param name="email">The email of the user to log out</param>
        /// <exception> in this method an InvalidCredentialException can be thrown when the email given is not registered </exception>
        /// <exception> in this method an InvalidOperationException can be thrown when the user isn't logged in </exception>
        /// <pre-condition> the user will be logged in and the email given is already registered </pre-condition>
        /// <post-condition> the user will be logged out </post-condition>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string Logout(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    Logger.GetLog().Error("Logout: Tried but one or more of the credentials given is null or empty");
                    throw new ArgumentNullException();
                }
                Uf.Logout(email.ToLower());
                return new Response(null, null).GetSerializedVersion();
            }
            catch (InvalidOperationException e)
            {
                return new Response("user isn't logged in to system", null).GetSerializedVersion();
            }
            catch (InvalidCredentialException g)
            {
                return new Response("email doesn't exist in the system", null).GetSerializedVersion();
            }
            catch (ArgumentNullException g)
            {
                return new Response("given null argument", null).GetSerializedVersion();
            }
            catch (DataMisalignedException e) // for failed sql execution in database
            {
                return new Response("SQL Execution in database failed", null).GetSerializedVersion();
            }

        }







        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////  DAL RELATED FROM HERE









        ///<summary>This method loads all persisted data.
        ///<para>
        ///<b>IMPORTANT:</b> When starting the system via the GradingService - do not load the data automatically, only through this method. 
        ///In some cases we will call LoadData when the program starts and in other cases we will call DeleteData. Make sure you support both options.
        ///</para>
        /// </summary>
        /// <exception> in this method an Exception can be thrown when a connected database doesn't exist or not found</exception>
        /// <pre-condition> A connected database must exist </pre-condition>
        /// <post-condition> No change in the state - All the data from the database is loaded into the system </post-condition>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string LoadData()  // for Users
        {
            try
            {
                Uf.LoadData();
                return new Response(null, null).GetSerializedVersion();
            }
            catch (DataMisalignedException e) // for failed sql execution in database
            {
                Logger.GetLog().Error("Users LoadData Failed");
                return new Response("SQL Execution in database failed", null).GetSerializedVersion();
            }
            catch (Exception e)
            {
                Logger.GetLog().Error("Users LoadData Failed");
                return new Response("Users LoadData Failed", null).GetSerializedVersion();
            }
            
        }











        ///<summary>This method deletes all persisted data.
        ///<para>
        ///<b>IMPORTANT:</b> 
        ///In some cases we will call LoadData when the program starts and in other cases we will call DeleteData. Make sure you support both options.
        ///</para>
        /// </summary>
        /// <exception> in this method an Exception can be thrown when a connected database doesn't exist or isn't found</exception>
        /// <pre-condition> A connected database must exist </pre-condition>
        /// <post-condition> No change in the state - All the data in the database is deleted </post-condition>
        ///<returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string DeleteData()
        {
            try
            {
                Uf.DeleteData();
                return new Response(null, null).GetSerializedVersion();
            }
            catch (DataMisalignedException e) // for failed sql execution in database
            {
                Logger.GetLog().Error("Users DeleteData Failed");
                return new Response("SQL Execution in database failed", null).GetSerializedVersion();
            }
            catch (Exception e)
            {
                Logger.GetLog().Error("Users DeleteData Failed");
                return new Response("Users DeleteData Failed", null).GetSerializedVersion();
            }

        }







    }
}
