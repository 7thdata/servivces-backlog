﻿using clsBacklog.Data;
using clsBacklog.Interfaces;
using clsBacklog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clsBacklog.Services
{
    public class ProjectServices : IProjectServices
    {
        private readonly IdentityContext _identity;
        private readonly ApplicationContext _db;
        public ProjectServices(IdentityContext identity,
            ApplicationContext db)
        {
            _identity = identity;
            _db = db;
        }

        /// <summary>
        /// Get projects.
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="sort"></param>
        /// <param name="currentPage"></param>
        /// <param name="itemsPerPage"></param>
        /// <returns></returns>
        public PaginationModel<ProjectModel> GetProjects(string organizationId, string keyword, string sort, int currentPage, int itemsPerPage)
        {
            var projects = from p in _db.Projects where p.IsDeleted == false && p.OwnerId == organizationId select p;

            if (!string.IsNullOrEmpty(keyword))
            {
                // Search logic here.
                projects = projects.Where(g => g.Id.Contains(keyword) || g.Name.Contains(keyword));
            }

            if (!string.IsNullOrEmpty(sort))
            {
                // Sort logic here.
            }
            else
            {
                // Default sort here.
                projects = projects.OrderBy(g => g.Name);
            }

            // Size
            int totalItems = projects.Count();
            int totalPages = 0;

            if (totalItems > 0)
            {
                totalPages = (totalItems / itemsPerPage) + 1;
            }

            // Skip, and take.
            projects = projects.Skip((currentPage - 1) * itemsPerPage);
            projects = projects.Take(itemsPerPage);

            var result = new PaginationModel<ProjectModel>()
            {
                Items = projects.ToList(),
                Sort = sort,
                Keyword = keyword,
                CurrentPage = currentPage,
                ItemsPerPage = itemsPerPage,
                TotalItems = totalItems,
                TotalPages = totalPages
            };


            return result;
        }

        /// <summary>
        /// Get specific project.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ProjectModel? GetProject(string id)
        {
            // Get specific project.
            var project = (from g in _db.Projects where g.Id == id && g.IsDeleted == false select g).FirstOrDefault();

            return project;
        }

        /// <summary>
        /// Create project.
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public async Task<ProjectModel?> CreateProjectAsync(ProjectModel project)
        {
            // Create project

            // Make sure there is no duplicate.
            var original = (from g in _db.Projects where g.Id == project.Id select g).FirstOrDefault();

            if (original != null)
            {
                return null;
            }

            // Timestamp is overwritten here.
            project.Created = DateTime.Now;

            _db.Projects.Add(project);
            await _db.SaveChangesAsync();

            return project;
        }

        /// <summary>
        /// Check to see if perma name is unique.
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="permaName"></param>
        /// <returns></returns>
        public bool IsPermaNameUnique(string accountId, string permaName)
        {
            var name = (from n in _db.Projects where n.PermaName == permaName && n.OwnerId == accountId select n).FirstOrDefault();

            if (name == null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Update project.
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public async Task<ProjectModel?> UpdateProjectAsync(ProjectModel project)
        {
            // Update project

            // Get
            var original = (from g in _db.Projects where g.Id == project.Id select g).FirstOrDefault();

            if (original == null)
            {
                return original;
            }

            original.Name = project.Name;
            original.Description = project.Description;
            original.DisplayOrder = project.DisplayOrder;

            _db.Projects.Update(original);

            await _db.SaveChangesAsync();

            return original;
        }

        /// <summary>
        /// Delete project.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ProjectModel?> DeleteProjectAsync(string id)
        {
            // Delete project, set flag to delete.

            // Get
            var project = (from g in _db.Projects where g.Id == id select g).FirstOrDefault();

            if (project == null)
            {
                return null;
            }

            project.IsDeleted = true;
            project.Deleted = DateTime.Now;

            _db.Projects.Update(project);
            await _db.SaveChangesAsync();

            // If you want to delete physically, uncomment the code below.
            // _db.Projects.Remove(project);
            // await _db.SaveChangesAsync();

            return project;
        }

        /// <summary>
        /// Get project members.
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="userId"></param>
        /// <param name="sort"></param>
        /// <param name="currentPage"></param>
        /// <param name="itemsPerPage"></param>
        /// <returns></returns>
        public PaginationModel<ProjectMemberModel> GetProjectMembers(string projectId, string userId, string sort, int currentPage, int itemsPerPage)
        {
            // Get members of the organization.

            var memberships = from m in _db.ProjectMembers
                              where
                              m.ProjectId == projectId && m.IsDeleted == false
                              select m;

            if (!string.IsNullOrEmpty(userId))
            {
                // If user id is specified.
                memberships = memberships.Where(m => m.UserId == userId);
            }

            if (!string.IsNullOrEmpty(sort))
            {
                // Sort logic here.
            }
            else
            {
                // Default sort here.
                memberships = memberships.OrderBy(m => m.Created);
            }

            // Size
            int totalItems = memberships.Count();
            int totalPages = 0;

            if (totalItems > 0)
            {
                totalPages = (totalItems / itemsPerPage) + 1;
            }

            // Skip, and take.
            memberships = memberships.Skip((currentPage - 1) * itemsPerPage);
            memberships = memberships.Take(itemsPerPage);

            var result = new PaginationModel<ProjectMemberModel>
            {
                Items = memberships.ToList(),
                Sort = sort,
                Keyword = userId,
                CurrentPage = currentPage,
                ItemsPerPage = itemsPerPage,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            return result;
        }

        /// <summary>
        /// Get project member (with member information inside)
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="userId"></param>
        /// <param name="sort"></param>
        /// <param name="currentPage"></param>
        /// <param name="itemsPerPage"></param>
        /// <returns></returns>
        public PaginationModel<ProjectMemberViewModel> GetProjectMembersView(string projectId, string userId, string sort, int currentPage, int itemsPerPage)
        {
            // Get members of the organization.

            var memberships = from m in _db.ProjectMembers
                              where
                              m.ProjectId == projectId && m.IsDeleted == false
                              select new ProjectMemberViewModel(m.Id, m.ProjectId,
                                   (from u in _identity.Users where u.Id == m.UserId && m.IsDeleted == false select u).FirstOrDefault(),
                                   m.MembershipType)
                              {
                                  Created = m.Created
                              };


            if (!string.IsNullOrEmpty(userId))
            {
                // If user id is specified.
            }

            if (!string.IsNullOrEmpty(sort))
            {
                // Sort logic here.
            }
            else
            {
                // Default sort here.
                memberships = memberships.OrderBy(m => m.Created);
            }

            // Size
            int totalItems = memberships.Count();
            int totalPages = 0;

            if (totalItems > 0)
            {
                totalPages = (totalItems / itemsPerPage) + 1;
            }

            // Skip, and take.
            memberships = memberships.Skip((currentPage - 1) * itemsPerPage);
            memberships = memberships.Take(itemsPerPage);

            var result = new PaginationModel<ProjectMemberViewModel>
            {
                Items = memberships.ToList(),
                Sort = sort,
                Keyword = userId,
                CurrentPage = currentPage,
                ItemsPerPage = itemsPerPage,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            return result;
        }

        /// <summary>
        /// Add member to the project.
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="userId"></param>
        /// <param name="membershipType"></param>
        /// <returns></returns>
        public async Task<ProjectMemberModel?> AddMemberToProjectAsync(string projectId, string userId, string membershipType)
        {
            // Make sure he/she isn't in it already.
            var membership = (from m in _db.ProjectMembers
                              where m.ProjectId == projectId
                              && m.UserId == userId && m.IsDeleted == false
                              select m).FirstOrDefault();

            if (membership != null)
            {
                return membership;
            }

            // I want to make sure userId and organizationId exist and valid.
            var user = (from u in _identity.Users
                        where u.IsDeleted == false && u.IsSuspended == false &&
                        u.Id == userId
                        select u).FirstOrDefault();

            if (user == null)
            {
                return null;
            }

            var project = (from o in _db.Projects
                           where o.IsDeleted == false &&
                          o.Id == projectId
                           select o).FirstOrDefault();
            if (project == null)
            {
                return null;
            }

            var newMembership = new ProjectMemberModel(Guid.NewGuid().ToString(), projectId, userId, membershipType)
            {
                Created = DateTime.Now
            };

            _db.ProjectMembers.Add(newMembership);

            await _db.SaveChangesAsync();

            return newMembership;
        }

        /// <summary>
        /// Remove member from the project.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ProjectMemberModel?> RemoveMemberFromProjectAsync(string id)
        {
            // Get the membership entity
            var membership = (from m in _db.ProjectMembers where m.Id == id select m).FirstOrDefault();

            if (membership == null)
            {
                return null;
            }

            membership.IsDeleted = true;
            membership.Deleted = DateTime.Now;

            _db.ProjectMembers.Update(membership);
            await _db.SaveChangesAsync();

            return membership;
        }

        /// <summary>
        /// Update member type.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="membershipType"></param>
        /// <returns></returns>
        public async Task<ProjectMemberModel?> UpdateProjectMemberTypeAsync(string id, string membershipType)
        {
            // Get the membership entity
            var membership = (from m in _db.ProjectMembers where m.Id == id select m).FirstOrDefault();

            if (membership == null)
            {
                return null;
            }

            membership.MembershipType = membershipType;

            _db.ProjectMembers.Update(membership);
            await _db.SaveChangesAsync();

            return membership;
        }
    }
}