using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomManagement.Models;

namespace RoomManagement.Controllers
{
    [Authorize]
    public class BillsController : Controller
    {
        private readonly RoomManagementContext _context;

        public BillsController(RoomManagementContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string search, string status)
        {
            var query = _context.Bills
                .Include(b => b.Room)
                .ThenInclude(r => r.Contracts)
                .ThenInclude(c => c.Tenant)
                .AsQueryable();

            if(!string.IsNullOrEmpty(search))
            {
                query = query.Where(b =>  
                    b.Room.RoomName.Contains(search) ||
                    b.Room.Contracts.Any(c => c.Tenant.Name.Contains(search))
                );
            }    

            if(!string.IsNullOrEmpty(status))
            {
                query = query.Where(b => b.Status == status);
            }    

            var bills = await query.ToListAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_BillTable", bills);
            }    

            return View(bills);
        }

        public async Task<IActionResult> Detail(long id)
        {
            var bill = await _context.Bills
                .Include(b => b.Room)
                .Include(b => b.BillDetails)
                    .ThenInclude(b => b.Service)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (bill == null)
            {
                return NotFound();
            }

            return View(bill);
        }

        public async Task<IActionResult> Create()
        {
            var rooms = await _context.Rooms
                .Where(r => r.Status == "Đã thuê")
                .ToListAsync();

            var electric = await _context.Services
                .FirstOrDefaultAsync(s => s.ServiceName == "Điện");
            var water = await _context.Services
                .FirstOrDefaultAsync(s => s.ServiceName == "Nước");

            var fixedServices = await _context.Services
                .Where(s => s.ServiceName != "Điện" && s.ServiceName != "Nước")
                .ToListAsync();

            ViewBag.Rooms = rooms;
            ViewBag.Electric = electric;
            ViewBag.Water = water;
            ViewBag.FixedServices = fixedServices;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(long roomId, string monthYear, int electric_old, int electric_new, int water_old, int water_new)
        {
            if (string.IsNullOrEmpty(monthYear))
            {
                TempData["Error"] = "Vui lòng chọn tháng năm";
                return RedirectToAction(nameof(Index));
            }

            var parts = monthYear.Split('-');
            int year = int.Parse(parts[0]);
            int month = int.Parse(parts[1]);

            var room = await _context.Rooms.FindAsync(roomId);
            var electric = await _context.Services.FirstOrDefaultAsync(s => s.ServiceName == "Điện");
            var water = await _context.Services.FirstOrDefaultAsync(s => s.ServiceName == "Nước");
            var services = await _context.Services.Where(s => s.ServiceName != "Điện" && s.ServiceName != "Nước").ToListAsync();

            if (room == null || electric == null || water == null)
            {
                TempData["Error"] = "Thiếu dữ liệu phòng hoặc dịch vụ";
                return RedirectToAction(nameof(Index));
            }

            int electricUsed = electric_new - electric_old;
            int waterUsed = water_new - water_old;

            decimal ePrice = electric.Price ?? 0;
            decimal wPrice = water.Price ?? 0;

            decimal electricTotal = (decimal)electricUsed * ePrice;
            decimal waterTotal = (decimal)waterUsed * wPrice;
            decimal serviceTotal = electricTotal + waterTotal;

            foreach(var service in services)
            {
                decimal price = service.Price ?? 0;
                serviceTotal += price;
            }    

            decimal roomPrice = room.Price ?? 0;
            decimal total = roomPrice + serviceTotal;

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var bill = new Bill
                    {
                        RoomId = roomId,
                        Month = month,
                        Year = year,
                        RoomPrice = roomPrice,
                        ServiceTotal = serviceTotal,
                        Total = total,
                        CreatedAt = DateTime.Now,
                        Status = "Chưa thanh toán"
                    };
                    _context.Bills.Add(bill);
                    await _context.SaveChangesAsync();

                    var details = new List<BillDetail> {
                        new BillDetail {
                            BillId = bill.Id,
                            ServiceId = electric.Id,
                            OldIndex = electric_old,
                            NewIndex = electric_new,
                            Quantity = electricUsed,
                            Price = ePrice,
                            Total = electricTotal
                        },
                        new BillDetail {
                            BillId = bill.Id,
                            ServiceId = water.Id,
                            OldIndex = water_old,
                            NewIndex = water_new,
                            Quantity = waterUsed,
                            Price = wPrice,
                            Total = waterTotal
                        }
                    };

                    foreach(var service in services)
                    {
                        details.Add(
                            new BillDetail
                            {
                                BillId = bill.Id,
                                ServiceId = service.Id,
                                OldIndex = 0,
                                NewIndex = 0,
                                Quantity = 1,
                                Price = service.Price ?? 0,
                                Total = service.Price ?? 0
                            }
                        );
                    }

                    _context.BillDetails.AddRange(details);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    TempData["Success"] = "Tạo hóa đơn thành công";
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    TempData["Error"] = "Lỗi khi lưu dữ liệu";
                }
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(long id)
        {
            var bill = await _context.Bills
                .Include(b => b.Room)
                .Include(b => b.BillDetails)
                    .ThenInclude(d => d.Service)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bill == null) return NotFound();

            var electricService = await _context.Services.FirstOrDefaultAsync(s => s.ServiceName == "Điện");
            var waterService = await _context.Services.FirstOrDefaultAsync(s => s.ServiceName == "Nước");

            var electricDetail = bill.BillDetails.FirstOrDefault(d => d.ServiceId == electricService?.Id);
            var waterDetail = bill.BillDetails.FirstOrDefault(d => d.ServiceId == waterService?.Id);

            var fixedServices = await _context.Services
                .Where(s => s.ServiceName != "Điện" && s.ServiceName != "Nước")
                .ToListAsync();

            ViewBag.ElectricDetail = electricDetail;
            ViewBag.WaterDetail = waterDetail;
            ViewBag.FixedServices = fixedServices;

            return View(bill);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, string monthYear, int electric_old, int electric_new, int water_old, int water_new)
        {
            var bill = await _context.Bills
                .Include(b => b.BillDetails)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bill == null || string.IsNullOrEmpty(monthYear)) return NotFound();

            var parts = monthYear.Split('-');
            int year = int.Parse(parts[0]);
            int month = int.Parse(parts[1]);

            var electric = await _context.Services.FirstOrDefaultAsync(s => s.ServiceName == "Điện");
            var water = await _context.Services.FirstOrDefaultAsync(s => s.ServiceName == "Nước");

            int eUsed = electric_new - electric_old;
            int wUsed = water_new - water_old;
            decimal eTotal = (decimal)eUsed * (electric?.Price ?? 0);
            decimal wTotal = (decimal)wUsed * (water?.Price ?? 0);

            decimal otherServicesTotal = bill.BillDetails
                        .Where(b => b.ServiceId != electric?.Id && b.ServiceId != water?.Id)
                        .Sum(b => b.Total ?? 0);

            decimal sTotal = eTotal + wTotal + otherServicesTotal;

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    bill.Month = month;
                    bill.Year = year;
                    bill.ServiceTotal = sTotal;
                    bill.Total = (bill.RoomPrice ?? 0) + sTotal;
                    bill.UpdatedAt = DateTime.Now;

                    var eDetail = bill.BillDetails.FirstOrDefault(d => d.ServiceId == electric?.Id);
                    if (eDetail != null)
                    {
                        eDetail.OldIndex = electric_old;
                        eDetail.NewIndex = electric_new;
                        eDetail.Quantity = eUsed;
                        eDetail.Price = electric?.Price;
                        eDetail.Total = eTotal;
                    }

                    var wDetail = bill.BillDetails.FirstOrDefault(d => d.ServiceId == water?.Id);
                    if (wDetail != null)
                    {
                        wDetail.OldIndex = water_old;
                        wDetail.NewIndex = water_new;
                        wDetail.Quantity = wUsed;
                        wDetail.Price = water?.Price;
                        wDetail.Total = wTotal;
                    }

                    _context.Update(bill);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    TempData["Success"] = "Cập nhật thành công";
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    TempData["Error"] = "Cập nhật thất bại";
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(long id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var bill = await _context.Bills
                    .Include(b => b.BillDetails)
                    .Include(b => b.Payments)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (bill == null) return NotFound();

                if(bill.BillDetails != null && bill.BillDetails.Any())
                {
                    _context.BillDetails.RemoveRange(bill.BillDetails);
                }

                if(bill.Payments != null && bill.Payments.Any())
                {
                    foreach(var payment in bill.Payments)
                    {
                        payment.DeletedAt = DateTime.Now;
                    }    
                    _context.UpdateRange(bill.Payments);
                }

                bill.DeletedAt = DateTime.Now;
                _context.Update(bill);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = "Xóa hóa đơn thành công";
                return RedirectToAction(nameof(Index));
            }
            catch(Exception)
            {
                await transaction.RollbackAsync();

                TempData["Error"] = "Có lỗi xảy ra. Vui lòng thử lại sau!";
                return RedirectToAction(nameof(Index));
            }
        }

        [Route("get-tenant-by-room/{roomId}")]
        public async Task<IActionResult> GetTenantByRoom(long roomId)
        {
            var contract = await _context.Contracts
                .Include(c => c.Tenant)
                .Include(c => c.Room)
                .Where(c => c.RoomId == roomId)
                .OrderByDescending(c => c.StartDay)
                .FirstOrDefaultAsync();

            if (contract == null)
            {
                return Json(null);
            }

            return Json(new
            {
                tenant_id = contract.Tenant?.Id,
                tenant_name = contract.Tenant?.Name ?? "Không có",
                room_price = contract.Room?.Price ?? 0
            });
        }
    }
}
