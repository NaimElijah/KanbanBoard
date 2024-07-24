using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.DataAccessLayer.DALControllers;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DAOs
{
    internal class TaskDAO
    {
        internal TaskController TController { get; set; }
        internal bool IsPersisted { get; set; }
        internal long Id { get; set; }
        internal long SourceBoardId { get; set; }

        private string AssigneeEmail;
        internal string _AssigneeEmail {
            get => AssigneeEmail;
            set
            {
                if (IsPersisted)
                {
                    TController.UpdateTask(this.Id, this.SourceBoardId, TaskAssigneeEmailCol, value);
                }
                AssigneeEmail = value;
            }
        }

        private string SourceColumnName;
        internal string _SourceColumnName {
            get => SourceColumnName;
            set
            {
                if (IsPersisted)
                {
                    TController.UpdateTask(this.Id, this.SourceBoardId, SourceColumnNameCol, value);
                }
                SourceColumnName = value;
            }
        }

        private DateTime CreationTime;
        internal DateTime _CreationTime {
            get => CreationTime;
            set
            {
                if (IsPersisted)
                {
                    TController.UpdateTask(this.Id, this.SourceBoardId, CreationTimeCol, value);
                }
                CreationTime = value;
            }
        }

        private string Title;
        internal string _Title {
            get => Title;
            set
            {
                if (IsPersisted)
                {
                    TController.UpdateTask(this.Id, this.SourceBoardId, TitleCol, value);
                }
                Title = value;
            }
        }

        private string Description;
        internal string _Description {
            get => Description;
            set
            {
                if (IsPersisted)
                {
                    TController.UpdateTask(this.Id, this.SourceBoardId, DescriptionCol, value);
                }
                Description = value;
            }
        }

        private DateTime DueDate;
        internal DateTime _DueDate {
            get => DueDate;
            set
            {
                if (IsPersisted)
                {
                    TController.UpdateTask(this.Id, this.SourceBoardId, DueDateCol, value);
                }
                DueDate = value;
            }
        }

        private const string SourceColumnNameCol = "SourceColumnName";
        private const string TaskAssigneeEmailCol = "TaskAssigneeEmail";
        private const string TitleCol = "Title";
        private const string DescriptionCol = "Description";
        private const string CreationTimeCol = "CreationTime";
        private const string DueDateCol = "DueDate";

        internal TaskDAO(long id, string sourceColumnName, string assigneeEmail, string title, string description, DateTime creationTime, DateTime dueDate, long sourceBoardId = -1)
        {
            TController = new TaskController();
            Id = id;
            AssigneeEmail = assigneeEmail;
            SourceBoardId = sourceBoardId;
            SourceColumnName = sourceColumnName;
            CreationTime = creationTime;
            Title = title;
            Description = description;
            DueDate = dueDate;
            IsPersisted = false;
        }

        /// <summary>
        /// just for separation of concerns, like achiya showed in class. 
        /// </summary>
        internal void Persist()
        {
            return;  
        }


        /// <summary>
        /// inserting a task to the database.
        /// </summary>
        /// <param name="boardId"> the board Id of the task we're inserting</param>
        internal void Persist(long boardId)
        {
            if (!IsPersisted && SourceBoardId == -1)
            {
                try
                {
                    this.SourceBoardId = boardId;
                    TController.Insert(this);
                    IsPersisted = true;
                }
                catch (Exception e)
                {
                    this.SourceBoardId = -1; 
                }
            }
        }


        /// <summary>
        /// deleting this task from the database.
        /// </summary>
        internal void DeleteThisTask()
        {
            TController.DeleteTask(Id, SourceBoardId);
        }


    }
}
