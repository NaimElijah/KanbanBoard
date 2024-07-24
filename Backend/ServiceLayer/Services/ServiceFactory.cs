using IntroSE.Kanban.Backend.BusinessLayer.BoardModule;
using IntroSE.Kanban.Backend.BusinessLayer.UserModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.BusinessLayer.AuthenticationModule;

namespace IntroSE.Kanban.Backend.ServiceLayer.Services
{
    public class ServiceFactory
    {
        public UserService Us { get; set; }
        public BoardService Bs { get; set; }
        public TaskService Ts { get; set; }
        public ColumnService Cs { get; set; }
        private bool IsLoaded { get; set; }

        public ServiceFactory()
        {
            Authentication auth = new Authentication();

            UserFacade Uf = new UserFacade(auth);
            Us = new UserService(Uf);

            BoardFacade Bf = new BoardFacade(auth);
            Bs = new BoardService(Bf);
            Ts = new TaskService(Bf);
            Cs = new ColumnService(Bf);
            IsLoaded = false;

        }

        /// <summary>
        /// this method activates the LoadData methods of the users and the boards
        /// </summary>
        /// <exception> in this method an Exception can be thrown when a connected database doesn't exist or isn't found</exception>
        /// <pre-condition> A connected database must exist </pre-condition>
        /// <post-condition> No change in the state - All the data from the database is loaded into the system </post-condition>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        internal string LoadAllData()
        {
            string NullRes = new Response(null, null).GetSerializedVersion();
            if (IsLoaded)
            {
                return NullRes;
            }
            string UsersLoadRes = Us.LoadData();
            string BoardsLoadRes = Bs.LoadData();          //  we need to check what both of these return and return accordingly
            if (UsersLoadRes.Equals(NullRes) && BoardsLoadRes.Equals(NullRes))
            {
                IsLoaded = true;
                Logger.GetLog().Info("LoadData Successful");
                return NullRes;
            }
            Logger.GetLog().Error("LoadData Failed");
            return new Response("Load Data Failed", null).GetSerializedVersion();
        }


        /// <summary>
        /// this method activates the DeleteData methods of the users and the boards
        /// </summary>
        /// <exception> in this method an Exception can be thrown when a connected database doesn't exist or isn't found</exception>
        /// <pre-condition> A connected database must exist </pre-condition>
        /// <post-condition> No change in the state - All the data in the database is deleted </post-condition>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        internal string DeleteAllData()
        {
            string BoardsDeleteRes = Bs.DeleteData();
            string UsersDeleteRes = Us.DeleteData();           //  we need to check what both of these return and return accordingly
            string NullRes = new Response(null, null).GetSerializedVersion();
            if (UsersDeleteRes.Equals(NullRes) && BoardsDeleteRes.Equals(NullRes))
            {
                Logger.GetLog().Info("DeleteData Successful");
                return NullRes;
            }
            Logger.GetLog().Error("DeleteData Failed");
            return new Response("Delete Data Failed", null).GetSerializedVersion();
        }






    }
}
