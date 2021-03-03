using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using IMSWebPortal.Areas;
using IMSWebPortal.Data;
using IMSWebPortal.Data.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace IMSWebPortal.Pages.ManageInventory
{
    [Authorize(Roles = "Admin,Manager")]
    public class SendReportModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SendReportModel> _logger;

        public SendReportModel(ApplicationDbContext context, UserManager<AppUser> userManager, ILogger<SendReportModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public string ReturnUrl { get; set; }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public EmailModel Input { get; set; }

        public class EmailModel
        {
            [Required]
            [Display(Name = "Recipient List")]
            public string RecipientList { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (Input.RecipientList != "")
            {
                var emailList = Input.RecipientList.Split(',');

                var subject = "EmpiteIMS - Inventory Details";
                var body = GenerateHtml();

                foreach (var email in emailList)
                {
                    var result = new EmailClient(_context).SendEmail(body, email, subject);
                }
            }
            return Page();
        }

        public string GenerateHtml()
        {
            var htmlString = "";
            try
            {
                var itemList = _context.ItemDetails.Where(e => e.IsDeleted == false);

                htmlString += "<html>";

                #region Styles

                htmlString += "<head>";
                htmlString += "<style>";
                htmlString += "#MainTable {";
                htmlString += "width: 500px;";
                htmlString += "font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, 'Noto Sans', sans-serif, 'Apple Color Emoji', 'Segoe UI Emoji', 'Segoe UI Symbol', 'Noto Color Emoji';";
                htmlString += "}";

                htmlString += "#SecondaryTable {";
                htmlString += "width: 100%;";
                htmlString += "}";

                htmlString += "#HeaderDetails {";
                htmlString += "width: 100%;";
                htmlString += "}";

                htmlString += ".oktable {";
                htmlString += "width: 80%;";
                htmlString += "margin-top: 20px;";
                htmlString += "margin-bottom: 20px;";
                htmlString += "border-collapse: collapse;";
                htmlString += "}";

                htmlString += ".oktable>tbody>tr>td:first-child {";
                htmlString += "text-align: left;";
                htmlString += "padding-left: 10px !important;";
                htmlString += "}";

                htmlString += ".oktable>tbody>tr>td:last-child {";
                htmlString += "text-align: right;";
                htmlString += "padding-right: 10px !important;";
                htmlString += "}";

                htmlString += ".oktable>thead>tr>th {";
                htmlString += "border-top: 1px solid;";
                htmlString += "border-bottom: 1px solid;";
                htmlString += "border-color: darkgray;";
                htmlString += "background-color: #f1a739;";
                htmlString += "padding: 5px;";
                htmlString += "}";

                htmlString += ".oktable>tbody>tr>td {";
                htmlString += "border-top: 1px solid;";
                htmlString += "border-bottom: 1px solid;";
                htmlString += "border-color: #e4e2de;";
                htmlString += "padding: 5px;";
                htmlString += "}";

                htmlString += ".title {";
                htmlString += "white-space: normal;";
                htmlString += "float: left;";
                htmlString += "word-break: break-all;";
                htmlString += "color: #f1a739 !important;";
                htmlString += "font-size: 25px;";
                htmlString += "font-weight: bold;";
                htmlString += "}";
                htmlString += "</style>";
                htmlString += "</head>";

                #endregion

                #region Body

                htmlString += "<body>";
                htmlString += "<table id='MainTable'>";
                htmlString += "<tbody>";
                htmlString += "<tr>";
                htmlString += "<td style='width:10%'></td>";
                htmlString += "<td align='center'>";

                htmlString += "<table id='SecondaryTable'>";
                htmlString += "<tbody>";
                htmlString += "<tr>";
                htmlString += "<td style='text-align: -webkit-center;'>";

                #region Header Details

                htmlString += "<table id='HeaderDetails'>";
                htmlString += "<tbody>";
                htmlString += "<tr>";
                htmlString += "<td><span class='title'>Empite IMS</span></td>";
                htmlString += "<td>";
                htmlString += "<img style='float:right' src='https://www.empite.com/assets/img/site/102%20x%2040.png' />";
                htmlString += "</td>";
                htmlString += "</tr>";
                htmlString += "<tr>";
                htmlString += "<td colspan='2'>";
                htmlString += "Please refer the inventory details below";
                htmlString += "</td>";
                htmlString += "</tr>";
                htmlString += "</tbody>";
                htmlString += "</table>";

                #endregion

                htmlString += "</td>";
                htmlString += "</tr>";
                htmlString += "<tr>";
                htmlString += "<td style='text-align: -webkit-center;'>";

                if(itemList.Count() > 0)
                {
                    #region Item Details

                    htmlString += "<table id='ItemDetails' class='oktable'>";
                    htmlString += "<thead>";
                    htmlString += "<tr>";
                    htmlString += "<th>Item Name</th>";
                    htmlString += "<th>Available Qty</th>";
                    htmlString += "</tr>";
                    htmlString += "</thead>";
                    htmlString += "<tbody>";

                    #region Each item

                    foreach (var item in itemList)
                    {
                        htmlString += "<tr>";
                        htmlString += "<td>" + item.Name + "</td>";
                        htmlString += "<td>" + item.Qty + "</td>";
                        htmlString += "</tr>";
                    }

                    #endregion

                    htmlString += "</tbody>";
                    htmlString += "</table>";

                    #endregion
                }
                else
                {
                    htmlString += "<span>";
                    htmlString += "Inventory is emptry at the moment";
                    htmlString += "</span>";
                }

                htmlString += "</td>";
                htmlString += "</tr>";
                htmlString += "</tbody>";
                htmlString += "</table>";

                htmlString += "</td>";
                htmlString += "<td style='width:10%'></td>";
                htmlString += "</tr>";
                htmlString += "</tbody>";
                htmlString += "</table>";
                htmlString += "</body>";

                #endregion

                htmlString += "</html>";

                return htmlString;
            }
            catch(Exception ex)
            {
                return htmlString;
            }
        }
    }
}
