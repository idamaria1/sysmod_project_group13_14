﻿using Microsoft.AspNetCore.Mvc;
using SysModelBank.Data.Models;
using SysModelBank.Data.Models.Identity;
using SysModelBank.Data.Repositories;
using SysModelBank.Data.Repositories.Identity;
using SysModelBank.Extensions;
using SysModelBank.Models.Identity;
using System.Threading.Tasks;

namespace SysModelBank.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUserRepository _userRepository;
        public AccountController(IAccountRepository accountRepository, IUserRepository userRepository)
        {
            _accountRepository = accountRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userRepository.GetAsync(User.Id());

            return View(MapToUserModel(user));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount()
        {
             await _accountRepository.CreateAsync(new Account
            {
                UserId = User.Id()
            });

            return RedirectToAction("Index", "Overview");
        }

        private UserModel MapToUserModel(User user) =>
            new UserModel
            {
                Address = user.Address,
                Email = user.Email,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Phone = user.PhoneNumber,
                Username = user.UserName,
                Status = user.Status
            };
    }
}
