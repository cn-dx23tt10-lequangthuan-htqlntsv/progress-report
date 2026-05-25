using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RoomManagement.Models;
using System.Transactions;

namespace RoomManagement.Controllers
{
    [Authorize]
    public class PaymentsController : Controller
    {
        private readonly RoomManagementContext _context;

        public PaymentsController(RoomManagementContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string search, string paymentMethod)
        {
            var query = _context.Payments.AsQueryable();

            if(!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Id.ToString().Contains(search) || p.BillId.ToString().Contains(search));
            }    

            if(!string.IsNullOrEmpty(paymentMethod))
            {
                query = query.Where(p => p.PaymentMethod == paymentMethod);
            }    

            var payments = await query.ToListAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_PaymentTable", payments);
            }    

            return View(payments);
        }

        public async Task<IActionResult> Create()
        {
            var bills = await _context.Bills.Where(b => b.Status == "Chưa thanh toán").ToListAsync();

            ViewBag.Bills = bills;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Payment payment)
        {
            if(!ModelState.IsValid) return View(payment);

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var billUpdate = await _context.Bills.FindAsync(payment.BillId);

                if (billUpdate == null) return NotFound();

                billUpdate.Status = "Đã thanh toán";

                _context.Payments.Add(payment);

                _context.Bills.Update(billUpdate);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = "Thêm phiếu thanh toán thành công";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                TempData["Error"] = "Có lỗi xảy ra. Vui lòng thử lại sau!";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Edit(long id)
        {
            var payment = await _context.Payments.FindAsync(id);

            return View(payment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Payment payment)
        {
            if (id != payment.Id) return NotFound();

            var currentPayment = await _context.Payments.FindAsync(payment.Id);

            if (currentPayment == null) return NotFound();

            currentPayment.BillId = payment.BillId;
            currentPayment.Amount = payment.Amount;
            currentPayment.PaymentMethod = payment.PaymentMethod;
            currentPayment.PaymentDate = payment.PaymentDate;
            currentPayment.Note = payment.Note;

            _context.Payments.Update(currentPayment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật phiếu thanh toán thành công";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            using var Transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var payment = await _context.Payments
                    .Include(p => p.Bill)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (payment == null) return NotFound();

                if(payment.Bill != null && payment.Bill.Status == "Đã thanh toán")
                {
                    payment.Bill.Status = "Chưa thanh toán";
                    _context.Bills.Update(payment.Bill);
                }

                payment.DeletedAt = DateTime.Now;

                _context.Payments.Update(payment);
                await _context.SaveChangesAsync();
                await Transaction.CommitAsync();

                TempData["Success"] = "Xóa phiếu thanh toán thành công";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                await Transaction.RollbackAsync();

                TempData["Error"] = "Có lỗi xảy ra. Vui lòng thử lại sau!";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
