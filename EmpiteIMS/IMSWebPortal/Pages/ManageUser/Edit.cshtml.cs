using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IMSWebPortal.Data;
using IMSWebPortal.Data.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using IMSWebPortal.Pages.Dtos.DtoMapping;

namespace IMSWebPortal.Pages.ManageUser
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IDataProtectionProvider _provider;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<EditModel> _logger;

        public EditModel(ApplicationDbContext context,
            IDataProtectionProvider provider,
            RoleManager<AppRole> roleManager,
            UserManager<AppUser> userManager,
            ILogger<EditModel> logger)
        {
            _context = context;
            _provider = provider;
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
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
            public int Id { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Prv Email")]
            public string PrvEmail { get; set; }

            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [Display(Name = "User Type")]
            public string UserTypeName { get; set; }

            [Display(Name = "Prv User Type")]
            public string PrvUserType { get; set; }
        }

        public class UserType
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = _userManager.Users.FirstOrDefault(e => e.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var registrationModel = new RegistrationModel();
            registrationModel.Id = user.Id;
            registrationModel.FirstName = new UserDtoMap(_provider).Decript(user.FirstName);
            registrationModel.LastName = new UserDtoMap(_provider).Decript(user.LastName);
            registrationModel.Email = user.Email;
            registrationModel.PrvEmail = user.Email;

            if (_context.UserRoles.Any(e => e.UserId == id))
            {
                var userRoleId = _context.UserRoles.FirstOrDefault(e => e.UserId == id).RoleId;
                var userRole = _roleManager.Roles.FirstOrDefault(e => e.Id == userRoleId);
                if(userRole != null)
                {
                    registrationModel.UserTypeName = userRole.Name;
                    registrationModel.PrvUserType = userRole.Name;
                }
            }

            Input = registrationModel;

            if (Input == null)
            {
                return NotFound();
            }

            var roles = _roleManager.Roles;
            var userTypes = new List<UserType>();
            foreach (var role in roles)
            {
                var userType = new UserType();
                userType.Id = role.Id;
                userType.Name = role.Name;
                userTypes.Add(userType);
            }

            var optionList = new List<SelectListItem>();
            foreach(var userType in userTypes.OrderByDescending(e => e.Name))
            {
                var option = new SelectListItem();
                option.Value = userType.Name.ToString();
                option.Text = userType.Name;
                if(Input.UserTypeName == userType.Name)
                {
                    option.Selected = true;
                }
                else
                {
                    option.Selected = false;
                }
                optionList.Add(option);
            }

            Options = optionList;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Input.PrvEmail != Input.Email && await _userManager.FindByEmailAsync(Input.Email) != null)
            {
                StatusMessage = "Error: User Name " + Input.Email + " already exist";
                return RedirectToPage("./Edit",new { id = Input.Id });
            }

            var user = _userManager.Users.FirstOrDefault(e => e.Email == Input.PrvEmail);
            user.Email = Input.Email;
            user.UserName = Input.Email;
            user.FirstName = new UserDtoMap(_provider).Encript(Input.FirstName);
            user.LastName = new UserDtoMap(_provider).Encript(Input.LastName);
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                if(Input.PrvUserType != Input.UserTypeName)
                {
                    var existingUserRole = _context.UserRoles.FirstOrDefault(e => e.UserId == user.Id);
                    if(existingUserRole != null)
                    {
                        _context.UserRoles.Remove(existingUserRole);
                        _context.SaveChanges();
                    }
                    var roleassign = await _userManager.AddToRoleAsync(user, Input.UserTypeName);
                    if (roleassign.Succeeded)
                    {
                        StatusMessage = "User details updated successfully";
                        return RedirectToPage("./Index");
                    }
                    StatusMessage = "Error: Something went wrong!";
                    return RedirectToPage("./Edit", new { id = Input.Id });
                }
                StatusMessage = "User details updated successfully";
                return RedirectToPage("./Index");
            }
            StatusMessage = "Error: Something went wrong!";
            return RedirectToPage("./Edit", new { id = Input.Id });
        }

    }
}
