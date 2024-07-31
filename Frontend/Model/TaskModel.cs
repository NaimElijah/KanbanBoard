namespace IntroSE.Kanban.Backend.ServiceLayer
{
    internal class TaskModel
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public TaskModel(TaskSL task)
        {
            this.Title = task.Title;
            this.Description = task.Description;
        }
    }
}