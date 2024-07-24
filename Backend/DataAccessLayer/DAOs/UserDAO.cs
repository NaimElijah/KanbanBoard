using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.DataAccessLayer.DALControllers;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DAOs
{
    internal class UserDAO
    {
        internal string Email { get; set; }
        private const string UsersEmailCol = "Email";
        private const string UsersPasswordCol = "Password";

        private string password;
        internal string Password
        {
            get => password;
            set
            {
                if (IsPersisted)
                {
                    UController.UpdateUser(Email, UsersPasswordCol, value);
                }
                password = value;
            }
        }

        internal bool IsPersisted { get; set; }
        internal UserController UController { get; set; }

        internal UserDAO(string email, string password)
        {
            Email = email;
            Password = password;
            UController = new UserController();
            IsPersisted = false;    // later when saved to database, this turns to true
        }


        /// <summary>
        /// inserting the current user to the database.
        /// </summary>
        internal void Persist()
        {
            if (!IsPersisted)
            {
                UController.Insert(this);
                IsPersisted = true;
            }
        }


    }
}
