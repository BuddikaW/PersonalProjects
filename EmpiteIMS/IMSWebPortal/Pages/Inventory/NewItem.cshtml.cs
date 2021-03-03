using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using IMSWebPortal.Data;
using IMSWebPortal.Data.Models.Identity;
using IMSWebPortal.Data.Models.Inventory;
using IMSWebPortal.Pages.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace IMSWebPortal.Pages.Inventory
{
    [Authorize(Roles = "Admin,Manager")]
    public class NewItemModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NewItemModel> _logger;

        public NewItemModel(ApplicationDbContext context, UserManager<AppUser> userManager, ILogger<NewItemModel> logger)
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
                if(_context.ItemDetails.Any(e => e.Sku == Input.Sku && e.IsDeleted == false))
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
