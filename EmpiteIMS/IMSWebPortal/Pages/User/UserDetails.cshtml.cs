using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using IMSWebPortal.Data.Models.Identity;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IMSWebPortal.Pages.User
{
    public class UserDetailsModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IDataProtector _protector;

        public UserDetailsModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IDataProtectionProvider provider)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _protector = provider.CreateProtector("empite");
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Display(Name = "Is Active")]
            public bool IsEnabled { get; set; }
        }

        public String Decript(string input)
        {
            return _protector.Unprotect(input);
        }

        public String Encript(string input)
        {
            return _protector.Protect(input);
        }

        private async Task LoadAsync(AppUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);

            Username = userName;

            var firstName = Decript(user.FirstName);
            var lastName = Decript(user.LastName);

            Input = new InputModel
            {
                FirstName = firstName,
                LastName = lastName,
                IsEnabled = user.IsEnabled
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var firstName = Decript(user.FirstName);
            var lastName = Decript(user.LastName);

            if (Input.FirstName != firstName || Input.LastName != lastName)
            {
                user.FirstName = Encript(Input.FirstName);
                user.LastName = Encript(Input.LastName);
                var setNameChange = await _userManager.UpdateAsync(user);
                if (!setNameChange.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to update the name.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
