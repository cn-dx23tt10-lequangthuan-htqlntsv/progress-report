using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RoomManagement.Models;

public partial class User
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public string? Image { get; set; }

    public string? Email { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [NotMapped] // Không tạo cột này trong Database
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
    public string? ConfirmPassword { get; set; }

    public string? Phone { get; set; }

    public string? Role { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}
