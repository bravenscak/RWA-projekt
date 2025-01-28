using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniOglasnikZaBesplatneStvariLibrary.Models;
using MiniOglasnikZaBesplatneStvariMvc.Models;

namespace MiniOglasnikZaBesplatneStvariMvc.Controllers
{
    [Authorize]
    public class ReservationController : Controller
    {
        private readonly AdvertisementRwaContext _context;

        private readonly IConfiguration _configuration;

        public ReservationController(AdvertisementRwaContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public ActionResult Index(int page = 1, int size = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (size < 1) size = 10;

                var username = User.Identity.Name;
                var isAdmin = User.IsInRole("Admin");

                IEnumerable<Reservation> reservations;

                if (isAdmin)
                {
                    reservations = _context.Reservations
                        .Include(r => r.Item)
                        .Include(r => r.UserDetail)
                        .OrderByDescending(r => r.Idreservation);
                }
                else
                {
                    reservations = _context.Reservations
                        .Include(r => r.Item)
                        .Include(r => r.UserDetail)
                        .Where(r => r.UserDetail.Username == username)
                        .OrderByDescending(r => r.Idreservation);
                }

                var reservationsCount = reservations.Count();

                reservations = reservations
                    .Skip((page - 1) * size)
                    .Take(size);

                var reservationsViewModel = reservations.Select(r => new ReservationViewModel
                {
                    Idreservation = r.Idreservation,
                    ItemId = r.ItemId,
                    ItemName = r.Item.Name,
                    ReservationDate = r.ReservationDate,
                    Status = r.Status,
                    UserDetailId = r.UserDetailId,
                    Username = r.UserDetail.Username
                }).ToList();

                ViewBag.CurrentPage = page;
                ViewBag.PageSize = size;
                ViewBag.TotalCount = reservationsCount;
                ViewBag.TotalPage = (int)Math.Ceiling((double)reservationsCount / size);
                ViewBag.FromPager = page > 5 ? page - 5 : 1;
                ViewBag.ToPager = page + 5 > ViewBag.TotalPages ? ViewBag.TotalPages : page + 5;

                return View(reservationsViewModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        public ActionResult Details(int id)
        {
            var reservation = _context.Reservations
                .Include(r => r.Item)
                .Include(r => r.UserDetail)
                .FirstOrDefault(r => r.Idreservation == id);

            if (reservation == null)
            {
                return NotFound();
            }

            var reservationViewModel = new ReservationViewModel
            {
                Idreservation = reservation.Idreservation,
                ItemId = reservation.ItemId,
                ItemName = reservation.Item.Name,
                ReservationDate = reservation.ReservationDate,
                Status = reservation.Status,
                UserDetailId = reservation.UserDetailId,
                Username = reservation.UserDetail.Username
            };

            return View(reservationViewModel);
        }

        public ActionResult Create(int ItemId)
        {
            var item = _context.Items.Find(ItemId);
            if (item == null)
            {
                return NotFound();
            }

            var username = User.Identity.Name;
            var userDetail = _context.UserDetails.FirstOrDefault(u => u.Username == username);

            var reservationViewModel = new ReservationViewModel
            {
                ItemId = item.Iditem,
                ItemName = item.Name,
                Username = username,
                UserDetailId = userDetail.IdUserDetails,
                ReservationDate = DateTime.Now,
                Status = "Reserved"
            };

            return View(reservationViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ReservationViewModel reservationViewModel)
        {
            var item = _context.Items.Find(reservationViewModel.ItemId);
            if (item == null)
            {
                return NotFound();
            }

            var existingReservation = _context.Reservations
                .FirstOrDefault(r => r.ItemId == reservationViewModel.ItemId && r.Status == "Reserved");

            if (existingReservation != null)
            {
                ModelState.AddModelError("", "This item is already reserved.");
                return View(reservationViewModel);
            }

            var username = User.Identity.Name;
            var userDetail = _context.UserDetails.FirstOrDefault(u => u.Username == username);

            if (userDetail == null)
            {
                return Unauthorized();
            }

            reservationViewModel.Status = "Reserved";
            reservationViewModel.UserDetailId = userDetail.IdUserDetails;

            var reservation = new Reservation
            {
                ItemId = reservationViewModel.ItemId,
                ReservationDate = reservationViewModel.ReservationDate,
                Status = reservationViewModel.Status,
                UserDetailId = reservationViewModel.UserDetailId,
            };

            // Log ModelState errors
            foreach (var modelState in ViewData.ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            if (ModelState.IsValid)
            {
                _context.Reservations.Add(reservation);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(reservationViewModel);
        }

        public ActionResult Edit(int id)
        {
            var reservation = _context.Reservations.Include(r => r.Item).Include(r => r.UserDetail).FirstOrDefault(r => r.Idreservation == id);

            if (reservation == null)
            {
                return NotFound();
            }

            var reservationViewModel = new ReservationViewModel
            {
                Idreservation = reservation.Idreservation,
                ItemId = reservation.ItemId,
                ItemName = reservation.Item.Name,
                ReservationDate = reservation.ReservationDate,
                Status = reservation.Status,
                UserDetailId = reservation.UserDetailId,
                Username = reservation.UserDetail.Username
            };

            return View(reservationViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ReservationViewModel reservationViewModel)
        {
            var username = User.Identity.Name;
            var userDetail = _context.UserDetails.FirstOrDefault(u => u.Username == username);

            try
            {
                var reservation = _context.Reservations.Include(r => r.Item).FirstOrDefault(r => r.Idreservation == id);

                if (reservation == null)
                {
                    return NotFound();
                }

                reservation.ReservationDate = DateTime.Now;
                reservation.Status = reservationViewModel.Status;
                reservation.UserDetailId = userDetail.IdUserDetails;
                reservation.UserDetail = userDetail;

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "An error occurred while updating the reservation.");
                return View(reservationViewModel);
            }
        }

        public ActionResult Delete(int id)
        {
            var reservation = _context.Reservations
                .Include(r => r.Item)
                .Include(r => r.UserDetail)
                .FirstOrDefault(r => r.Idreservation == id);

            if (reservation == null)
            {
                return NotFound();
            }

            var reservationViewModel = new ReservationViewModel
            {
                Idreservation = reservation.Idreservation,
                ItemId = reservation.ItemId,
                ItemName = reservation.Item.Name,
                ReservationDate = reservation.ReservationDate,
                Status = reservation.Status,
                UserDetailId = reservation.UserDetailId,
                Username = reservation.UserDetail.Username
            };

            return View(reservationViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, ReservationViewModel reservationViewModel)
        {
            try
            {
                var reservation = _context.Reservations.Find(id);

                if (reservation == null)
                {
                    return NotFound();
                }

                _context.Reservations.Remove(reservation);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
