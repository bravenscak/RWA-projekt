using MiniOglasnikZaBesplatneStvariLibrary.Models;
using System.ComponentModel.DataAnnotations;

namespace MiniOglasnikZaBesplatneStvariMvc.Models
{
    public class ItemTagViewModel
    {
        public int IditemTag { get; set; }

        [Display(Name = "Item")]
        [Required(ErrorMessage = "Item is required")]
        public int? ItemId { get; set; }

        [Display(Name = "Item")]
        public string? ItemName { get; set; }

        [Display(Name = "Tag")]
        [Required(ErrorMessage = "Tag is required")]
        public int? TagId { get; set; }

        [Display(Name = "Tag")]
        public string? TagName { get; set; }
    }
}
