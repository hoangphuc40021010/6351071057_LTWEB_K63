using System;
using System.Collections.Generic;
using PagedList;
using QL_Cuahangxe.Models;

namespace QL_Cuahangxe.Models
{
    public class MotobikeVM
    {
        // Danh sách xe gắn máy
        public IPagedList<XEGANMAY> Xeganmays { get; set; }

        // Danh sách loại xe
        public List<LOAIXE> Loaixes { get; set; }

        // Danh sách hãng sản xuất
        public List<HANGSANXUAT> Hangsanxuats { get; set; }
    }
}
