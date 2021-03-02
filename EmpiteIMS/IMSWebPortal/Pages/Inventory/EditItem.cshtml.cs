using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IMSWebPortal.Pages.Inventory
{
    [Authorize(Roles = "Admin,Manager")]
    public class EditItemModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
