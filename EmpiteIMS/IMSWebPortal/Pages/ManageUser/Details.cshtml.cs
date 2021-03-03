using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using IMSWebPortal.Data;
using IMSWebPortal.Data.Models.Identity;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using IMSWebPortal.Pages.Dtos.DtoMapping;
using Microsoft.AspNetCore.Authorization;

namespace IMSWebPortal.Pages.ManageUser
{
    [Authorize(Roles = "Admin")]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IDataProtectionProvider _provider;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public DetailsModel(ApplicationDbContext context,
            IDataProtectionProvider provider,
            RoleManager<AppRole> roleManager,
            UserManager<AppUser> userManager)
        {
            _context = context;
            _provider = provider;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [BindProperty]
        public RegistrationModel Input { get; set; }

        public class RegistrationModel
        {
            [Display(Name = "Id")]
            public int Id { get; set; }

            [Display(Name = "Email")]
            public string Email { get; set; }

            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Display(Name = "User Type")]
            public string UserTypeName { get; set; }
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

            if (_context.UserRoles.Any(e => e.UserId == id))
            {
                var userRoleId = _context.UserRoles.FirstOrDefault(e => e.UserId == id).RoleId;
                var userRole = _roleManager.Roles.FirstOrDefault(e => e.Id == userRoleId);
                if (userRole != null)
                {
                    registrationModel.UserTypeName = userRole.Name;
                }
            }
            Input = registrationModel;
            if (Input == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
