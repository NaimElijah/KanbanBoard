using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.BusinessLayer.UserModule;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    internal class UserSL
    {
        public string Email { get; set; }

        internal UserSL(UserBL usBL)
        {
            Email = usBL.Email;
        }



    }
}
