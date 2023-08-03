using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TiKayStore.Areas.Admin.Model
{
    public class SearchOrder
    {
        [Required(ErrorMessage = "Vui lòng tìm kiếm")]
        public string SearchString { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn loại tìm kiếm")]
        public string SearchType{ get; set; }
       
    }
}