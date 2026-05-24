using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomManagement.Models;

namespace RoomManagement.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly RoomManagementContext _context;

        public DashboardController(RoomManagementContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            await AutoCheckExpiredContracts();

            var today = DateOnly.FromDateTime(DateTime.Now);
            var nextSevenDays = today.AddDays(7);

            var viewModel = new DashboardViewModel
            {
                TotalRooms = await _context.Rooms.CountAsync(),

                RentedRooms = await _context.Rooms.CountAsync(r => r.Status == "Đã thuê"),

                EmptyRooms = await _context.Rooms.CountAsync(r => r.Status == "Trống"),

                TotalTenants = await _context.Tenants.CountAsync(),

                AvailableRooms = await _context.Rooms
                    .Where(r => r.Status == "Trống")
                    .ToListAsync(),

                ExpiringContracts = await _context.Contracts
                    .Where(c => c.Status == "Còn hạn"
                             && c.EndDay.HasValue
                             && c.EndDay.Value >= today
                             && c.EndDay.Value <= nextSevenDays)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        private async Task AutoCheckExpiredContracts()
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);

            var expiredContracts = await _context.Contracts
                .Include(c => c.Room)
                .Where(c => c.Status == "Còn hạn" && c.EndDay.HasValue && c.EndDay.Value < today)
                .ToListAsync();

            if(expiredContracts.Any())
            {
                foreach(var contract in expiredContracts)
                {
                    contract.Status = "Hết hạn";
                    if(contract.Room != null)
                    {
                        contract.Room.Status = "Trống";
                    }    
                }
                await _context.SaveChangesAsync();
            }
        }
    }
}