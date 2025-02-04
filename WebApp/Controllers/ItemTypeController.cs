using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniOglasnikZaBesplatneStvariLibrary.Models;
using MiniOglasnikZaBesplatneStvariMvc.Models;
using NuGet.Protocol;

namespace MiniOglasnikZaBesplatneStvariMvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ItemTypeController : Controller
    {
        private readonly AdvertisementRwaContext _context;

        private readonly IMapper _mapper;

        public ItemTypeController(AdvertisementRwaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public ActionResult Index()
        {
            try
            {
                ItemTypeViewModel itemTypeViewModel = null;
                if (TempData.ContainsKey("newItemType"))
                {
                    itemTypeViewModel = ((string)TempData["newItemType"]).FromJson<ItemTypeViewModel>();
                }

                var itemTypes = _context.ItemTypes;
                var itemTypesViewModel = _mapper.Map<IEnumerable<ItemTypeViewModel>>(itemTypes);

                return View(itemTypesViewModel);
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
                var itemType = _context.ItemTypes.FirstOrDefault(i => i.IditemType == id);
                var itemTypeViewModel = new ItemTypeViewModel
                {
                    IditemType = itemType.IditemType,
                    Name = itemType.Name
                };

                return View(itemTypeViewModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ItemTypeViewModel itemTypeViewModel)
        {
            var trimmedName = itemTypeViewModel.Name.Trim();
            if (_context.ItemTypes.Any(i => i.Name.Equals(trimmedName)))
            {
                ModelState.AddModelError("", "This type alredy exists");
                return View();
            }

            try
            {
                var itemType = new ItemType
                {
                    Name = itemTypeViewModel.Name
                };

                _context.ItemTypes.Add(itemType);
                _context.SaveChanges();

                TempData["newItemType"] = itemTypeViewModel.ToJson();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Edit(int id)
        {
            try
            {
                var itemType = _context.ItemTypes.FirstOrDefault(i => i.IditemType == id);
                var itemTypeViewModel = new ItemTypeViewModel
                {
                    IditemType = itemType.IditemType,
                    Name = itemType.Name
                };

                return View(itemTypeViewModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ItemTypeViewModel itemTypeViewModel)
        {
            try
            {
                var itemType = _context.ItemTypes.FirstOrDefault(i => i.IditemType == id);
                itemType.Name = itemTypeViewModel.Name;

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                var itemType = _context.ItemTypes.FirstOrDefault(i => i.IditemType == id);
                var itemTypeViewModel = new ItemTypeViewModel
                {
                    IditemType = itemType.IditemType,
                    Name = itemType.Name
                };

                return View(itemTypeViewModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, ItemTypeViewModel itemTypeViewModel)
        {
            try
            {
                var itemType = _context.ItemTypes.FirstOrDefault(i => i.IditemType == id);

                _context.ItemTypes.Remove(itemType);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
