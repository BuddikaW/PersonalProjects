using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using IMSWebPortal.Data;
using IMSWebPortal.Data.Models.Identity;
using IMSWebPortal.Data.Models.Inventory;
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
        public ItemModel Input { get; set; }

        public class ItemModel
        {
            [Required]
            [Display(Name = "Item Name")]
            public string Name { get; set; }

            [Required]
            [Display(Name = "Sku")]
            public string Sku { get; set; }

            [Required]
            [Display(Name = "Price")]
            //[RegularExpression(@"^\$?\d+(\.(\d{2}))?$", ErrorMessage = "Please enter a valid price")]
            [Range(0, 10000000000, ErrorMessage = "Please enter a valid price")]
            public decimal Price { get; set; }

            [Required]
            [Display(Name = "Available Qty")]
            [Range(0, 10000000000, ErrorMessage = "Please enter a valid qty")]
            [RegularExpression("([0-9]+)", ErrorMessage = "Please enter a valid qty")]
            //[Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
            public int Qty { get; set; }
        }

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
