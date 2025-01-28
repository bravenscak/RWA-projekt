using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiniOglasnikZaBesplatneStvariLibrary.Models;
using MiniOglasnikZaBesplatneStvariMvc.Models;
using NuGet.Protocol;

namespace MiniOglasnikZaBesplatneStvariMvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ItemTagController : Controller
    {
        private readonly AdvertisementRwaContext _context;

        public ItemTagController(AdvertisementRwaContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            try
            {
                var itemTagViewModel = _context.ItemTags
                    .Include(it => it.Item)
                    .Include(it => it.Tag)
                    .Select(it => new ItemTagViewModel
                    {
                        IditemTag = it.IditemTag,
                        ItemId = it.ItemId,
                        TagId = it.TagId,
                        ItemName = it.Item.Name,
                        TagName = it.Tag.Name
                    }).ToList();

                return View(itemTagViewModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IActionResult Details(int id)
        {
            try
            {
                var itemTag = _context.ItemTags
                    .Include(it => it.Item)
                    .Include(it => it.Tag)
                    .FirstOrDefault(it => it.IditemTag == id);

                if (itemTag == null)
                {
                    return NotFound();
                }

                var itemTagViewModel = new ItemTagViewModel
                {
                    IditemTag = itemTag.IditemTag,
                    ItemId = itemTag.ItemId,
                    TagId = itemTag.TagId,
                    ItemName = itemTag.Item.Name,
                    TagName = itemTag.Tag.Name
                };

                return View(itemTagViewModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private IQueryable<SelectListItem> GetItem()
        {
            return _context.Items.Select(i => new SelectListItem
            {
                Value = i.Iditem.ToString(),
                Text = i.Name
            });
        }

        private IQueryable<SelectListItem> GetTag()
        {
            return _context.Tags.Select(t => new SelectListItem
            {
                Value = t.Idtag.ToString(),
                Text = t.Name
            });
        }

        private List<SelectListItem> GetItemList()
        {
            var itemListItemsJson = HttpContext.Session.GetString("ItemListItems");

            List<SelectListItem> itemListItems;
            if (itemListItemsJson == null)
            {
                itemListItems = _context.Items.Select(i => new SelectListItem
                {
                    Value = i.Iditem.ToString(),
                    Text = i.Name
                }).ToList();

                HttpContext.Session.SetString("ItemListItems", itemListItems.ToJson());
            }
            else
            {
                itemListItems = itemListItemsJson.FromJson<List<SelectListItem>>();
            }

            return itemListItems;
        }

        private List<SelectListItem> GetTagListItems()
        {
            var tagListItemsJson = HttpContext.Session.GetString("TagListItems");

            List<SelectListItem> tagListItems;
            if (tagListItemsJson == null)
            {
                tagListItems = _context.Tags.Select(t => new SelectListItem
                {
                    Value = t.Idtag.ToString(),
                    Text = t.Name
                }).ToList();

                HttpContext.Session.SetString("TagListItems", tagListItems.ToJson());
            }
            else
            {
                tagListItems = tagListItemsJson.FromJson<List<SelectListItem>>();
            }

            return tagListItems;
        }

        public ActionResult Create()
        {
            ViewBag.ItemDdlItems = GetItemList();
            ViewBag.TagDdlItems = GetTagListItems();

            var itemTagViewModel = new ItemTagViewModel();

            return View(itemTagViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ItemTagViewModel itemTagViewModel)
        {
            if(!ModelState.IsValid)
            {
                ViewBag.ItemDdlItems = GetItemList();
                ViewBag.TagDdlItems = GetTagListItems();
                ModelState.AddModelError("", "Failed");
                return View();
            }

            if(_context.ItemTags.Any(it => it.ItemId == itemTagViewModel.ItemId && it.TagId == itemTagViewModel.TagId))
            {
                ModelState.AddModelError("", "This tag is already assigned to the item");
                ViewBag.ItemDdlItems = GetItemList();
                ViewBag.TagDdlItems = GetTagListItems();
                return View(itemTagViewModel);
            }

            try
            {
                var itemTag = new ItemTag
                {
                    ItemId = itemTagViewModel.ItemId,
                    TagId = itemTagViewModel.TagId
                };

                _context.ItemTags.Add(itemTag);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred" + ex.Message);
                ViewBag.ItemDdlItems = GetItemList();
                ViewBag.TagDdlItems = GetTagListItems();
                return View(itemTagViewModel);
            }
        }

        public ActionResult Edit(int id)
        {
            ViewBag.ItemDdlItems = GetItemList();
            ViewBag.TagDdlItems = GetTagListItems();

            var itemTag = _context.ItemTags
                    .Include(it => it.Item)
                    .Include(it => it.Tag)
                    .FirstOrDefault(it => it.IditemTag == id);

            if (itemTag == null)
            {
                return NotFound();
            }

            var itemTagViewModel = new ItemTagViewModel
            {
                IditemTag = itemTag.IditemTag,
                ItemId = itemTag.ItemId,
                TagId = itemTag.TagId
            };

            return View(itemTagViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ItemTagViewModel itemTagViewModel)
        {
            try
            {
                var itemTag = _context.ItemTags
                    .Include(it => it.Item)
                    .Include(it => it.Tag)
                    .FirstOrDefault(it => it.IditemTag == id);

                itemTag.ItemId = itemTagViewModel.ItemId;
                itemTag.TagId = itemTagViewModel.TagId;

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch 
            {
                return View();
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                var itemTag = _context.ItemTags
                        .Include(it => it.Item)
                        .Include(it => it.Tag)
                        .FirstOrDefault(it => it.IditemTag == id);

                if (itemTag == null)
                {
                    return NotFound();
                }

                var itemTagViewModel = new ItemTagViewModel
                {
                    IditemTag = itemTag.IditemTag,
                    ItemId = itemTag.ItemId,
                    TagId = itemTag.TagId,
                    ItemName = itemTag.Item.Name,
                    TagName = itemTag.Tag.Name
                };

                return View(itemTagViewModel);
            }
            catch 
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, ItemTagViewModel itemTagViewModel)
        {
            try
            {
                var itemTag = _context.ItemTags.Find(id);

                if (itemTag != null)
                {
                    _context.ItemTags.Remove(itemTag);
                    _context.SaveChanges();
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
