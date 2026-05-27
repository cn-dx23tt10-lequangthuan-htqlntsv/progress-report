using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomManagement.Models;
using System.Security.Claims;

public class UserInfoViewComponent : ViewComponent
{
    private readonly RoomManagementContext _context;
    public UserInfoViewComponent(RoomManagementContext context) => _context = context;

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId)) return Content("");

        var user = await _context.Users.FindAsync(long.Parse(userId));

        return View(user);
    }
}