using Frontend.Model;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public class TaskModel
    {

        private string title;
        public string Title
        {
            get => title;
           
        }

        private string description;
        public string Description
        {
            get => description;
         
        }

        internal TaskModel(TaskSL task)
        {
            this.title = task.Title;
            this.description = task.Description;
        }
    }
}
