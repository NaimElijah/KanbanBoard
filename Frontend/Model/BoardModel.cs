﻿using Frontend.Model;
using IntroSE.Kanban.Backend.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Model
{
    public class BoardModel : NotifiableModelObject
    {
        private string boardName;
        public string BoardName
        {
            get => boardName;
            set
            {
                boardName = value;
                RaisePropertyChanged("Name");
            }
        }
        private string owner; 
        public string Owner
        {
            get => owner;
            set
            {
                owner = value;
                RaisePropertyChanged("Owner");
            }
        }

        private List<string> members;
        public List<string> Members
        {
            get => members;
            set
            {
                members = value;
                RaisePropertyChanged("members");
            }
        }

        private ObservableCollection<TaskModel> backlogTasks;
        public ObservableCollection<TaskModel> BacklogTasks
        {
            get => backlogTasks;
            set
            {
                if (backlogTasks != value)
                {
                    backlogTasks = value;
                    RaisePropertyChanged("BacklogTasks");
                }
            }
        }

        private ObservableCollection<TaskModel> inProgressTasks;
        public ObservableCollection<TaskModel> InProgressTasks
        {
            get => inProgressTasks;
            set
            {
                if (inProgressTasks != value)
                {
                    inProgressTasks = value;
                    RaisePropertyChanged("InProgressTasks");
                }
            }
        }

        private ObservableCollection<TaskModel> doneTasks;
        public ObservableCollection<TaskModel> DoneTasks
        {
            get => doneTasks;
            set
            {
                if (doneTasks != value)
                {
                    doneTasks = value;
                    RaisePropertyChanged("DoneTasks");
                }
            }
        }

         

           private string userModelEmail;
           public string UserModelEmail
           {
               get => userModelEmail;
           }

/*        public BoardModel(BackendController controller, UserModel user, string name, string ownnerNmae, List<string> boardMembers) : base(controller)
        {
            BoardName = name;
            this.userModelEmail = user;
            Owner = ownnerNmae;
            Members = boardMembers;
            backlogTasks = new ObservableCollection<TaskModel>();
            inProgressTasks = new ObservableCollection<TaskModel>();
            doneTasks = new ObservableCollection<TaskModel>();
        }*/

        public BoardModel(BackendController controller, string userEmail, string name, string ownnerNmae, List<string> boardMembers) : base(controller)
        {
            BoardName = name;
            this.userModelEmail = userEmail;
            Owner = ownnerNmae;
            Members = boardMembers;
            backlogTasks = new ObservableCollection<TaskModel>();
            inProgressTasks = new ObservableCollection<TaskModel>();
            doneTasks = new ObservableCollection<TaskModel>();
        }

   
    }
}