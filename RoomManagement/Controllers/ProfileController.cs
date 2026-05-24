using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomManagement.Models;
using System.Security.Claims;

namespace RoomManagement.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly RoomManagementContext _context;

        public ProfileController(RoomManagementContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (long.TryParse(userIdClaim, out long userIdLong))
            {
                var user = await _context.Users.FindAsync(userIdLong);

                if (user == null) return NotFound();

                return View(user);
            }

            return BadRequest("Định dạng ID người dùng không hợp lệ.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeInfo(long id, User user)
        {
            if(id != user.Id) return NotFound();

            var currentUser = await _context.Users.FindAsync(id);

            if (user.Name == currentUser.Name && user.Email == currentUser.Email && user.Phone == currentUser.Phone) return RedirectToAction(nameof(Index));

            if (!string.IsNullOrEmpty(user.Email) && user.Email == currentUser.Email)
            {
                currentUser.Email = user.Email;
            }
            else if(!string.IsNullOrEmpty(user.Email))
            {
                var isBoolEmail = await _context.Users.AnyAsync(u => u.Email == user.Email);

                if (isBoolEmail)
                {
                    TempData["Error"] = "Email này đã được sử dụng!";
                    return RedirectToAction(nameof(Index));
                }

                currentUser.Email = user.Email;
            }   
            else
            {
                TempData["Error"] = "Bạn không được phép để trống email!";
                return RedirectToAction(nameof(Index));
            }    

            currentUser.Name = user.Name;
            currentUser.Phone = user.Phone;

            _context.Update(currentUser);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật thông tin hồ sơ thành công";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(long id, User user, string? oldPass)
        {
            if(string.IsNullOrEmpty(oldPass) && user.Password == null && user.ConfirmPassword == null) return RedirectToAction(nameof(Index));

            if (id != user.Id) return NotFound(user);

            var currentUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

            if (currentUser == null) return NotFound();

            var isOldPassCorrect = BCrypt.Net.BCrypt.Verify(oldPass, currentUser.Password);

            if (!isOldPassCorrect)
            {
                TempData["Error"] = "Mật khẩu củ không chính xác!";
                return RedirectToAction(nameof(Index));
            }  
            
            currentUser.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            _context.Update(currentUser);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đổi mật khẩu thành công";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAvatar(IFormFile uploadFile)
        {
            if(uploadFile == null || uploadFile.Length == 0)
            {
                return Json(new { success = false, message = "Không tìm thấy file!" });
            }
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _context.Users.FindAsync(long.Parse(userId));

                if(!string.IsNullOrEmpty(user.Image))
                {
                    string oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", user.Image);

                    if(System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }    
                }

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(uploadFile.FileName);
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await uploadFile.CopyToAsync(stream);
                }

                user.Image = fileName;
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
