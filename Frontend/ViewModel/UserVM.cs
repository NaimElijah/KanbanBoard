﻿using Frontend.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.ViewModel
{
    public class UserVM : NotifiableObject
    {

        private BackendController controller;

        public BackendController Controller {  get { return controller; } }

        private string errorMessage = "";

        private ObservableCollection<BoardModel> userBoards;
        public  ObservableCollection<BoardModel> UserBoards { get => userBoards; set => userBoards = value; }
        
        public string ErrorMessage {
            get =>  errorMessage;
            set
            {  errorMessage = value;
                RaisePropertyChanged("ErrorMessage");
            }
        }

        public UserVM(UserModel user)
        {
            controller = user.Controller;
            UserBoards = user.Boards ?? new ObservableCollection<BoardModel>();
        }

        public UserVM(BoardModel board)
        {
            controller = board.Controller;
            UserBoards = board.Controller.GetUserBoards(board.UserModelEmail) ?? new ObservableCollection<BoardModel>();
        }

        internal BoardModel GetBoard(UserModel user, string boardName)
        {

            return user.GetBoard(boardName);
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
