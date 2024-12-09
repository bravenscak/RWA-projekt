using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using MiniOglasnikZaBesplatneStvari.Dtos;
using MiniOglasnikZaBesplatneStvari.Models;

namespace MiniOglasnikZaBesplatneStvari.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly AdvertisementRwaContext _context;
        private readonly ILogService _logService;

        public ItemController(AdvertisementRwaContext context, ILogService logService)
        {
            _context = context;
            _logService = logService;
        }

        [HttpGet("[action]")]
        public ActionResult<ItemDto> GetAllItems()
        {
            try
            {
                var result = _context.Items;
                var mappedResult = result.Select(x =>
                    new ItemDto
                    {
                        Iditem = x.Iditem,
                        Name = x.Name,
                        Description = x.Description,
                        ItemTypeName = x.Type.Name
                    });
                _logService.Log("INFO", "Successfully retrieved all items");
                return Ok(mappedResult);
            }
            catch (Exception ex)
            {
                _logService.Log("ERROR", $"Error retrieving items: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("[action]/{id}")]
        public ActionResult<ItemDto> GetItemById(int id)
        {
            try
            {
                var result = _context.Items
                    .Include(x => x.Type)
                    .Include(x => x.ItemTags)
                    .FirstOrDefault(x => x.Iditem == id);
                if (result == null)
                {
                    _logService.Log("ERROR", $"Item where id = {id} not found.");
                    return NotFound("Item not found");
                }
                var mappedResult = new ItemDto
                {
                    Iditem = result.Iditem,
                    Name = result.Name,
                    Description = result.Description,
                    ItemTypeName = result.Type.Name
                };
                _logService.Log("INFO", $"Successfully retrieved item where id = {id}.");
                return Ok(mappedResult);
            }
            catch (Exception ex) {
                _logService.Log("ERROR", $"Error retrieving item where id = {id}: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("[action]")]
        public ActionResult<IEnumerable<Item>> Search(string text, string sortType, int page = 1, int count = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (count < 10) count = 10;

                IEnumerable<Item> results = _context.Items.Include(x => x.Type);

                if (!String.IsNullOrEmpty(text))
                {
                    results = results.Where(x => x.Name.Contains(text, StringComparison.OrdinalIgnoreCase));
                }

                switch (sortType)
                {
                    case "Id":
                        results = results.OrderBy(x => x.Iditem);
                        break;
                    case "Name":
                        results = results.OrderBy(x => x.Name);
                        break;
                    default:
                        _logService.Log("WARN", $"Invalid sortType '{sortType}' provided.");
                        break;
                }

                results = results.Skip((page - 1) * count).Take(count);

                _logService.Log("INFO", $"Search completed for text '{text}' with sort '{sortType}'.");

                return Ok(results);
            }
            catch (Exception ex)
            {
                _logService.Log("ERROR", $"Error occurred during search: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, "An internal server error occurred.");
            }
        }
    }
}