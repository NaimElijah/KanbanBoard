using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.DataAccessLayer.DALControllers;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DAOs
{
    internal class ColumnDAO
    {
        internal ColumnController CController { get; set; }
        internal bool IsPersisted { get; set; }
        internal long SourceBoardId { get; set; }
        internal string Name { get; set; }

        internal long tasksLimit;
        internal long TasksLimit
        {
            get => tasksLimit;
            set
            {
                if (IsPersisted)
                {
                    CController.UpdateColumn(SourceBoardId, Name, TasksLimitCol, value);
                }
                tasksLimit = value;
            }
        }

        private const string ColumnNameCol = "ColumnName";
        private const string SourceBoardIdCol = "SourceBoardId";
        private const string TasksLimitCol = "TasksLimit";  //  the attributeNames are to be used when doing the set methods here/doing the updates.

        private const string ColumnNameColInTasksTable = "SourceColumnName";

        internal TaskController Tcontroller { get; set; }  // for loadData

        internal ColumnDAO(string name, long sourceBoardId, long tasksLimit)
        {
            CController = new ColumnController();
            Tcontroller = new TaskController();  // for loadData
            IsPersisted = false;
            SourceBoardId = sourceBoardId;
            Name = name;
            TasksLimit = tasksLimit;
        }



        /// <summary>
        /// inserting a column to the database.
        /// </summary>
        internal void Persist()
        {
            if (!IsPersisted)
            {
                CController.Insert(this);
                IsPersisted = true;
            }
        }



        /// <summary>
        /// get a List of all of this column's TaskDAOs.
        /// </summary>
        /// <returns>get a List of all of this column's TaskDAOs.</returns>
        internal List<TaskDAO> GetTasks()  // <<------------------  for loading
        {
            return Tcontroller.SelectTasks(SourceBoardIdCol, SourceBoardId, ColumnNameColInTasksTable, Name);
        }


        /// <summary>
        /// deletes the current column from the database/table.
        /// </summary>
        internal void DeleteThisColumn(){
            CController.DeleteColumn(SourceBoardId, Name);    
        }



    }
}
