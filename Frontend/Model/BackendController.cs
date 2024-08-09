using IntroSE.Kanban.Backend.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
             return new UserModel(this,email,GetUserBoards(email));

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
            return new UserModel(this, email, GetUserBoards(email));
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

        public List<string> GetUserBoards(string email)
        {
            Response response = JsonSerializer.Deserialize<Response>(Service.GetUserBoards(email));
            List<int> userBoardsId = JsonSerializer.Deserialize<List<int>>((JsonElement)response.ReturnValue);
            List<string> boardNames = new List<string>();
            foreach(int i in userBoardsId)
            {
                boardNames.Add(JsonSerializer.Deserialize<Response>(Service.GetBoardName(i)).ReturnValue.ToString());
            }
            return boardNames;


        }

        public BoardModel GetBoard(UserModel user, string boardName)
        {
            Response res0 = JsonSerializer.Deserialize<Response>(Service.GetColumn(user.Email, boardName, 0));
            Response res1 = JsonSerializer.Deserialize<Response>(Service.GetColumn(user.Email, boardName, 1));
            Response res2 = JsonSerializer.Deserialize<Response>(Service.GetColumn(user.Email, boardName, 2));

            BoardModel board = new BoardModel(this, user, boardName);

            List<TaskSL> t0 = JsonSerializer.Deserialize<List<TaskSL>>((JsonElement)res0.ReturnValue);
            List<string> n0 = new List<string>(), n1 = new List<string>(), n2 = new List<string>();
            n0.AddRange(t0.Select(t => t.Title));
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

    }
}

