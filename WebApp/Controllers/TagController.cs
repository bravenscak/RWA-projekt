using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniOglasnikZaBesplatneStvariLibrary.Models;
using MiniOglasnikZaBesplatneStvariMvc.Models;
using NuGet.Protocol;

namespace MiniOglasnikZaBesplatneStvariMvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TagController : Controller
    {
        private readonly AdvertisementRwaContext _context;

        private readonly IMapper _mapper;

        public TagController(AdvertisementRwaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public ActionResult Index()
        {
            try
            {
                TagViewModel tagViewModel = null;
                if(TempData.ContainsKey("newTag"))
                {
                    tagViewModel = ((string)TempData["newTag"]).FromJson<TagViewModel>();
                }

                var tagViewModels = _context.Tags.Select(t => new TagViewModel
                {
                    Idtag = t.Idtag,
                    Name = t.Name
                }).ToList();

                var tags = _context.Tags;
                var tagsViewModel = _mapper.Map<IEnumerable<TagViewModel>>(tags);

                return View(tagsViewModel);
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
                var tag = _context.Tags.FirstOrDefault(tag => tag.Idtag == id);
                var tagViewModel = new TagViewModel
                {
                    Idtag = tag.Idtag,
                    Name = tag.Name
                };

                return View(tagViewModel);
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
        public ActionResult Create(TagViewModel tagViewModel)
        {
            try
            {
                var tag = new Tag
                {
                    Name = tagViewModel.Name
                };

                _context.Tags.Add(tag);
                _context.SaveChanges();

                TempData["newTag"] = tagViewModel.ToJson();

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
                var tag = _context.Tags.FirstOrDefault(tag => tag.Idtag == id);
                var tagViewModel = new TagViewModel
                {
                    Idtag = tag.Idtag,
                    Name = tag.Name
                };

                return View(tagViewModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, TagViewModel tagViewModel)
        {
            try
            {
                var tag = _context.Tags.FirstOrDefault(tag => tag.Idtag == id);
                tag.Name = tagViewModel.Name;

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
                var tag = _context.Tags.FirstOrDefault(tag => tag.Idtag == id);
                var tagViewModel = new TagViewModel
                {
                    Idtag = tag.Idtag,
                    Name = tag.Name
                };

                return View(tagViewModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, TagViewModel tagViewModel)
        {
            try
            {
                var tag = _context.Tags.FirstOrDefault(tag => tag.Idtag == id);

                _context.Tags.Remove(tag);
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
