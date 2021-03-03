using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IMSWebPortal.Data;
using IMSWebPortal.Data.Models.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using IMSWebPortal.Data.Models.Identity;
using IMSWebPortal.Pages.Dtos;
using IMSWebPortal.Pages.Dtos.DtoMapping;

namespace IMSWebPortal.Pages.ManageInventory
{
    [Authorize(Roles = "Admin,Manager")]
    public class EditModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EditModel> _logger;

        public EditModel(ApplicationDbContext context, UserManager<AppUser> userManager, ILogger<EditModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public string ReturnUrl { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public ItemDetailModel Input { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var item = _context.ItemDetails.Where(e => e.Id == id).FirstOrDefault();
            if (item == null)
            {
                return NotFound();
            }
            Input = new ItemDtoMap().Map(item);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                if (Input.Sku != Input.PrvSku && _context.ItemDetails.Any(e => e.Sku == Input.Sku && e.IsDeleted == false))
                {
                    StatusMessage = "Error: Duplicate SKU value. Item with SKU #" + Input.Sku + " already exist.";
                    return Page();
                }

                var thisItem = _context.ItemDetails.Where(e => e.IsDeleted == false && e.Sku == Input.PrvSku).FirstOrDefault();
                thisItem.Name = Input.Name;
                thisItem.Sku = Input.Sku;
                thisItem.Price = Input.Price;
                thisItem.Qty = Input.Qty;

                _context.ItemDetails.Update(thisItem);
                _context.SaveChanges();

                _logger.LogInformation("Item Updated.");

                StatusMessage = "Item updated successfully";

                return Page();
            }

            return Page();
        }
    }
}
