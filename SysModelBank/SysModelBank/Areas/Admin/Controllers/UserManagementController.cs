﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SysModelBank.Areas.Admin.Models.UserManagement;
using SysModelBank.Data.Enums;
using SysModelBank.Data.Models.Identity;
using SysModelBank.Data.Repositories.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SysModelBank.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserManagementController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;

        public UserManagementController(IUserRepository userRepository, UserManager<User> userManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetAsync();

            var pending = new List<UserListItem>();
            var deleting = new List<UserListItem>();
            var activeUsers = new List<UserListItem>();

            foreach (var user in users)
            {
                if (user.Status == UserStatus.Active)
                {
                    activeUsers.Add(await MapToListItem(user));
                }
                else if (user.Status == UserStatus.PendingVerification)
                {
                    pending.Add(await MapToListItem(user));
                }
                else if (user.Status == UserStatus.PendingDeletion)
                {
                    deleting.Add(await MapToListItem(user));
                }
            }

            return View(new UserList
            {
                PendingUsers = pending,
                DeletingUsers = deleting,
                Users = activeUsers
            });
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await _userRepository.GetAsync(id);

            return View(await MapToDetail(user));
        }

        [HttpPost]
        public async Task<IActionResult> Verify(int id)
        {
            if (!User.IsInRole(Role.Admin))
            {
                return RedirectToAction("Details", new { id });
            }

            var user = await _userRepository.GetAsync(id);

            user.Status = UserStatus.Active;

            await _userRepository.UpdateAsync(user);

            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!User.IsInRole(Role.Admin))
            {
                return RedirectToAction("Details", new { id });
            }

            var user = await _userRepository.GetAsync(id);

            user.Status = UserStatus.Deleted;

            await _userRepository.UpdateAsync(user);

            return RedirectToAction("Index");
        }

        private async Task<UserListItem> MapToListItem(User user)
        {
            return new UserListItem
            {
                Id = user.Id,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault(),
                AccountCount = user.Accounts.Count()
            };
        }

        private async Task<UserDetail> MapToDetail(User user)
        {
            return new UserDetail
            {
                Id = user.Id,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                UserName = user.UserName,
                Address = user.Address,
                Email = user.Email,
                Phone = user.PhoneNumber,
                Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault(),
                Status = user.Status
            };
        }
    }
}