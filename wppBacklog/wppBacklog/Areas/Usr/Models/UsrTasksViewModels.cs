﻿using clsBacklog.Models;
using wppBacklog.Models;

namespace wppBacklog.Areas.Usr.Models
{
    public class UsrTasksViewModels
    {
    }

    public class UsrTaskIndexViewModel : ViewBaseModel
    {
        public UsrTaskIndexViewModel(ProjectModel project, OrganizationModel organization,
            PaginationModel<TaskViewModel> tasks, IList<ProjectMemberViewModel> assignableMembers,
            IList<TaskTypeModel> taskTypes, IList<TaskStatusModel> taskStatuses, IList<TaskCategoryModel> taskCategories,
            IList<TaskMilestoneModel> taskMilestones, IList<TaskVersionModel> taskVersions, UsersSavedSearch usersSavedSearch)
        {
            Tasks = tasks;
            Project = project;
            Organization = organization;
            AssignableMembers = assignableMembers;
            TaskTypes = taskTypes;
            TaskStatuses = taskStatuses;
            TaskCategories = taskCategories;
            TaskMilestones = taskMilestones;
            TaskVersions = taskVersions;
            UsersSavedSearch = usersSavedSearch;
        }

        public OrganizationModel Organization { get; set; }
        public ProjectModel Project { get; set; }
        public PaginationModel<TaskViewModel> Tasks { get; set; }

        public IList<ProjectMemberViewModel> AssignableMembers { get; set; }
        public IList<TaskTypeModel> TaskTypes { get; set; }
        public IList<TaskStatusModel> TaskStatuses { get; set; }
        public IList<TaskCategoryModel> TaskCategories { get; set; }
        public IList<TaskMilestoneModel> TaskMilestones { get; set; }
        public IList<TaskVersionModel> TaskVersions { get; set; }
        public UsersSavedSearch UsersSavedSearch { get; set; }
        public DateTime DefaultStartTime { get; set; }
        public DateTime DefaultEndTime { get; set; }
    }

    public class UsrTaskUpsertViewModel : ViewBaseModel
    {
        public UsrTaskUpsertViewModel(ProjectModel project, OrganizationModel organization, TaskModel task)
        {
            Task = task;
            Project = project;
            Organization = organization;
        }
        public OrganizationModel Organization { get; set; }
        public ProjectModel Project { get; set; }
        public TaskModel Task { get; set; }
    }

    public class UsrTaskDetailsViewModel : ViewBaseModel
    {
        public UsrTaskDetailsViewModel(ProjectModel project, OrganizationModel organization,
            TaskViewModel task, IList<TaskUpdateModel> logs, IList<ProjectMemberViewModel> assignableMembers,
            IList<TaskTypeModel> taskTypes, IList<TaskStatusModel> taskStatuses, IList<TaskCategoryModel> taskCategories,
            IList<TaskMilestoneModel> taskMilestones, IList<TaskVersionModel> taskVersions, IList<TaskCompletionReasonModel> taskCompletionReasons)
        {
            Task = task;
            Project = project;
            Organization = organization;
            AssignableMembers = assignableMembers;
            TaskTypes = taskTypes;
            TaskStatuses = taskStatuses;
            TaskCategories = taskCategories;
            TaskMilestones = taskMilestones;
            TaskVersions = taskVersions;
            TaskCompletionReasons = taskCompletionReasons;
            TaskLogs = logs;
        }

        public OrganizationModel Organization { get; set; }
        public ProjectModel Project { get; set; }
        public TaskViewModel Task { get; set; }
        public IList<ProjectMemberViewModel> AssignableMembers { get; set; }
        public IList<TaskTypeModel> TaskTypes { get; set; }
        public IList<TaskStatusModel> TaskStatuses { get; set; }
        public IList<TaskCategoryModel> TaskCategories { get; set; }
        public IList<TaskMilestoneModel> TaskMilestones { get; set; }
        public IList<TaskVersionModel> TaskVersions { get; set; }
        public IList<TaskCompletionReasonModel> TaskCompletionReasons { get; set; }
        public IList<TaskUpdateModel>? TaskLogs { get; set; }
    }

    public class UsrTasksGuntViewModel : ViewBaseModel
    {
        public UsrTasksGuntViewModel(ProjectModel project, OrganizationModel organization)
        {
            Project = project;
            Organization = organization;
        }
        public OrganizationModel Organization { get; set; }
        public ProjectModel Project { get; set; }
    }

    public class UsrTasksBoardViewModel : ViewBaseModel
    {
        public UsrTasksBoardViewModel(ProjectModel project, OrganizationModel organization)
        {
            Project = project;
            Organization = organization;
        }
        public OrganizationModel Organization { get; set; }
        public ProjectModel Project { get; set; }
    }
}
