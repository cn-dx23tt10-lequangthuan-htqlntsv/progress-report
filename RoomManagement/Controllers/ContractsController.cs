using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RoomManagement.Models;

namespace RoomManagement.Controllers
{
    [Authorize]
    public class ContractsController : Controller
    {
        private readonly RoomManagementContext _context;

        public ContractsController(RoomManagementContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string search, string status)
        {
            var query = _context.Contracts
                .Include(c => c.Room)
                .Include(c => c.Tenant)
                .AsQueryable();

            if(!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Id.ToString().Contains(search) || c.Tenant.Name == search || c.Room.RoomName == search);
            }    

            if(!string.IsNullOrEmpty(status))
            {
                query = query.Where(c => c.Status == status);
            }

            var contracts = await query.ToListAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ContractTable", contracts);
            }    

            return View(contracts);
        }

        public async Task<IActionResult> Detail(long id)
        {
            var contract = await _context.Contracts
                .Include(c => c.Room)
                .Include(c => c.Tenant)
                .FirstOrDefaultAsync(c => c.Id == id);

            return View(contract);
        }

        public async Task<IActionResult> Create()
        {
            var tenants = await _context.Tenants.Select(t => new {t.Id, t.Name}).ToListAsync();

            var rooms = await _context.Rooms.Where(r => r.Status == "Trống").ToListAsync();

            ViewBag.Tenants = new SelectList(tenants, "Id", "Name");
            ViewBag.Rooms = rooms;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contract contract)
        {
            if (!ModelState.IsValid)
            {
                var tenants = await _context.Tenants.Select(t => new { t.Id, t.Name }).ToListAsync();

                var rooms = await _context.Rooms.Where(r => r.Status == "Trống").ToListAsync();

                ViewBag.Tenants = new SelectList(tenants, "Id", "Name");
                ViewBag.Rooms = rooms;

                return View(contract);
            }    

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var RoomUpdate = await _context.Rooms.FindAsync(contract.RoomId);

                if (RoomUpdate == null) return NotFound();

                if(RoomUpdate.Status != "Trống")
                {
                    TempData["Error"] = "Phòng này đã có người thuê, vui lòng chọn phòng khác";

                    var tenants = await _context.Tenants.Select(t => new { t.Id, t.Name }).ToListAsync();

                    var rooms = await _context.Rooms.Where(r => r.Status == "Trống").ToListAsync();

                    ViewBag.Tenants = new SelectList(tenants, "Id", "Name");
                    ViewBag.Rooms = rooms;

                    return View(contract);
                }

                contract.Status = "Còn hạn";
                _context.Contracts.Add(contract);

                RoomUpdate.Status = "Đã thuê";
                _context.Rooms.Update(RoomUpdate);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = "Tạo hợp đồng thành công";
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
            var tenants = await _context.Tenants.ToListAsync();

            var rooms = await _context.Rooms.Where(r => r.Status == "Trống").ToListAsync();

            ViewBag.Tenants = tenants;
            ViewBag.Rooms = rooms;

            var contract = await _context.Contracts
                .Include(c => c.Room)
                .Include(c => c.Tenant)
                .FirstOrDefaultAsync(c => c.Id == id);

            return View(contract);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Contract contract)
        {
            if (id != contract.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                var tenants = await _context.Tenants.Select(t => new { t.Id, t.Name }).ToListAsync();

                var rooms = await _context.Rooms.Where(r => r.Status == "Trống").ToListAsync();

                ViewBag.Tenants = new SelectList(tenants, "Id", "Name");
                ViewBag.Rooms = rooms;

                return View(contract);
            }    

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var currentContract = await _context.Contracts
                    .Include(c => c.Room)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (currentContract == null) return NotFound();

                if(currentContract.RoomId != contract.RoomId)
                {
                    if(currentContract.Room != null)
                    {
                        currentContract.Room.Status = "Trống";
                    }

                    var newRoom = await _context.Rooms.FindAsync(contract.RoomId);

                    if (newRoom == null) return NotFound();

                    newRoom.Status = "Đã thuê";
                }

                if(currentContract.Status != contract.Status)
                {
                    if(contract.Status == "Còn hạn")
                    {
                        if (currentContract.Room != null)
                        {
                            currentContract.Room.Status = "Đã thuê";
                        }
                    }    
                    else
                    {
                        if (currentContract.Room != null)
                        {
                            currentContract.Room.Status = "Trống";
                        }
                    }    
                }    

                currentContract.TenantId = contract.TenantId;
                currentContract.RoomId = contract.RoomId;
                currentContract.StartDay = contract.StartDay;
                currentContract.EndDay = contract.EndDay;
                currentContract.Deposit = contract.Deposit;
                currentContract.Status = contract.Status;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = "Cập nhật hợp đồng thành công";
                return RedirectToAction(nameof(Index));
            }
            catch(Exception)
            {
                await transaction.RollbackAsync();

                TempData["Error"] = "Có lỗi xảy ra, vui lòng thử lại sau!";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var contract = await _context.Contracts
                    .Include(c => c.Room)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (contract == null) return NotFound();

                if(contract.Status == "Còn hạn")
                {
                    TempData["Error"] = "Hợp đồng còn hạn, không được xóa!";
                    return RedirectToAction(nameof(Index));
                }    

                if(contract.Room != null)
                {
                    contract.Room.Status = "Trống";
                    _context.Rooms.Update(contract.Room);
                }

                contract.DeletedAt = DateTime.Now;
                _context.Contracts.Update(contract);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = "Xóa hợp đồng thành công";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                TempData["Error"] = "Có lỗi xảy ra, vui lòng thử lại sau!";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
