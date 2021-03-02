using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IMSWebPortal.Pages.User
{
    [Authorize(Roles = "Admin")]
    public class NewUserModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
