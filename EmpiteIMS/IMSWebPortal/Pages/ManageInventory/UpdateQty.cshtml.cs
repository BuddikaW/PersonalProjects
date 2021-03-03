using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IMSWebPortal.Data;
using IMSWebPortal.Data.Models.Identity;
using IMSWebPortal.Pages.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace IMSWebPortal.Pages.ManageInventory
{
    [Authorize(Roles = "Admin,Manager")]
    public class UpdateQtyModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<UpdateQtyModel> _logger;

        public UpdateQtyModel(ApplicationDbContext context, UserManager<AppUser> userManager, ILogger<UpdateQtyModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public IList<ItemDetailModel> ItemDetils { get; set; }

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

            var itemList = new List<ItemDetailModel>();

            foreach (var itemData in allItems)
            {
                var itemRecord = new ItemDetailModel();
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                foreach (var itemData in ItemDetils)
                {
                    var qty = itemData.Qty;

                    if (qty >= 0)
                    {
                        var existingItem = _context.ItemDetails.Where(e => e.IsDeleted == false && e.Sku == itemData.Sku).FirstOrDefault();

                        if (existingItem == null)
                        {
                            StatusMessage = "Error: Something went wrong!";
                            return Page();
                        }
                        else
                        {
                            if (qty != existingItem.Qty)
                            {
                                existingItem.Qty = qty;
                                _context.ItemDetails.Update(existingItem);
                                _context.SaveChanges();
                            }
                        }
                    }
                }

                _logger.LogInformation("Item Qty Updated");

                StatusMessage = "Item qty updated successfully";

                return RedirectToPage();

                //return LocalRedirect(returnUrl);
                //return RedirectToPage("./ViewInventory");
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
