using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiniOglasnikZaBesplatneStvariLibrary.Models;
using MiniOglasnikZaBesplatneStvariMvc.Models;
using NuGet.Packaging;
using NuGet.Protocol;

namespace MiniOglasnikZaBesplatneStvariMvc.Controllers
{
    public class ItemController : Controller
    {
        private readonly AdvertisementRwaContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public ItemController(AdvertisementRwaContext context, IConfiguration configuration, IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
        }

        private List<SelectListItem> GetTypeListItems()
        {
            var typeListItemsJson = HttpContext.Session.GetString("TypeListItems");

            List<SelectListItem> typeListItems;
            if (typeListItemsJson == null)
            {
                typeListItems = _context.ItemTypes.Select(t => new SelectListItem
                {
                    Text = t.Name,
                    Value = t.IditemType.ToString()
                }).ToList();

                HttpContext.Session.SetString("TypeListItems", typeListItems.ToJson());
            }
            else
            {
                typeListItems = typeListItemsJson.FromJson<List<SelectListItem>>();
            }

            return typeListItems;
        }

        public ActionResult Index(int page = 1, int size = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (size < 1) size = 10;

                IEnumerable<Item> items = _context.Items
                    .Include(i => i.Type)
                    .OrderByDescending(i => i.Iditem);

                var total = items.Count();

                items = items.Skip((page - 1) * size).Take(size);

                var itemsViewModels = items.Select(i => new ItemViewModel
                {
                    Iditem = i.Iditem,
                    TypeId = i.TypeId,
                    ItemTypeName = i.Type.Name,
                    Name = i.Name,
                    Description = i.Description
                }).ToList();

                ViewBag.Page = page;
                ViewBag.Size = size;
                ViewBag.TotalCount = total;
                ViewBag.TotalPages = (int)Math.Ceiling((double)total / size);
                ViewBag.FromPager = page > 5 ? page - 5 : 1;
                ViewBag.ToPager = page + 5 > ViewBag.TotalPages ? ViewBag.TotalPages : page + 5;

                return View(itemsViewModels);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        public ActionResult Search(SearchViewModel searchViewModel)
        {
            try
            {
                if (string.IsNullOrEmpty(searchViewModel.Q) && string.IsNullOrEmpty(searchViewModel.Submit))
                {
                    searchViewModel.Q = Request.Cookies["query"];
                }

                Response.Cookies.Append("query", searchViewModel.Q ?? "");

                IQueryable<Item> items = _context.Items
                    .Include(i => i.Type)
                    .OrderByDescending(i => i.Iditem);

                if (!string.IsNullOrEmpty(searchViewModel.Q))
                {
                    items = items.Where(i => i.Name.Contains(searchViewModel.Q) || i.Type.Name.Contains(searchViewModel.Q));
                }

                var total = items.Count();

                if (!string.IsNullOrEmpty(searchViewModel.OrderBy))
                {
                    switch (searchViewModel.OrderBy.ToLower())
                    {
                        case "iditem":
                            items = items.OrderBy(i => i.Iditem);
                            break;
                        case "iditem_desc":
                            items = items.OrderByDescending(i => i.Iditem);
                            break;
                        case "name":
                            items = items.OrderBy(i => i.Name);
                            break;
                        case "name_desc":
                            items = items.OrderByDescending(i => i.Name);
                            break;
                        case "type":
                            items = items.OrderBy(i => i.Type.Name);
                            break;
                        case "type_desc":
                            items = items.OrderByDescending(i => i.Type.Name);
                            break;
                        default:
                            items = items.OrderBy(i => i.Iditem);
                            break;
                    }
                }

                items = items.Skip((searchViewModel.Page - 1) * searchViewModel.Size).Take(searchViewModel.Size);

                searchViewModel.Items = items.Select(i => new ItemViewModel
                {
                    Iditem = i.Iditem,
                    TypeId = i.TypeId,
                    ItemTypeName = i.Type.Name,
                    Name = i.Name,
                    Description = i.Description
                }).ToList();

                var expandPages = _configuration.GetValue<int>("Paging:ExpandPages");
                searchViewModel.LastPage = (int)Math.Ceiling((double)total / searchViewModel.Size);
                searchViewModel.FromPager = searchViewModel.Page > expandPages ? searchViewModel.Page - expandPages : 1;
                searchViewModel.ToPager = searchViewModel.Page + expandPages > searchViewModel.LastPage ? searchViewModel.LastPage : searchViewModel.Page + expandPages;

                return View(searchViewModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult Details(int id)
        {
            try
            {
                var item = _context.Items
                    .Include(i => i.Type)
                    .Include(i => i.ItemTags).ThenInclude(it => it.Tag)
                    .FirstOrDefault(i => i.Iditem == id);

                if (item == null)
                {
                    return NotFound();
                }

                var allTags = _context.Tags.ToList();

                var itemViewModel = new ItemViewModel
                {
                    Iditem = item.Iditem,
                    TypeId = item.TypeId,
                    ItemTypeName = item.Type.Name,
                    Name = item.Name,
                    Description = item.Description,

                    TagIds = item.ItemTags
                        .Select(it => it.TagId)
                        .Where(id => id.HasValue)
                        .Select(id => id.Value)
                        .ToList(),

                    Tags = allTags.Select(t => new TagViewModel
                    {
                        Idtag = t.Idtag,
                        Name = t.Name
                    }).ToList()
                };

                return View(itemViewModel);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.ItemTypeDdlItems = GetTypeListItems();

            var item = new ItemViewModel();

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(ItemViewModel itemViewModel)
        {
            var trimmedName = itemViewModel.Name.Trim();
            if (_context.Items.Any(i => i.Name == trimmedName))
            {
                ModelState.AddModelError("", "Item with the same name already exists.");
                return View();
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.ItemTypeDdlItems = GetTypeListItems();

                    ModelState.AddModelError("", "Failed to create item");

                    return View(itemViewModel);
                }

                var item = new Item
                {
                    Iditem = itemViewModel.Iditem,
                    TypeId = itemViewModel.TypeId,
                    Name = itemViewModel.Name,
                    Description = itemViewModel.Description
                };

                _context.Items.Add(item);
                _context.SaveChanges();

                HttpContext.Session.Remove("TypeListItems");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the item: " + ex.Message);
                ViewBag.PropertyTypeDdlItems = GetTypeListItems();
                return View(itemViewModel);
            }
        }

        private IQueryable<SelectListItem> GetItemTypes()
        {
            return _context.ItemTypes.Select(t => new SelectListItem
            {
                Text = t.Name,
                Value = t.IditemType.ToString()
            });
        }

        private IQueryable<SelectListItem> GetUserDetails()
        {
            return _context.UserDetails.Select(u => new SelectListItem
            {
                Text = u.Username,
                Value = u.IdUserDetails.ToString()
            });
        }

        private List<SelectListItem> GetTags()
        {
            return _context.Tags.Select(t => new SelectListItem
            {
                Text = t.Name,
                Value = t.Idtag.ToString()
            }).ToList();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            ViewBag.ItemTypeDdlItems = GetItemTypes();

            ViewBag.TagDdlItems = GetTags();

            var item = _context.Items
                .Include(i => i.ItemTags)
                .FirstOrDefault(i => i.Iditem == id);

            var itemViewModel = new ItemViewModel
            {
                Iditem = item.Iditem,
                TypeId = item.TypeId,
                Name = item.Name,
                Description = item.Description,
                TagIds = item.ItemTags
                        .Select(it => it.TagId)
                        .Where(id => id.HasValue)
                        .Select(id => id.Value)
                        .ToList()
            };

            return View(itemViewModel);     
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id, ItemViewModel itemViewModel)
        {
            try
            {
                var item = _context.Items.Include(i => i.ItemTags).FirstOrDefault(i => i.Iditem == id);
                item.TypeId = itemViewModel.TypeId;
                item.Name = itemViewModel.Name;
                item.Description = itemViewModel.Description;

                _context.RemoveRange(item.ItemTags);
                var itemTags = itemViewModel.TagIds.Select(tagId => new ItemTag
                {
                    ItemId = id,
                    TagId = tagId
                });
                item.ItemTags.AddRange(itemTags);

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var item = _context.Items
                .Include(i => i.Type)
                .FirstOrDefault(i => i.Iditem == id);

            var itemViewModel = new ItemViewModel
            {
                Iditem = item.Iditem,
                TypeId = item.TypeId,
                Name = item.Name,
                Description = item.Description
            };

            return View(itemViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id, ItemViewModel itemViewModel)
        {
            try
            {
                var item = _context.Items.FirstOrDefault(i => i.Iditem == id);

                _context.Items.Remove(item);
                _context.SaveChanges();

                return RedirectToAction("Index", "Item");
            }
            catch
            {
                return View();
            }
        }
    }
}
