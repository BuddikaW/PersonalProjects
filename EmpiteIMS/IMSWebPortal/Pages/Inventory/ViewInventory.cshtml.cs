using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using IMSWebPortal.Data;
using IMSWebPortal.Data.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace IMSWebPortal.Pages.Inventory
{
    [Authorize(Roles = "Admin,Manager,Viewer")]
    public class ViewInventoryModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<ViewInventoryModel> _logger;

        public ViewInventoryModel(ApplicationDbContext context, UserManager<AppUser> userManager, ILogger<ViewInventoryModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public IList<ItemDetailsModel> ItemDetils { get; set; }

        public class ItemDetailsModel
        {
            [Display(Name = "Id")]
            public int Id { get; set; }

            [Display(Name = "Item Name")]
            public string Name { get; set; }

            [Display(Name = "Sku")]
            public string Sku { get; set; }

            [Display(Name = "Price")]
            public decimal Price { get; set; }

            [Display(Name = "Qty")]
            public int Qty { get; set; }

            [Display(Name = "Is Deleted")]
            public bool IsDeleted { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userName = await _userManager.GetUserNameAsync(user);
            Username = userName;

            var allItems = _context.ItemDetails.Where(e => e.IsDeleted == false).OrderBy(e => e.Name).ToList();

            var itemList = new List<ItemDetailsModel>();

            foreach (var itemData in allItems)
            {
                var itemRecord = new ItemDetailsModel();
                itemRecord.Id = itemData.Id;
                itemRecord.Name = itemData.Name;
                itemRecord.Sku = itemData.Sku;
                itemRecord.Price = itemData.Price;
                itemRecord.Qty = itemData.Qty;
                itemRecord.IsDeleted = false;
                itemList.Add(itemRecord);
            }

            ItemDetils = itemList;

            return Page();
        }
    }
}
