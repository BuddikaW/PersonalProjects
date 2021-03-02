using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using IMSWebPortal.Data;
using IMSWebPortal.Data.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IMSWebPortal.Pages.User
{
    [Authorize(Roles = "Admin")]
    public class ManageUsersModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IDataProtector _protector;

        public ManageUsersModel(ApplicationDbContext context,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IDataProtectionProvider provider)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _protector = provider.CreateProtector("empite");
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public IList<UserDetailModel> UserDetails { get; set; }

        public class UserDetailModel
        {
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Display(Name = "Username")]
            public string Username { get; set; }

            [Display(Name = "Is Active")]
            public bool IsEnabled { get; set; }
        }

        public String Decript(string input)
        {
            return _protector.Unprotect(input);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userName = await _userManager.GetUserNameAsync(user);
            Username = userName;

            var allUsers = _context.Users.Where(e=>e.UserName != Username).OrderBy(e => e.IsEnabled).ToList();

            var userList = new List<UserDetailModel>();

            foreach(var userProfile in allUsers)
            {
                var userDetail = new UserDetailModel();
                userDetail.FirstName = Decript(userProfile.FirstName);
                userDetail.LastName = Decript(userProfile.LastName);
                userDetail.Username = userProfile.UserName;
                userDetail.IsEnabled = userProfile.IsEnabled;
                userList.Add(userDetail);
            }

            UserDetails = userList;

            return Page();
        }
    }
}
