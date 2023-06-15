using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TiKayStore.Areas.Admin.Model
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Vui lòng nhập email")]
        public string email { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        public string password { get; set; }
        public bool remember { get; set; }
    }
}