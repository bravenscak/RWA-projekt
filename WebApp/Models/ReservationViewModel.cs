using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace MiniOglasnikZaBesplatneStvariMvc.Models
{
    public class ReservationViewModel
    {
        public int Idreservation { get; set; }

        [Display(Name = "Item")]
        [Required(ErrorMessage = "Item is required")]
        public int? ItemId { get; set; }

        [Display(Name = "Item")]
        public string? ItemName { get; set; }

        [Display(Name = "User")]
        [ValidateNever]
        [Required(ErrorMessage = "User is required")]
        public int? UserDetailId { get; set; }

        [Display(Name = "User")]
        public string? Username { get; set; }

        [Display(Name = "Reservation Date")]
        [Required(ErrorMessage = "Reservation date is required")]

        public DateTime ReservationDate { get; set; }

        [Display(Name = "Status")]
        public string? Status { get; set; }

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
