using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using IMSWebPortal.Data;
using IMSWebPortal.Data.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace IMSWebPortal.Pages.Inventory
{
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
        public IList<ItemDetailsModel> ItemDetils { get; set; }

        public class ItemDetailsModel
        {
            [Display(Name = "Id")]
            public int Id { get; set; }

            [Display(Name = "Item Name")]
            public string Name { get; set; }

            [Display(Name = "SKU")]
            public string Sku { get; set; }

            [Display(Name = "Price")]
            public decimal Price { get; set; }

            [Display(Name = "Qty")]
            [Range(0, 10000000000, ErrorMessage = "Please enter a valid qty")]
            [RegularExpression("([0-9]+)", ErrorMessage = "Please enter a valid qty")]
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

        public async Task<IActionResult> OnGetSubmit(List<ItemDetailsModel> itemDetailsModel)
        {
            try
            {
                if(itemDetailsModel.Count > 0)
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
                }

                _logger.LogInformation("Item Qty Updated");

                StatusMessage = "Item qty updated successfully";

                return RedirectToPage();
            }
            catch(Exception ex)
            {
                StatusMessage = "Error: Something went wrong!";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsyncxxx()
        {
            if (ModelState.IsValid)
            {
                foreach(var itemData in ItemDetils)
                {
                    var qty = itemData.Qty;

                    if(qty >= 0)
                    {
                        var existingItem = _context.ItemDetails.Where(e => e.IsDeleted == false && e.Sku == itemData.Sku).FirstOrDefault();

                        if(existingItem == null)
                        {
                            StatusMessage = "Error: Something went wrong!";
                            return Page();
                        }
                        else
                        {
                            if(qty != existingItem.Qty)
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
