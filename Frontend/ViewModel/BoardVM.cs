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

        private string boardName;
        public string BoardName { get => boardName; set => boardName = value; }

        private string owner;
        public string Owner
        {
            get => owner;
            set
            {
                if (owner != value)
                {
                    owner = value;
                    RaisePropertyChanged(nameof(Owner));
                }
            }
        }

        private ObservableCollection<string> members;

        public ObservableCollection<string> Members
        {
            get => members;
            set
            {
                if (members != value)
                {
                    members = value;
                    RaisePropertyChanged(nameof(Members));

                }
            }
        }
        public BoardVM(BoardModel board)
        {
            controller = board.Controller;
            Backlog = board.BacklogTasks;
            InProgress = board.InProgressTasks;
            Done = board.DoneTasks;
            BoardName = board.BoardName;
            Owner = board.Owner;
            Members = board.Members;
        }
    }

}