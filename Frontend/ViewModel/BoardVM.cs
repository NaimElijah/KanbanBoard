using Frontend.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.ViewModel
{
    public class BoardVM
    {
        internal BackendController controller;

        private List<string> backlog;

        public List<string> Backlog
        {
            get => backlog;
            set { backlog = value; }
        }

        private List<string> inProgress;

        public List<string> InProgress
        {
            get => inProgress;
            set { inProgress = value; }
        }

        private List<string> done;

        public List<string> Done
        {
            get => done;
            set { done = value; }
        }

        public BoardVM(BoardModel board)
        {
            controller = board.Controller;
            backlog = board.BacklogTasks;
            inProgress = board.InProgressTasks;
            done = board.DoneTasks;
        }
    }
}