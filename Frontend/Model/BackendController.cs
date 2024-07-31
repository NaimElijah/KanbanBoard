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

        public Tuple<UserModel?,string> Login(string email, string password)
        {  
            Response response = JsonSerializer.Deserialize<Response>(Service.Login(email, password));
            if (response.ErrorMessage!= null)
                return Tuple.Create<UserModel?,string>(null, response.ErrorMessage);
            return Tuple.Create<UserModel?, string>(new UserModel(this, email, GetUserBoards(email)), response.ErrorMessage);

        
        }

        public Tuple<UserModel?, string> Register(string email, string password) {
         
            Response response = JsonSerializer.Deserialize<Response>(Service.Register(email, password));
            if(response.ErrorMessage != null)
                return Tuple.Create<UserModel?,string>(null, response.ErrorMessage);
            return Tuple.Create<UserModel?,string>(new UserModel(this,email,new List<string>()),response.ErrorMessage);

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
            Response? res0 = JsonSerializer.Deserialize<Response>(Service.GetColumn(user.Email, boardName, 0));
            Response? res1 = JsonSerializer.Deserialize<Response>(Service.GetColumn(user.Email, boardName, 1));
            Response? res2 = JsonSerializer.Deserialize<Response>(Service.GetColumn(user.Email, boardName, 2));

            Console.WriteLine(res0.ToString());
            
            BoardModel board = new BoardModel(this, user, boardName);

            List<TaskSL> t0 = JsonSerializer.Deserialize<List<TaskSL>>((JsonElement)res0.ReturnValue);
            List<string> n0 = new List<string>(), n1 = new List<string>(), n2 = new List<string>();
            n0.AddRange(t0.Select(t => t.Title));
            List<TaskSL> t1 = JsonSerializer.Deserialize<List<TaskSL>>((JsonElement)res1.ReturnValue);
            n1.AddRange(t1.Select(t => t.Title));
            List<TaskSL> t2 = JsonSerializer.Deserialize<List<TaskSL>>((JsonElement)res2.ReturnValue);
            n2.AddRange(t2.Select(t => t.Title));
            board.BacklogTasks = n0;
            board.InProgressTasks = n1;
            board.DoneTasks = n2;
            return board;
        }
    }
}
