﻿using Frontend.ViewModel;
using IntroSE.Kanban.Backend.ServiceLayer;
using System;
using System.Collections.Generic;
<<<<<<< HEAD
using System.IO;
=======
using System.Collections.ObjectModel;
>>>>>>> Add_Create_And_Delte_Board
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Frontend.Model
{
    public class BackendController
    {

        private GradingService Service {get; set;}

        public BackendController(GradingService service)
        {
            Service = service;
        }

        public BackendController()
        {
            Service = new GradingService();
            Response? response = JsonSerializer.Deserialize<Response>(Service.LoadData());
            if (response.ErrorMessage != null)
                throw new Exception(response.ErrorMessage);
        }

       /*public Tuple<UserModel?,string> Login(string email, string password)
        {  
            Response response = JsonSerializer.Deserialize<Response>(Service.Login(email, password));
            if (response.ErrorMessage!= null)
                return Tuple.Create<UserModel?,string>(null, response.ErrorMessage);
            return Tuple.Create<UserModel?, string>(new UserModel(this, email, GetUserBoards(email)), response.ErrorMessage);

        
        }*/
        public UserModel Login(string email, string password)
        {
            Response? response = JsonSerializer.Deserialize<Response>(Service.Login(email, password));
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
             return new UserModel(this,email, GetUserBoardsInfo(email));

        }

        /*public Tuple<UserModel?, string> Register(string email, string password) {
         
            Response response = JsonSerializer.Deserialize<Response>(Service.Register(email, password));
            if(response.ErrorMessage != null)
                return Tuple.Create<UserModel?,string>(null, response.ErrorMessage);
            return Tuple.Create<UserModel?,string>(new UserModel(this,email,new List<string>()),response.ErrorMessage);

         }*/

        public UserModel Register(string email, string password)
        {

            Response? response = JsonSerializer.Deserialize<Response>(Service.Register(email, password));
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
            return new UserModel(this, email, GetUserBoardsInfo(email));
        }

        /* public Tuple<UserModel?, string> Logout(string email, string password)
         {
             Response response = JsonSerializer.Deserialize<Response>(Service.Logout(email));
             if(response.ErrorMessage != null)
                 return Tuple.Create<UserModel?, string>(null, response.ErrorMessage);
             return Tuple.Create<UserModel?, string>(new UserModel(this, email, new List<string>()), response.ErrorMessage);
         }*/

        public void Logout(string email)
        {
            Response? response = JsonSerializer.Deserialize<Response>(Service.Logout(email));
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
        }

        public ObservableCollection<BoardModel> GetUserBoards(string usserEmail)
        {
            Response response = JsonSerializer.Deserialize<Response>(Service.GetUserBoards(usserEmail));
            List<int> userBoardsId = JsonSerializer.Deserialize<List<int>>((JsonElement)response.ReturnValue);
            ObservableCollection<BoardModel> boards = new ObservableCollection<BoardModel>();

            foreach (int i in userBoardsId)
            {
                string name = JsonSerializer.Deserialize<Response>(Service.GetBoardName(i)).ReturnValue.ToString();

                string owner = JsonSerializer.Deserialize<Response>(Service.GetBoardOwner(i)).ReturnValue.ToString();

                Response r0 = JsonSerializer.Deserialize<Response>(Service.GetBoardMembers(i));

                List<string> members = JsonSerializer.Deserialize<List<string>>((JsonElement)r0.ReturnValue);

                BoardModel boardModel = GetBoard(usserEmail, name, owner, members);
            }

            return boards;
        }

        public List<string> GetUserBoardsInfo(string email)
        {
            Response response = JsonSerializer.Deserialize<Response>(Service.GetUserBoardsFull(email));
            List<BoardSL> boardSLs = JsonSerializer.Deserialize<List<BoardSL>>((JsonElement)response.ReturnValue);
            List<string> resBoards = new List<string>();
            foreach (var board in boardSLs)
            {
                resBoards.Add($"{board.BoardId}:{board.BoardName}:{board.BoardOwnerEmail}");
            }
            return resBoards;

        }


        public BoardModel GetBoard(string userEmail, string boardName ,string emailOwner , List<string> members)
        {
            Response res0 = JsonSerializer.Deserialize<Response>(Service.GetColumn(userEmail, boardName, 0));
            Response res1 = JsonSerializer.Deserialize<Response>(Service.GetColumn(userEmail, boardName, 1));
            Response res2 = JsonSerializer.Deserialize<Response>(Service.GetColumn(userEmail, boardName, 2));
            

            BoardModel board = new BoardModel(this, userEmail, boardName ,emailOwner , members);
            List<TaskSL> t0 = JsonSerializer.Deserialize<List<TaskSL>>((JsonElement)res0.ReturnValue);
            List<TaskModel> n0 = new List<TaskModel>(), n1 = new List<TaskModel>(), n2 = new List<TaskModel>();
            List<TaskSL> t1 = JsonSerializer.Deserialize<List<TaskSL>>((JsonElement)res1.ReturnValue);
            List<TaskSL> t2 = JsonSerializer.Deserialize<List<TaskSL>>((JsonElement)res2.ReturnValue);

            foreach (var task in t0)
            {
                board.BacklogTasks.Add(new TaskModel(task));
            }

            foreach (var task in t1)
            {
                board.InProgressTasks.Add(new TaskModel(task));
            }
            foreach (var task in t2)
            {
                board.DoneTasks.Add(new TaskModel(task));
            }
            

            return board;
        }

        internal void CreateBoard(BackendController controller, UserModel user, string boardName)
        {
            BoardModel board = new BoardModel(this, user, boardName);
        }
    }
}

