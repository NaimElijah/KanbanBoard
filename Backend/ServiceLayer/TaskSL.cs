using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.BusinessLayer.BoardModule;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public class TaskSL
    {
        public long Id { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime DueDate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        internal string AssigneeEmail { get; set; }

        public TaskSL() { }

        internal TaskSL(TaskBL taskbl)
        {
            Id = taskbl.Id;
            AssigneeEmail = taskbl.AssigneeEmail;
            CreationTime = taskbl.CreationTime;
            Title = taskbl.Title;
            Description = taskbl.Description;
            DueDate = taskbl.DueDate;
        }

        public TaskSL()
        {
            // paramaterless constructor for deserialization
        }




    }
}
