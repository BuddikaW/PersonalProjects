using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using IMSWebPortal.Data;
using IMSWebPortal.Data.Models.Inventory;
using Microsoft.AspNetCore.Identity;
using IMSWebPortal.Data.Models.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using IMSWebPortal.Pages.Dtos;

namespace IMSWebPortal.Pages.ManageInventory
{
    [Authorize(Roles = "Admin,Manager")]
    public class CreateModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(ApplicationDbContext context, UserManager<AppUser> userManager, ILogger<CreateModel> logger)
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
        public ItemDetailModel Input { get; set; }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                if (_context.ItemDetails.Any(e => e.Sku == Input.Sku && e.IsDeleted == false))
                {
                    StatusMessage = "Error: Duplicate SKU value. Item with SKU #" + Input.Sku + " already exist.";
                    return Page();
                }

                var item = new ItemDetail { Name = Input.Name, Sku = Input.Sku, Price = Input.Price, Qty = Input.Qty, IsDeleted = false };

                _context.ItemDetails.Add(item);
                _context.SaveChanges();

                _logger.LogInformation("New Item added.");

                StatusMessage = "New item added successfully";

                return RedirectToPage();
            }

            return Page();
        }
    }
}
