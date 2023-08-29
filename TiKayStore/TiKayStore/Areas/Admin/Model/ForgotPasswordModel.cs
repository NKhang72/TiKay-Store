using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TiKayStore.Areas.Admin.Model
{
    public class ForgotPasswordModel
    {
        [Required(ErrorMessage = "Vui lòng nhập email")]
        public string email { get; set; }
    }
}