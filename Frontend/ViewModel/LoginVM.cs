using System;
using System.Collections.Generic;
using System.Windows;
using Frontend;
using Frontend.Model;

namespace Frontend.ViewModel
{
    internal class LoginVM : NotifiableObject
    {
        private BackendController controller;

        public LoginVM(BackendController controller)
        {
            this.controller = controller;
        }

        public LoginVM()
        {
            controller = new BackendController();
        }

        private string errorMessage = "";

        public string ErrorMessage
        {
            get => errorMessage;
            set
            {
                errorMessage = value;
                RaisePropertyChanged("ErrorMessage");
            }
        }

        private string email = "";

        public string Email
        {
            get => email;
            set
            {
                email = value;
                FieldsAreNotEmpty = string.IsNullOrWhiteSpace(value);
            }
        }

        private string password = "";

        public string Password
        {
            get => password;
            set
            {
                password = value;
                FieldsAreNotEmpty = string.IsNullOrWhiteSpace(value);
            }
        }

        internal bool FieldsAreNotEmpty
        {
            get => !(string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(password));
            set => RaisePropertyChanged("FieldsAreNotEmpty");
        }

        /*internal UserModel? Login()
        {
            Tuple<UserModel?, string> t = controller.Login(email, password);
            ErrorMessage = t.Item2;
            return t.Item1;
        }*/

        internal UserModel? Login()
        {
            try
            {
                return controller.Login(Email, Password);            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return null;
            }
        }

       /* internal UserModel? Register()
        {
            Tuple<UserModel?, string> t = controller.Register(email, password);
            ErrorMessage = t.Item2;
            return t.Item1;
        }*/

        internal UserModel? Register()
        {
            try
            {
                return controller.Register(Email, Password);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return null;
            }
        }
        internal List<string> GetUserBoards()
        {
            return controller.GetUserBoards(Email);
        }
    }
}