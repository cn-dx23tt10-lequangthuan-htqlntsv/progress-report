using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomManagement.Models;

namespace RoomManagement.Controllers
{
    [Authorize(Roles = "admin")]
    public class ServicesController : Controller
    {
        private readonly RoomManagementContext _context;

        public ServicesController(RoomManagementContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string search)
        {
            var query = _context.Services.AsQueryable();

            if(!string.IsNullOrEmpty(search))
            {
                query = query.Where(s => s.ServiceName.Contains(search));
            }

            var services = await query.ToListAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ServiceTable", services);
            }    

            return View(services);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Service service)
        {
            if(ModelState.IsValid)
            {
                _context.Services.Add(service);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Thêm dịch vụ thành công";
                return RedirectToAction(nameof(Index));
            }

            return View(service);
        }

        public async Task<IActionResult> Edit(long Id)
        {
            var service = await _context.Services.FindAsync(Id);

            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Service service)
        {
            if (id != service.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(service);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Cập nhật dịch vụ thành công";
                return RedirectToAction(nameof(Index));
            }

            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var service = await _context.Services.FindAsync(id);

            if (service == null) return NotFound();

            service.DeletedAt = DateTime.Now;

            _context.Services.Update(service);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Xóa dịch vụ thành công";

            return RedirectToAction(nameof(Index));
        }
    }
}
