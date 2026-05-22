using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomManagement.Models;
using System.Security.Claims;

namespace RoomManagement.Controllers
{
    [Authorize(Roles = "admin")]
    public class AccountsController : Controller
    {
        private readonly RoomManagementContext _context;

        public AccountsController(RoomManagementContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string search, string role)
        {
            var currentUserName = User.Identity.Name;

            var query = _context.Users
                .Where(u => u.Name != currentUserName)
                .AsQueryable();

            if(!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => 
                            u.Name.Contains(search) ||
                            u.Email.Contains(search) ||
                            u.Phone.Contains(search));
            }    

            if(!string.IsNullOrEmpty(role))
            {
                query = query.Where(u => u.Role == role);
            }    

            var accounts = await query.ToListAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_AccountTable", accounts);
            }    

            return View(accounts);
        }

        public async Task<IActionResult> Detail(long id)
        {
            var account = await _context.Users.FindAsync(id);

            return View(account);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if(ModelState.IsValid)
            {
                bool isEmailExist = await _context.Users.AnyAsync(u => u.Email == user.Email);

                if (isEmailExist)
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng.");

                    return View(user);
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

                _context.Add(user);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Thêm tài khoản thành công";
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        public async Task<IActionResult> Edit(long id)
        {
            var account = await _context.Users.FindAsync(id);

            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, User user, string? OldPassword)
        {
            if (id != user.Id) return NotFound();

            var accountInDb = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

            if(accountInDb == null) return NotFound();

            if(!string.IsNullOrEmpty(user.Password))
            {
                bool isOldPassCorrect = BCrypt.Net.BCrypt.Verify(OldPassword, accountInDb.Password);

                if (!isOldPassCorrect)
                {
                    ModelState.AddModelError("OldPassword", "Mật khẩu củ không chính xác!");
                    return View(user);
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            }
            else
            {
                user.Password = accountInDb.Password;

                ModelState.Remove("OldPassword");
                ModelState.Remove("Password");
                ModelState.Remove("ConfirmPassword");
            }    

            if(ModelState.IsValid)
            {
                _context.Update(user);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Cập nhật thông tin tài khoản thành công";
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var account = await _context.Users.FindAsync(id);

            if(account == null) return NotFound();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(account.Id.ToString() == currentUserId)
            {
                return BadRequest("Bạn không thể tự xóa chính mình!");
            }    

            account.DeletedAt = DateTime.Now;

            _context.Update(account);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Xóa tài khoản thành công";
            return RedirectToAction(nameof(Index));
        }
    }
}
