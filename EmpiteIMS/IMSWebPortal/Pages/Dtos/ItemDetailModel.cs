using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IMSWebPortal.Pages.Dtos
{
    public class ItemDetailModel
    {
        [Required]
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Item Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "SKU")]
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

        [Display(Name = "Is Deleted")]
        public bool IsDeleted { get; set; }
    }
}
