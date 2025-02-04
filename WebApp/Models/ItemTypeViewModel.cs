using System.ComponentModel.DataAnnotations;

namespace MiniOglasnikZaBesplatneStvariMvc.Models
{
    public class ItemTypeViewModel
    {
        public int IditemType { get; set; }

        [Display(Name = "Item type")]
        [Required(ErrorMessage = "Item type is required")]
        public string? Name { get; set; }
    }
}
