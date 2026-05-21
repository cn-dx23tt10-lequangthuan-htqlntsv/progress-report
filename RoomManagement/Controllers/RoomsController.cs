using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomManagement.Models;
using Microsoft.AspNetCore.Authorization;

public class RoomsController : Controller
{
    private readonly RoomManagementContext _context;

    public RoomsController(RoomManagementContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string search, string status)
    {
        var query = _context.Rooms.AsQueryable();

        if(!string.IsNullOrEmpty(search))
        {
            query = query.Where(r => r.RoomCode.Contains(search) || r.RoomName.Contains(search));
        }    

        if(!string.IsNullOrEmpty(status))
        {
            query = query.Where(r => r.Status == status);
        }    

        var rooms = await query.ToListAsync();

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return PartialView("_RoomTable", rooms);
        }    

        return View(rooms);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Room room)
    {
        if (ModelState.IsValid)
        {
            var lastRoom = await _context.Rooms
                .OrderByDescending(r => r.RoomCode)
                .FirstOrDefaultAsync();

            int number = 1;
            if (lastRoom != null && lastRoom.RoomCode.StartsWith("P"))
            {
                if (int.TryParse(lastRoom.RoomCode.Substring(1), out int lastNumber))
                {
                    number = lastNumber + 1;
                }
            }

            room.RoomCode = "P" + number.ToString("D3");

            _context.Add(room);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Thêm phòng thành công";
            return RedirectToAction(nameof(Index));
        }
        return View(room);
    }

    public async Task<IActionResult> Edit(long id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null) return NotFound();

        return View(room);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(long id, Room room)
    {
        if (id != room.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            _context.Update(room);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật thông tin phòng thành công";
            return RedirectToAction(nameof(Index));
        }
        return View(room);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(long id)
    {
        var room = await _context.Rooms
            .Include(r => r.Contracts)
            .Include(r => r.Bills)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (room == null) return NotFound();

        if(room.Contracts.Any(c => c.Status == "Còn hạn"))
        {
            TempData["Error"] = "Phòng đang có khách thuê. Hãy thanh lý hợp đồng trước!";
            return RedirectToAction(nameof(Index));
        }    

        if(room.Bills.Any(b => b.Status == "Chưa thanh toán"))
        {
            TempData["Error"] = "Phòng còn hóa đơn chưa thanh toán. Hãy thu tiền hoặc xử lý hóa đơn trước!";
            return RedirectToAction(nameof(Index));
        }

        room.DeletedAt = DateTime.Now;

        _context.Rooms.Update(room);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Xóa phòng thành công";
        return RedirectToAction(nameof(Index));
    }
}