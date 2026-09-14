using DoAn_Pc_DACS.Data; // Nhớ thêm thư viện này để gọi DbContext
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace DoAn_Pc_DACS.Controllers
{
    public class AccountController : Controller
    {
        // Khai báo kết nối Database
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<Models.Account> _passwordHasher;

        public AccountController(ApplicationDbContext context, IPasswordHasher<Models.Account> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // 1. Hiển thị form đăng nhập
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Admin");
            }
            return View();
        }

        // 2. Xử lý khi bấm nút Đăng nhập (Dò trong Database)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, bool rememberMe = false)
        {
            username = username?.Trim() ?? string.Empty;
            var user = _context.Accounts.FirstOrDefault(u => u.Username == username);
            var verificationResult = user == null
                ? PasswordVerificationResult.Failed
                : _passwordHasher.VerifyHashedPassword(user, user.Password, password ?? string.Empty);

            if (user != null && verificationResult != PasswordVerificationResult.Failed)
            {
                if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
                {
                    user.Password = _passwordHasher.HashPassword(user, password ?? string.Empty);
                    await _context.SaveChangesAsync();
                }

                // Nếu tìm thấy, cấp quyền dựa trên Role trong DB
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var claimsIdentity = new ClaimsIdentity(claims, "AdminCookie");
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = rememberMe
                };

                await HttpContext.SignInAsync("AdminCookie", new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToAction("Index", "Admin");
            }

            // Nếu không tìm thấy (sai user hoặc pass)
            ViewBag.Error = "Tài khoản hoặc mật khẩu không đúng!";
            return View();
        }

        // 3. Đăng xuất
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AdminCookie");
            return RedirectToAction("Login", "Account");
        }
        // 4. HIỂN THỊ FORM ĐỔI MẬT KHẨU
        // ====================================================
        [Authorize] // Phải đăng nhập mới được vào đây
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // ====================================================
        // 5. XỬ LÝ ĐỔI MẬT KHẨU XUỐNG DATABASE
        // ====================================================
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            // 1. Kiểm tra mật khẩu mới nhập lại có khớp không
            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu mới không khớp nhau!";
                return View();
            }

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 8)
            {
                ViewBag.Error = "Mật khẩu mới phải có ít nhất 8 ký tự!";
                return View();
            }

            // 2. Lấy tên tài khoản của người đang đăng nhập hiện tại
            string? currentUsername = User.Identity?.Name;

            // 3. Tìm user trong Database
            var user = _context.Accounts.FirstOrDefault(u => u.Username == currentUsername);

            if (user != null &&
                _passwordHasher.VerifyHashedPassword(user, user.Password, oldPassword ?? string.Empty)
                    != PasswordVerificationResult.Failed)
            {
                // Nếu đúng mật khẩu cũ -> Cập nhật mật khẩu mới
                user.Password = _passwordHasher.HashPassword(user, newPassword);
                await _context.SaveChangesAsync();

                ViewBag.Success = "Đổi mật khẩu thành công! Lần đăng nhập sau hãy dùng mật khẩu mới.";
                return View();
            }

            // Nếu sai mật khẩu cũ
            ViewBag.Error = "Mật khẩu cũ không chính xác!";
            return View();
        }
    }
}
