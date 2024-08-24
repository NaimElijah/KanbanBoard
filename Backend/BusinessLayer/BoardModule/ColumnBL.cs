using IntroSE.Kanban.Backend.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.DataAccessLayer.DAOs;

namespace IntroSE.Kanban.Backend.BusinessLayer.BoardModule
{
    internal class ColumnBL
    {
        internal ColumnDAO ColumnDao { get; set; }
        internal string Name { get; set; }
        internal Dictionary<long, TaskBL> Tasks { get; set; }

        private long tasksLimit;
        internal long TasksLimit {
            get { return tasksLimit; }
            set
            {
                ColumnDao.TasksLimit = tasksLimit;  // and this goes to the DAO's set
                tasksLimit = value;
            }
        }


        internal ColumnBL(long sourceBoardId, string name, long tasksLimit)
        {
            ColumnDao = new ColumnDAO(name, sourceBoardId, tasksLimit);
            Name = name;
            Tasks = new Dictionary<long, TaskBL>();
            TasksLimit = tasksLimit;
            ColumnDao.Persist();
        }

        internal ColumnBL(ColumnDAO columnDao)    //    <<------------------------  constructor for loading data into it.
        {
            ColumnDao = columnDao;
            Name = columnDao.Name;
            TasksLimit = columnDao.TasksLimit;


            Tasks = new Dictionary<long, TaskBL>();

            List<TaskDAO> taskDaos = ColumnDao.GetTasks();

            List<TaskBL> taskBls = new List<TaskBL>();  // for adding the tasks to here and then arranging in the Dict in this class

            foreach (TaskDAO t in taskDaos)
            {
                taskBls.Add(new TaskBL(t));
            }

            foreach (TaskBL taskbl in taskBls) // now add the tasks in the taskBls List, to the Dict in this class according to their key(their Id in the board).
            {
                Tasks.Add(taskbl.Id, taskbl);
            }

            ColumnDao.IsPersisted = true;
        }



        /// <summary>
        /// deletes the current column and all of the tasks in it.
        /// </summary>
        internal void DeleteCol()
        {
            foreach (TaskBL taskInCol in Tasks.Values)
            {
                taskInCol.DeleteTask();
            }

            ColumnDao.DeleteThisColumn();
        }


    }
}
