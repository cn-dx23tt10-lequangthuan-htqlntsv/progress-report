using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RoomManagement.Models;

namespace RoomManagement.Controllers
{
    [Authorize]
    public class TenantsController : Controller
    {
        private readonly RoomManagementContext _context;

        public TenantsController(RoomManagementContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string search, long? room)
        {
            ViewBag.Rooms = await _context.Rooms.ToListAsync();

            var query = _context.Tenants
                .Include(t => t.Contracts)
                .ThenInclude(c => c.Room)
                .AsQueryable();

            if(!string.IsNullOrEmpty(search))
            {
                query = query.Where(t => t.Name.Contains(search) || t.Phone.Contains(search) || t.Cccd.Contains(search));
            }

            if(room.HasValue && room.Value > 0)
            {
                query = query.Where(t => t.Contracts.Any(c => c.RoomId == room));
            }

            var tenants = await query.ToListAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_TenantTable", tenants);
            }    

            return View(tenants);
        }

        public async Task<IActionResult> Detail(long id)
        {
            var tenants = await _context.Tenants
                .Include(t => t.Contracts)
                .ThenInclude(c => c.Room)
                .FirstOrDefaultAsync(t => t.Id == id);

            return View(tenants);
        }

        public async Task<IActionResult> Create()
        {
            var listRoom = await _context.Rooms
                .Where(r => r.Status == "Trống")
                .Select(r => new { r.Id, r.RoomName })
                .ToListAsync();

            ViewBag.listRoom = new SelectList(listRoom, "Id", "RoomName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tenant tenant, long room, DateOnly start_day)
        {
            if (!ModelState.IsValid)
            {
                return View(tenant);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var RoomUpdate = await _context.Rooms.FindAsync(room);

                if (RoomUpdate == null || RoomUpdate.Status == "Đã thuê")
                {
                    ModelState.AddModelError("RoomUpdate", "Phòng không tồn tại hoặc đã có người thuê");

                    return RedirectToAction(nameof(Index));
                }

                _context.Tenants.Add(tenant);
                await _context.SaveChangesAsync();

                var contract = new Contract
                {
                    TenantId = tenant.Id,
                    RoomId = room,
                    StartDay = start_day,
                    Status = "Còn hạn"
                };

                _context.Contracts.Add(contract);

                RoomUpdate.Status = "Đã thuê";
                _context.Rooms.Update(RoomUpdate);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                TempData["Success"] = "Thêm người thuê thành công";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                TempData["Error"] = "Có lỗi xảy ra, vui lòng thử lại sau!";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Edit(long id)
        {
            var listRoom = await _context.Rooms
                .Where(r => r.Status == "Trống")
                .Select(r => new { r.Id, r.RoomName })
                .ToListAsync();

            ViewBag.listRoom = new SelectList(listRoom, "Id", "RoomName");

            var tenants = await _context.Tenants
                .Include(t => t.Contracts)
                .ThenInclude(c => c.Room)
                .FirstOrDefaultAsync(t => t.Id == id);

            return View(tenants);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Tenant tenant, long room, DateOnly start_day)
        {
            if (id != tenant.Id) return NotFound();
            if (!ModelState.IsValid) return View(tenant);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var currentTenant = await _context.Tenants
                    .Include(t => t.Contracts)
                    .ThenInclude(c => c.Room)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (currentTenant == null) return NotFound();

                currentTenant.Name = tenant.Name;
                currentTenant.Phone = tenant.Phone;
                currentTenant.Cccd = tenant.Cccd;
                currentTenant.Birthday = tenant.Birthday;
                currentTenant.Gender = tenant.Gender;
                currentTenant.Address = tenant.Address;

                var firstContract = currentTenant.Contracts?.FirstOrDefault();

                if (firstContract != null && firstContract.RoomId != room)
                {
                    var newRoom = await _context.Rooms.FindAsync(room);
                    if (newRoom == null || newRoom.Status == "Đã thuê")
                    {
                        TempData["Error"] = "Phòng mới không tồn tại hoặc đã có người thuê!";
                        return RedirectToAction(nameof(Edit), new { id = id });
                    }

                    if (firstContract.Room != null)
                    {
                        firstContract.Room.Status = "Trống";
                    }

                    firstContract.RoomId = room;
                    newRoom.Status = "Đã thuê";
                    firstContract.StartDay = start_day;
                }
                else if (firstContract != null)
                {
                    firstContract.StartDay = start_day;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = "Cập nhật thông tin thành công";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                TempData["Error"] = "Có lỗi xảy ra, vui lòng thử lại!";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            bool isStillRenting = await _context.Contracts
                .AnyAsync(c => c.TenantId == id && c.Status == "Còn hạn" && c.DeletedAt == null);

            if (isStillRenting)
            {
                TempData["Error"] = "Người thuê này đang có hợp đồng còn hạn. Hãy thanh lý hợp đồng trước!";
                return RedirectToAction(nameof(Index));
            }

            var tenant = await _context.Tenants.FindAsync(id);

            if (tenant == null) return NotFound();

            tenant.DeletedAt = DateTime.Now;

            _context.Tenants.Update(tenant);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Xóa người thuê thành công";
            return RedirectToAction(nameof(Index));
        }
    }
}
