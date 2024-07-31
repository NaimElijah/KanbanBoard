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
            if (response != null)
                throw new Exception(response.ErrorMessage);
        }

        public Tuple<UserModel?,string> Login(string email, string password)
        {  
            Response response = JsonSerializer.Deserialize<Response>(Service.Login(email, password));
            if (response.ErrorMessage!= null)
                return Tuple.Create<UserModel?,string>(null, response.ErrorMessage);
            return Tuple.Create<UserModel?, string>(new UserModel(this, email, GetUserBoards(email)), response.ErrorMessage);

        
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

        internal BoardModel GetBoard(UserModel user, string boardName)
        {
            throw new NotImplementedException();
        }
    }
}
