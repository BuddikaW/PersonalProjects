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
using IMSWebPortal.Data.Models.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using IMSWebPortal.Pages.Dtos;

namespace IMSWebPortal.Pages.ManageInventory
{
    [Authorize(Roles = "Admin,Manager")]
    public class DetailsModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DetailsModel> _logger;

        public DetailsModel(ApplicationDbContext context, UserManager<AppUser> userManager, ILogger<DetailsModel> logger)
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

            var itemModel = new ItemDetailModel();
            itemModel.Id = item.Id;
            itemModel.Name = item.Name;
            itemModel.Sku = item.Sku;
            itemModel.Price = item.Price;
            itemModel.Qty = item.Qty;
            itemModel.PrvSku = item.Sku;

            Input = itemModel;
            return Page();
        }
    }
}
