using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace MiniOglasnikZaBesplatneStvariMvc.Models
{
    public class ItemViewModel
    {
        public int Iditem { get; set; }

        [Display(Name = "Item type")]
        [Required(ErrorMessage = "Item type is required")]
        public int? TypeId { get; set; }

        [Display(Name = "Item type")]
        public string? ItemTypeName { get; set; }

        [Required(ErrorMessage = "Item name is required")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Item description is required")]
        public string? Description { get; set; }

        [ValidateNever]
        public List<int>? TagIds { get; set; }

        [ValidateNever]
        public List<TagViewModel>? Tags { get; set; }

        [ValidateNever]
        public int Page { get; set; } = 1;

        [ValidateNever]
        public int Size { get; set; } = 10;

        [ValidateNever]
        public int FromPager { get; set; }

        [ValidateNever]
        public int ToPager { get; set; }

        [ValidateNever]
        public int LastPage { get; set; }
    }
}
