using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MiniOglasnikZaBesplatneStvariMvc.Models
{
    public class SearchViewModel
    {
        public string Q { get; set; }
        public string OrderBy { get; set; }
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
        public int FromPager { get; set; }
        public int ToPager { get; set; }
        public int LastPage { get; set; }
        public IEnumerable<ItemViewModel> Items{ get; set; }
        public string Submit { get; set; }
    }
}
