using Frontend.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.ViewModel
{
    internal class UserVM : NotifiableObject
    {
        private BackendController controller;
        private string errorMessage = "";
        
        public string ErrorMessage {
            get =>  errorMessage;
            set
            {  errorMessage = value;
                RaisePropertyChanged("ErrorMessage");
            }
        }

        public UserVM()
        {  controller = new BackendController(); }

        public UserVM(UserModel user)
        {
            controller = user.Controller;
        }

        public UserVM(BackendController controller)
        {
            this.controller = controller;
        }

        internal BoardModel GetBoard(UserModel user ,string boardName)
        {
            return controller.GetBoard(user, boardName);
        }

        internal void LogoutUser(string email)
        {
            try
            {
                controller.Logout(email);
                

            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                
            }
        }


    }
}
