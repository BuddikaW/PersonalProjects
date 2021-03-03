using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using IMSWebPortal.Data;
using IMSWebPortal.Data.Models.Inventory;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using IMSWebPortal.Data.Models.Identity;
using IMSWebPortal.Pages.Dtos;
using Microsoft.AspNetCore.Authorization;
using IMSWebPortal.Pages.Dtos.DtoMapping;

namespace IMSWebPortal.Pages.ManageInventory
{
    [Authorize(Roles = "Admin,Manager,Viewer")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ApplicationDbContext context, UserManager<AppUser> userManager, ILogger<IndexModel> logger)
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
            ItemDetils = new ItemDtoMap().Map(allItems);
            return Page();
        }

        public IActionResult OnPost(int? id)
        {
            try
            {
                if (id == null)
                {
                    StatusMessage = "Error: Something went wrong!";
                    return new JsonResult(true);
                }

                var item = _context.ItemDetails.Where(e => e.Id == id).FirstOrDefault();
                if (item == null)
                {
                    StatusMessage = "Error: Something went wrong!";
                    return new JsonResult(true);
                }

                item.IsDeleted = true;
                _context.ItemDetails.Update(item);
                _context.SaveChanges();

                StatusMessage = "Item deleted successfully";
                //return RedirectToPage();
                return new JsonResult(true);
            }
            catch (Exception ex)
            {
                StatusMessage = "Error: Something went wrong!";
                return new JsonResult(true);
            }
        }
    }
}
