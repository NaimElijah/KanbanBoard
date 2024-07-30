using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using IntroSE.Kanban.Backend.DataAccessLayer.DAOs;
using IntroSE.Kanban.Backend.ServiceLayer;

namespace IntroSE.Kanban.Backend.BusinessLayer.BoardModule
{
    internal class TaskBL
    {
        internal TaskDAO TaskDao { get; set; }
        internal long Id { get; set; }


        private string assigneeEmail;
        internal string AssigneeEmail
        {
            get { return assigneeEmail; }
            set
            {
                TaskDao._AssigneeEmail = assigneeEmail;  // and this goes to the DAO's set
                assigneeEmail = value;
            }
        }



        internal DateTime CreationTime { get; set; }


        private string title;
        internal string Title
        {
            get { return title;}
            set
            {
                TaskDao._Title = title;  // and this goes to the DAO's set
                title = value;
            }
        }


        private string description;
        internal string Description
        {
            get { return description;}
            set
            {
                TaskDao._Description = description;  // and this goes to the DAO's set
                description = value;
            }
        }



        private DateTime dueDate;
        internal DateTime DueDate
        {
            get { return dueDate;}
            set
            {
                TaskDao._DueDate = dueDate;  // and this goes to the DAO's set
                dueDate = value;
            }
        }







        internal TaskBL(long id, string title, string description, DateTime dueDate, string assigneeEmaill="unassigned") // default AssigneeEmail to "unassigned"  <<--------  according to instructions
        {
            DateTime DateOfNow = new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, System.DateTime.Now.Hour, System.DateTime.Now.Minute, System.DateTime.Now.Second);
            TaskDao = new TaskDAO(id, "backlog", assigneeEmaill, title, description, DateOfNow, dueDate); // and then the default boardId.
            Id = id;
            AssigneeEmail = assigneeEmaill;
            CreationTime = DateOfNow;
            Title = title;
            Description = description;
            DueDate = dueDate;
            TaskDao.Persist();   // this won't do anything, this is for the seperation of concerns
        }


        internal TaskBL(TaskDAO taskDao)    //    <<------------------------  constructor for loading data into it.
        {
            TaskDao = taskDao;
            Id = taskDao.Id;
            AssigneeEmail = taskDao._AssigneeEmail;
            CreationTime = taskDao._CreationTime;
            Title = taskDao._Title;
            Description = taskDao._Description;
            DueDate = taskDao._DueDate;
            TaskDao.IsPersisted = true;
        }



        /// <summary>
        /// deletes the current task.
        /// </summary>
        internal void DeleteTask()
        {
            TaskDao.DeleteThisTask();
        }


        /// <summary>
        /// advances the current task according to the rules.
        /// </summary>
        internal void TaskAdvance()
        {
            if (TaskDao._SourceColumnName.Equals("backlog"))
            {
                TaskDao._SourceColumnName = "in progress";   // this will go the set there and use the controller to update
            }
            else if (TaskDao._SourceColumnName.Equals("in progress"))
            {
                TaskDao._SourceColumnName = "done";   // this will go the set there and use the controller to update
            }
        }



    }
}
