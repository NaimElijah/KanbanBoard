using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.DataAccessLayer.DALControllers;
using IntroSE.Kanban.Backend.DataAccessLayer.DAOs;

namespace IntroSE.Kanban.Backend.BusinessLayer.UserModule
{
    internal class UserBL
    {
        internal UserDAO UserDao { get; set; }

        private string password;
        internal string Password
        {
            private get { return password; }
            set
            {
                if (CheckPasswordLegitness(value))
                {
                    password = value;
                    UserDao.Password = password;
                }
                else
                {
                    Logger.GetLog().Error("A user has tried to register but entered a password that does not comply to the password rules");
                    if (!UserDao.IsPersisted)  // if we get an invalid password in the initialization
                    {
                        UserDao = null;  // turn it back to null, might not be needed because the constructor hasn't finished so this might be already garbage
                    }
                    throw new InvalidCredentialException();  // will be caught in an upper level.(code that came before this bulk of code, in the workflow of things)
                }
            }
        }
        internal string Email { get; set; }

        internal UserBL(string email, string passw)
        {
            UserDao = new UserDAO(email, passw);
            Email = email;
            Password = passw;
            UserDao.Persist();  // inserting to Users table in database
        }


        internal UserBL(UserDAO userdao)  //  for LoadData of users
        {
            UserDao = userdao;
            Email = userdao.Email;
            Password = userdao.Password;
            UserDao.IsPersisted = true;
        }




        /// <summary>
        /// This method checks if a password is legit according to the password rules.
        /// </summary>
        /// <param name="password">the password we are checking it's legitness</param>
        /// <returns>true if a password in legit, false otherwise</returns>
        internal bool CheckPasswordLegitness(string passw)
        {
            if ( (passw.Length >= 6 && passw.Length <= 20) && (passw.Any(char.IsLower) && passw.Any(char.IsUpper)) && (passw.Any(char.IsDigit)) )
            {
                return true;
            }
            return false;

        }

        internal bool IsCorrectPassword(string passwordd)
        {
            return Password == passwordd;
        }



        


    }
}
