using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.Exceptions
{
    internal class NotTaskAssigneeException: Exception { 
        public NotTaskAssigneeException() : base("The user is not an assignee of this task")
        {
        }
    }
}
