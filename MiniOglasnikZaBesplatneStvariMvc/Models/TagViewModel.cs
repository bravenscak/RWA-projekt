using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MiniOglasnikZaBesplatneStvariMvc.Models
{
    public class TagViewModel
    {
        public int Idtag { get; set; }

        [Display(Name = "Tag")]
        [Required(ErrorMessage = "Tag is required")]
        public string Name { get; set; } = null!;
    }
}
