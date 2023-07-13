using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TiKayStore.Areas.Admin.Model
{
    public class EditPasswordModel
    {
        [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu")]
        public string passwordConfirm { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        public string passwordNew { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu cũ")]
        public string passwordOld { get; set; }
        public int id { get; set; }
    }
}