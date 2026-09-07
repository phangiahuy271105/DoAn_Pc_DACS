using System.ComponentModel.DataAnnotations;

namespace DoAn_Pc_DACS.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tài khoản")]
        [StringLength(50)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(255)]
        public string Password { get; set; }

        public string Role { get; set; } = "Admin";
    }
}