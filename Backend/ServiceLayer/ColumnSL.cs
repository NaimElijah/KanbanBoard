using IntroSE.Kanban.Backend.BusinessLayer.BoardModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    internal class ColumnSL
    {
        public string Name { get; set; }
        public Dictionary<long, TaskSL> Tasks { get; set; }
        public long TasksLimit { get; set; }

        internal ColumnSL(ColumnBL colbl)
        {
            Name = colbl.Name;
            Tasks = new Dictionary<long, TaskSL>();
            TasksLimit = colbl.TasksLimit;

            foreach (TaskBL task in colbl.Tasks.Values)
            {
                Tasks.Add(task.Id, new TaskSL(task));
            }

        }



    }
}
