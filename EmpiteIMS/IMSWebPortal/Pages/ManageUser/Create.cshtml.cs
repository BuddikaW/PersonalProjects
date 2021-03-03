using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using IMSWebPortal.Data;
using IMSWebPortal.Data.Models.Identity;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using IMSWebPortal.Pages.Dtos.DtoMapping;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace IMSWebPortal.Pages.ManageUser
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly IDataProtectionProvider _provider;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<CreateModel> _logger;
        private readonly IEmailSender _emailSender;

        public CreateModel(IDataProtectionProvider provider,
            RoleManager<AppRole> roleManager,
            UserManager<AppUser> userManager,
            ILogger<CreateModel> logger,
            IEmailSender emailSender)
        {
            _provider = provider;
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public List<SelectListItem> Options { get; set; }

        [BindProperty]
        public RegistrationModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class RegistrationModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 5)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "User Type")]
            public string UserTypeName { get; set; }
        }

        public class UserType
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            var roles = _roleManager.Roles;
            var userTypes = new List<UserType>();
            foreach(var role in roles)
            {
                var userType = new UserType();
                userType.Id = role.Id;
                userType.Name = role.Name;
                userTypes.Add(userType);
            }

            Options = userTypes.OrderByDescending(e=>e.Name).Select(e => new SelectListItem
            {
                Value = e.Name.ToString(),
                Text = e.Name
            }).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                //Encript User Details
                var firstName = new UserDtoMap(_provider).Encript(Input.FirstName);
                var lastName = new UserDtoMap(_provider).Encript(Input.LastName);

                if (await _userManager.FindByEmailAsync(Input.Email) != null)
                {
                    StatusMessage = "Error: User Name " + Input.Email + " already exist";
                    return RedirectToPage();
                }

                var user = new AppUser { UserName = Input.Email, Email = Input.Email, FirstName = firstName, LastName = lastName, IsEnabled = true };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, Input.UserTypeName);
                    }

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        StatusMessage = "New user created successfully";
                        return RedirectToPage("./Index");
                    }
                }
                var errorList = "";
                foreach (var error in result.Errors)
                {
                    errorList += error.Description + "/";
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                StatusMessage = "Error: [" + errorList + "]";
            }
            return RedirectToPage();
            //return Page();
        }
    }
}
