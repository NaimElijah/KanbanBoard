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
            Response response = JsonSerializer.Deserialize<Response>(Service.LoadData());
            if (response != null)
                throw new Exception(response.ErrorMessage);
        }

        public Tuple<UserModel?,string> Login(string email, string password)
        { }

    }
}
