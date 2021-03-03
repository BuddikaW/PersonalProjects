using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using IMSWebPortal.Data;
using IMSWebPortal.Data.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.DataProtection;
using IMSWebPortal.Pages.Dtos;
using Microsoft.AspNetCore.Authorization;
using IMSWebPortal.Pages.Dtos.DtoMapping;

namespace IMSWebPortal.Pages.ManageUser
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<IndexModel> _logger;
        public readonly IDataProtectionProvider _provider;

        public IndexModel(ApplicationDbContext context,
            UserManager<AppUser> userManager,
            IDataProtectionProvider provider,
            ILogger<IndexModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _provider = provider;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public IList<UserDetailModel> UserDetails { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            var userName = await _userManager.GetUserNameAsync(user);
            Username = userName;
            var allUsers = _context.Users.Where(e => e.UserName != Username).OrderBy(e => e.IsEnabled).ToList();
            var userList = new UserDtoMap(_provider).Map(allUsers);
            UserDetails = userList;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                foreach (var userData in UserDetails)
                {
                    var userName = userData.UserName;
                    var existingUser = _context.Users.Where(e => e.UserName == userName).FirstOrDefault();
                    if (existingUser == null)
                    {
                        StatusMessage = "Error: Something went wrong!";
                        return Page();
                    }
                    else
                    {
                        if (userData.IsEnabled != existingUser.IsEnabled)
                        {
                            existingUser.IsEnabled = userData.IsEnabled;
                            _context.Users.Update(existingUser);
                            _context.SaveChanges();
                        }
                    }
                }
                _logger.LogInformation("User Details Updated");
                StatusMessage = "User details updated successfully";
                return RedirectToPage();
            }
            return Page();
        }
    }
}
