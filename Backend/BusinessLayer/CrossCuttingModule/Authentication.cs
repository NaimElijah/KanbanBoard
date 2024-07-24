using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.BusinessLayer.AuthenticationModule
{
    internal class Authentication
    {
        private Dictionary<string, bool> AllLoginStatuses { get; set; }

        internal Authentication()
        {
            AllLoginStatuses = new Dictionary<string, bool>();
        }

        
        
        internal bool IsLoggedIn(string email)
        {
            if (AllLoginStatuses.ContainsKey(email))
            {
                return AllLoginStatuses[email];
            }
            return false;  // not even registered
        }


        
        
        internal void ChangeLoggedStatus(string email)   // legitness of using this method is already checked before calling this method and responded accordingly
        {
            if (AllLoginStatuses.ContainsKey(email))
            {
                AllLoginStatuses[email] = !AllLoginStatuses[email];
            }
        }




        internal void AddUser(string email)    ///  used by the UserFacade upon registration, legitness of using this method is already checked before calling this method and responded accordingly
        {
            AllLoginStatuses.Add(email, true);
        }



        internal void DeleteAllData()
        {
            AllLoginStatuses = new Dictionary<string, bool>();
        }


    }
}
