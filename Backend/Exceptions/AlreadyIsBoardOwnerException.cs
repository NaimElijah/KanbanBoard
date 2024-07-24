using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.Exceptions
{
    internal class AlreadyIsBoardOwnerException : Exception
    {
        public AlreadyIsBoardOwnerException(): base("The user is already the owner of this board.") {
    }
}
}
