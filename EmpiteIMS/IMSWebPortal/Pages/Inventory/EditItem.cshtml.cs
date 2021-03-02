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
    public class EditItemModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EditItemModel> _logger;

        public EditItemModel(ApplicationDbContext context, UserManager<AppUser> userManager, ILogger<EditItemModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public string ReturnUrl { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public ItemModel Input { get; set; }

        public class ItemModel
        {
            [Required]
            [Display(Name = "Id")]
            public int Id { get; set; }

            [Required]
            [Display(Name = "Item Name")]
            public string Name { get; set; }

            [Required]
            [Display(Name = "Sku")]
            public string Sku { get; set; }

            [Display(Name = "PrvSku")]
            public string PrvSku { get; set; }

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

            var itemModel = new ItemModel();
            itemModel.Id = item.Id;
            itemModel.Name = item.Name;
            itemModel.Sku = item.Sku;
            itemModel.Price = item.Price;
            itemModel.Qty = item.Qty;
            itemModel.PrvSku = item.Sku;

            Input = itemModel;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                if(Input.Sku != Input.PrvSku && _context.ItemDetails.Any(e=>e.Sku == Input.Sku && e.IsDeleted == false))
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
