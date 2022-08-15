﻿using clsBacklog.Interfaces;
using clsBacklog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using wppBacklog.Areas.Usr.Models;

namespace wppBacklog.Areas.Usr.Controllers
{
    [Area("Usr"), Authorize()]
    public class ProjectsController : Controller
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly IOrganizationServices _organizationServices;
        private readonly IProjectServices _projectServices;
        private readonly ITaskServices _taskServices;

        public ProjectsController(UserManager<UserModel> userManager,
            IProjectServices projectServices, IOrganizationServices organizationServices,
            ITaskServices taskServices)
        {
            _userManager = userManager;
            _projectServices = projectServices;
            _taskServices = taskServices;
            _organizationServices = organizationServices;
        }

        /// <summary>
        /// Show projects.
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="keyword"></param>
        /// <param name="sort"></param>
        /// <param name="currentPage"></param>
        /// <param name="itemsPerPage"></param>
        /// <returns></returns>
        [Route("/{culture}/projects")]
        public async Task<IActionResult> Index(string culture, string keyword, string sort, int currentPage = 1, int itemsPerPage = 50)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (string.IsNullOrEmpty(currentUser.OrganizationId))
            {
                return NotFound();
            }

            var projects = _projectServices.GetProjects(currentUser.OrganizationId, keyword, sort, currentPage, itemsPerPage);

            var view = new UsrProjectIndexViewModel(projects)
            {
                Culture = culture,
                Title = "Projects"
            };

            return View(view);
        }

        /// <summary>
        /// Create new project.
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="name"></param>
        /// <param name="permaName"></param>
        /// <param name="description"></param>
        /// <param name="displayOrder"></param>
        /// <returns></returns>
        [HttpPost, AutoValidateAntiforgeryToken]
        [Route("/{culture}/project/create")]
        public async Task<IActionResult> CreateProject(string culture, string name,
            string description, int displayOrder)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser.OrganizationId == null)
            {
                return BadRequest();
            }

            var projectId = Guid.NewGuid().ToString();

            // make permaname
            var permaName = "";
            var isPermaNameUnique = false;

            while (!isPermaNameUnique)
            {
                permaName = GetPermaName();

                isPermaNameUnique = _projectServices.IsPermaNameUnique(permaName, currentUser.OrganizationId);
            }

            var project = await _projectServices.CreateProjectAsync(new ProjectModel(projectId,
                permaName, name, currentUser.OrganizationId)
            {
                Description = description,
                DisplayOrder = displayOrder
            });

            // if project is null then likely an error.
            if (project == null)
            {
                return BadRequest();
            }

            return RedirectToAction("Details", new { @culture = culture, @id = project.Id, @rcode = 200 });

        }

        /// <summary>
        /// Generate PermaName.
        /// </summary>
        /// <returns></returns>
        private string GetPermaName()
        {
            string originalChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            string permaName = "";
            Random random = new Random();

            for (int i = 0; i < 8; i++)
            {
                int index = random.Next(originalChars.Length);

                permaName += originalChars[index];
            }

            return permaName;
        }

        /// <summary>
        /// Show detail of the project.
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("/{culture}/project/{id}")]
        public async Task<IActionResult> Details(string culture, string id, int rcode = 0, int currentPage = 1, int itemsPerPage = 50)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser.OrganizationId == null)
            {
                return BadRequest();
            }

            // Later: Make sure this is your project.

            var project = _projectServices.GetProject(id);

            if (project == null)
            {
                return NotFound();
            }

            // Get tatus
            var listOfStatus = _taskServices.GetStatus(project.Id);
            var listOfTypes = _taskServices.GetTaskTypes(project.Id);
            var listOfCategories = _taskServices.GetCategories(project.Id);
            var listOfMilestones = _taskServices.GetMilestones(project.Id);
            var listOfVersion = _taskServices.GetVersions(project.Id);

            // Get members, Later: you should make this into partial view.
            var projectMembers = _projectServices.GetProjectMembersView(project.Id, "", "", currentPage, itemsPerPage);
            var organizationMembers = _organizationServices.GetMembershipInformationByOrganizationIdFullListView(currentUser.OrganizationId, "");

            var view = new UsrProjectDetailsViewModel(project)
            {
                Project = project,
                Culture = culture,
                Title = project.Name,
                RCode = rcode,
                ListOfCategories = listOfCategories,
                ListOfMileStones = listOfMilestones,
                ListOfStatus = listOfStatus,
                ListOfTypes = listOfTypes,
                ListOfVersions = listOfVersion,
                ProjectMembers = projectMembers,
                OrganizationMembers = organizationMembers
            };

            return View(view);
        }

        /// <summary>
        /// Set active project id and go back.
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, AutoValidateAntiforgeryToken]
        [Route("/{culture}/project/active")]
        public async Task<IActionResult> SetActiveProject(string culture, string id)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            currentUser.LastProjectId = id;

            await _userManager.UpdateAsync(currentUser);

            return RedirectToAction("Details", new { @id = id, @culture = culture, @rcode = 201 });
        }


        [HttpPost, AutoValidateAntiforgeryToken]
        [Route("/{culture}/project/status/upsert")]
        public async Task<IActionResult> UpsertStatus(string culture, string projectId, string id, string name, string color, int displayOrder)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString();
                var createResult = await _taskServices.CreateStatusAsync(new TaskStatusModel(
                    projectId, id, name, displayOrder, color));

                if (createResult == null)
                {
                    return BadRequest();
                }

                return RedirectToAction("Details", new { @culture = culture, @id = createResult.ProjectId, @rcode = 210 });
            }

            var updateResult = await _taskServices.UpdateStatusAsync(new TaskStatusModel(projectId,
                id, name, displayOrder, color)
            {
            });

            if (updateResult == null)
            {
                return BadRequest();
            }

            return RedirectToAction("Details", new { @culture = culture, @id = updateResult.ProjectId, @rcode = 220 });

        }

        [HttpPost, AutoValidateAntiforgeryToken]
        [Route("/{culture}/project/status/delete")]
        public async Task<IActionResult> DeleteStatus(string culture, string projectId, string id)
        {
            var result = await _taskServices.DeleteStatusAsync(id);

            if (result == null)
            {
                return BadRequest();
            }

            return RedirectToAction("Details", new { @culture = culture, @id = projectId, @rcode = 230 });

        }

        [HttpPost, AutoValidateAntiforgeryToken]
        [Route("/{culture}/project/type/upsert")]
        public async Task<IActionResult> UpsertType(string culture, string projectId, string id, string name, string color, int displayOrder)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString();
            }

            var result = await _taskServices.CreateTaskTypeAsync(new TaskTypeModel(
                projectId, id, name, displayOrder, color));

            if (result == null)
            {
                return BadRequest();
            }

            return RedirectToAction("Details", new { @culture = culture, @id = result.ProjectId, @rcode = 211 });
        }

        [HttpPost, AutoValidateAntiforgeryToken]
        [Route("/{culture}/project/category/upsert")]
        public async Task<IActionResult> UpsertCategory(string culture, string projectId, string id, string name, int displayOrder)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString();
            }

            var result = await _taskServices.CreateCategoryAsync(new TaskCategoryModel(
                projectId, id, name, displayOrder));

            if (result == null)
            {
                return BadRequest();
            }

            return RedirectToAction("Details", new { @culture = culture, @id = result.ProjectId, @rcode = 212 });
        }

        [HttpPost, AutoValidateAntiforgeryToken]
        [Route("/{culture}/project/milestone/upsert")]
        public async Task<IActionResult> UpsertMilestone(string culture, string projectId, string id, string name, int displayOrder)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString();
            }

            var result = await _taskServices.CreateMilestonesAsync(new TaskMilestoneModel(
                projectId, id, name, displayOrder));

            if (result == null)
            {
                return BadRequest();
            }

            return RedirectToAction("Details", new { @culture = culture, @id = result.ProjectId, @rcode = 213 });
        }

        [HttpPost, AutoValidateAntiforgeryToken]
        [Route("/{culture}/project/version/upsert")]
        public async Task<IActionResult> UpsertVersion(string culture, string projectId, string id, string name, int displayOrder)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString();
            }

            var result = await _taskServices.CreateVersionAsync(new TaskVersionModel(
                projectId, id, name, displayOrder));

            if (result == null)
            {
                return BadRequest();
            }

            return RedirectToAction("Details", new { @culture = culture, @id = result.ProjectId, @rcode = 214 });
        }

    }
}