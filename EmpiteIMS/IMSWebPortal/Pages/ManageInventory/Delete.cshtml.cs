using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using IMSWebPortal.Data;
using IMSWebPortal.Data.Models.Inventory;

namespace IMSWebPortal.Pages.ManageInventory
{
    public class DeleteModel : PageModel
    {
        private readonly IMSWebPortal.Data.ApplicationDbContext _context;

        public DeleteModel(IMSWebPortal.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ItemDetail ItemDetail { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ItemDetail = await _context.ItemDetails.FirstOrDefaultAsync(m => m.Id == id);

            if (ItemDetail == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ItemDetail = await _context.ItemDetails.FindAsync(id);

            if (ItemDetail != null)
            {
                _context.ItemDetails.Remove(ItemDetail);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
