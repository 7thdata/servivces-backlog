﻿using clsBacklog.Models;
using wppBacklog.Models;

namespace wppBacklog.Areas.Usr.Models
{
    public class UsrTasksViewModels
    {
    }

    public class UsrTaskIndexViewModel : ViewBaseModel
    {
        public UsrTaskIndexViewModel(ProjectModel project, PaginationModel<TaskModel> tasks)
        {
            Tasks = tasks;
            Project = project;
        }

        public ProjectModel Project { get; set; }
        public PaginationModel<TaskModel> Tasks { get; set; }
    }

    public class UsrTasksGuntViewModel : ViewBaseModel
    {
        public UsrTasksGuntViewModel(ProjectModel project)
        {
            Project = project;
        }
        public ProjectModel Project { get; set; }
    }

    public class UsrTasksBoardViewModel : ViewBaseModel
    {
        public UsrTasksBoardViewModel(ProjectModel project)
        {
            Project = project;
        }
        public ProjectModel Project { get; set; }
    }
}
