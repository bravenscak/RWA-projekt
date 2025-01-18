using System.ComponentModel.DataAnnotations;

namespace MiniOglasnikZaBesplatneStvari.Dtos
{
    public class ItemDto
    {
        public int Iditem { get; set; }

        [Required(ErrorMessage = "Enter something in here")]

        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Enter something in here")]

        public string? Description { get; set; }

        [Required(ErrorMessage = "Enter something in here")]

        public string ItemTypeName  { get; set; }
    }
}
