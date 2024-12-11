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
            catch (Exception ex)
            {
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

        [HttpPost("[action]")]
        public ActionResult<ItemDto> NewItem([FromBody] ItemDto itemDto)
        {
            try
            {
                if (itemDto == null)
                {
                    return BadRequest("There is no value.");
                }

                if (!ModelState.IsValid)
                {
                    _logService.Log("ERROR", "Model state is invalid.");
                    return BadRequest(ModelState);
                }

                if (string.IsNullOrWhiteSpace(itemDto.ItemTypeName))
                {
                    return BadRequest("Item type name is required.");
                }

                var trimmedItemTypeName = itemDto.ItemTypeName.Trim();

                var itemType = _context.ItemTypes.FirstOrDefault(x => x.Name.Equals(trimmedItemTypeName));

                if (itemType == null)
                {
                    itemType = new ItemType
                    {
                        Name = trimmedItemTypeName
                    };
                    _context.ItemTypes.Add(itemType);
                    _context.SaveChanges();
                }

                var trimmedItemName = itemDto.Name.Trim();

                var item = _context.Items.FirstOrDefault(x => x.Name.Equals(trimmedItemName));

                if (item != null)
                {
                    return BadRequest("Item name is required");
                }

                item = new Item
                {
                    Name = trimmedItemName,
                    Description = itemDto.Description,
                    TypeId = itemType.IditemType
                };

                _context.Items.Add(item);
                _context.SaveChanges();

                itemDto.Iditem = item.Iditem;

                _logService.Log("INFO", $"New item created with id = {item.Iditem}.");
                return Ok(itemDto);
            }
            catch (Exception ex)
            {
                _logService.Log("ERROR", $"Problem with creating new item: {ex.Message}");
                return BadRequest("An error occurred while creating a new item.");
            }
        }

        [HttpPut("[action]/{id}")]
        public ActionResult<ItemDto> UpdateItem(int id, [FromBody] ItemDto itemDto)
        {
            try
            {
                var existingItem = _context.Items.FirstOrDefault(x => x.Iditem == id);
                if (existingItem == null)
                {
                    _logService.Log("ERROR", $"Item where id = {id} not found.");
                    return NotFound("Item not found");
                }

                if (string.IsNullOrWhiteSpace(itemDto.ItemTypeName))
                {
                    return BadRequest("Item type name is required.");
                }

                var trimmedItemTypeName = itemDto.ItemTypeName.Trim();

                var itemType = _context.ItemTypes.FirstOrDefault(x => x.Name.Equals(trimmedItemTypeName));

                if (itemType == null)
                {
                    _logService.Log("ERROR", $"Item type where name = {trimmedItemTypeName} not found.");
                    return BadRequest("Item type not found");
                }

                var trimmedItemName = itemDto.Name.Trim();

                var item = _context.Items.FirstOrDefault(x => x.Name.Equals(trimmedItemName));

                if (item != null) 
                {
                    return BadRequest("Item name is required");
                }

                existingItem.Name = trimmedItemName;
                existingItem.Description = itemDto.Description;
                existingItem.TypeId = itemType.IditemType;

                _context.SaveChanges();

                var updatedItemDto = new ItemDto
                {
                    Iditem = existingItem.Iditem,
                    Name = existingItem.Name,
                    Description = existingItem.Description,
                    ItemTypeName = itemType.Name
                };

                _logService.Log("INFO", $"Item where id = {id} updated.");
                return Ok(updatedItemDto);
            }
            catch (Exception ex)
            {
                _logService.Log("ERROR", $"Problem with updating item where id = {id}: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("[action]/{id}")]
        public ActionResult<ItemDto> DeleteItem(int id)
        {
            try
            {
                var item = _context.Items.FirstOrDefault(x => x.Iditem == id);
                if (item == null)
                {
                    _logService.Log("ERROR", $"Item where id = {id} not found.");
                    return NotFound("Item not found");
                }

                _context.Items.Remove(item);
                _context.SaveChanges();
                _logService.Log("INFO", $"Item where id = {id} deleted.");
                return Ok();
            }
            catch (Exception ex)
            {
                _logService.Log("ERROR", $"Problem with deleting item where id = {id}: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
