using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using IMSWebPortal.Areas;
using IMSWebPortal.Data;
using IMSWebPortal.Data.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace IMSWebPortal.Pages.ManageInventory
{
    [Authorize(Roles = "Admin,Manager")]
    public class SendReportModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SendReportModel> _logger;

        public SendReportModel(ApplicationDbContext context, UserManager<AppUser> userManager, ILogger<SendReportModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public string ReturnUrl { get; set; }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public EmailModel Input { get; set; }

        public class EmailModel
        {
            [Required]
            [Display(Name = "Recipient List")]
            public string RecipientList { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (Input.RecipientList != "")
            {
                var emailList = Input.RecipientList.Split(',');

                var subject = "Test";

                foreach (var email in emailList)
                {
                    var result = new EmailClient(_context).SendEmail("<html><body>Hi</body></html>", email, subject);
                }
            }
            return Page();
        }
    }
}
