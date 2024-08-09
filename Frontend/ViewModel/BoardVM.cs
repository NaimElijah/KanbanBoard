using Frontend.Model;
using IntroSE.Kanban.Backend.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Frontend.ViewModel
{
    public class BoardVM: NotifiableObject
    {
        internal BackendController controller;

        private ObservableCollection<TaskModel> backlog;

        public  ObservableCollection<TaskModel> Backlog
        {
            get => backlog;
            set { backlog = value; }
        }

        private ObservableCollection<TaskModel> inProgress;

        public ObservableCollection<TaskModel> InProgress
        {
            get => inProgress;
            set { inProgress = value; }
        }

        private ObservableCollection<TaskModel> done;

        public ObservableCollection<TaskModel> Done
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