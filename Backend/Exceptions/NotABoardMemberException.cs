using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.Exceptions
{
    internal class NotABoardMemberException : Exception
    {
        public NotABoardMemberException() : base("The user is not a board member") {
        }
    }
}
